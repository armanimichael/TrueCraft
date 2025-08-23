using System;
using System.Collections;
using System.Collections.Generic;
using fNbt;
using TrueCraft.Core.Lighting;
using TrueCraft.Core.Logging;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.World;

public sealed class Dimension : IDisposable, IDimensionServer, IEnumerable<IChunk>, IEquatable<IDimension>
{
    private readonly DimensionID _dimensionID;

    private readonly IMultiplayerServer _server;

    private readonly IEntityManager _entityManager;

    /// <inheritdoc />
    public string Name { get; }

    public readonly string _baseDirectory;

    private readonly IDictionary<RegionCoordinates, IRegion> _regions;

    private readonly IChunkProvider _chunkProvider;

    private readonly IBlockRepository _blockRepository;

    private readonly IItemRepository _itemRepository;

    /// <summary>
    /// Chunks in this Queue are waiting to have their BlockLoaded methods called for each block.
    /// </summary>
    private readonly Queue<IChunk> _recentlyLoadedChunks = new();

    private bool _blockUpdatesEnabled = true;

    /// <summary>
    /// A list of coordinates where block updates are pending.
    /// </summary>
    private readonly Queue<GlobalVoxelCoordinates> _pendingBlockUpdates = new();

    private readonly ILightingQueue _lightingQueue;

    /// <inheritdoc/>
    public IBlockRepository BlockRepository => _blockRepository;

    /// <inheritdoc/>
    public IItemRepository ItemRepository => _itemRepository;

    private DateTime _baseTime;

    public long TimeOfDay
    {
        get => (long) ((DateTime.UtcNow - _baseTime).TotalSeconds * 20) % 24000;
        set => _baseTime = DateTime.UtcNow.AddSeconds(-value / 20);
    }

    public event EventHandler<BlockChangeEventArgs>? BlockChanged;
    public event EventHandler<ChunkLoadedEventArgs>? ChunkGenerated;
    public event EventHandler<ChunkLoadedEventArgs>? ChunkLoaded;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceLocator"></param>
    /// <param name="baseDirectory"></param>
    /// <param name="dimensionID"></param>
    /// <param name="chunkProvider"></param>
    /// <param name="lightingQueue">The Lighting Queue for lighting this Dimension</param>
    /// <param name="entityManager"></param>
    public Dimension(
        IServerServiceLocator serviceLocator,
        string baseDirectory,
        DimensionID dimensionID,
        IChunkProvider chunkProvider,
        ILightingQueue lightingQueue,
        IEntityManager entityManager
    )
    {
        _dimensionID = dimensionID;
        _server = serviceLocator.Server;
        _entityManager = entityManager;
        _baseDirectory = baseDirectory;
        Name = DimensionInfo.GetName(dimensionID);
        _chunkProvider = chunkProvider;
        _lightingQueue = lightingQueue;
        _blockRepository = serviceLocator.BlockRepository;
        _itemRepository = serviceLocator.ItemRepository;
        _regions = new Dictionary<RegionCoordinates, IRegion>();
        _baseTime = DateTime.UtcNow;
    }

    private static string DimensionIDToFolder(DimensionID id)
    {
        if (id == DimensionID.Overworld)
        {
            return string.Empty;
        }

        return "DIM" + (byte) id;
    }

    // TODO: Update IMultiplayerServer to a logger.
    public void Initialize(
        GlobalChunkCoordinates spawnChunk,
        IMultiplayerServer server,
        IDimensionServer.ProgressNotification? progressNotification
    )
    {
        server.Log(LogCategory.Notice, "Generating world around spawn point...");
        var chunkRadius = 5;
        var lastLoggedProgress = -10;

        for (var x = -chunkRadius; x < chunkRadius; x++)
        {
            for (var z = -chunkRadius; z < chunkRadius; z++)
            {
                GetChunk(new GlobalChunkCoordinates(spawnChunk.X + x, spawnChunk.Z + z), LoadEffort.Generate);
            }

            var progress = (int) ((x + chunkRadius) / (2.0 * chunkRadius) * 100);
            progressNotification?.Invoke(progress / 100.0, "Generating world...");

            if (progress / 10 > lastLoggedProgress / 10)
            {
                server.Log(LogCategory.Notice, "{0}% complete", progress + 10);
                lastLoggedProgress = progress;
            }
        }

        server.Log(LogCategory.Notice, "Simulating the world for a moment...");
        lastLoggedProgress = -10;

        for (var x = -chunkRadius; x < chunkRadius; x++)
        {
            for (var z = -chunkRadius; z < chunkRadius; z++)
            {
                var chunkCoordinates = new GlobalChunkCoordinates(spawnChunk.X + x, spawnChunk.Z + z);
                var chunk = GetChunk(chunkCoordinates)!;

                for (byte _x = 0; _x < WorldConstants.ChunkWidth; _x++)
                for (byte _z = 0; _z < WorldConstants.ChunkDepth; _z++)
                for (var _y = 0; _y < chunk.GetHeight(_x, _z); _y++)
                {
                    var localCoordinates = new LocalVoxelCoordinates(_x, _y, _z);
                    var coords = GlobalVoxelCoordinates.GetGlobalVoxelCoordinates(chunkCoordinates, localCoordinates);

                    var data = GetBlockData(
                        coords
                    ); // TODO: we ought to get this from the Chunk rather than the Dimension

                    var provider = _blockRepository.GetBlockProvider(data.ID);
                    // TODO: do we really want a block update? This will cause
                    //    generated suspended sand to fall.  Maybe this should
                    //    be an "on loaded"?
                    provider.BlockUpdate(data, data, server, this);
                }
            }

            var progress = (int) ((x + chunkRadius) / (2.0 * chunkRadius) * 100);
            progressNotification?.Invoke(progress / 100.0, "Simulating world...");

            if (progress / 10 > lastLoggedProgress / 10)
            {
                server.Log(LogCategory.Notice, "{0}% complete", progress + 10);
                lastLoggedProgress = progress;
            }
        }
    }

    /// <inheritdoc />
    public IEntityManager EntityManager => _entityManager;

    #region object overrides

    public override int GetHashCode() => _dimensionID.GetHashCode();

    public override bool Equals(object? obj) => Equals(obj as IDimension);

    public override string ToString() => Name;

    #endregion

    #region IEquatable<IDimension>

    public bool Equals(IDimension? other)
    {
        if (other is null)
        {
            return false;
        }

        return _dimensionID == other.ID;
    }

    #endregion

    /// <inheritdoc />
    public DimensionID ID => _dimensionID;

    /// <inheritdoc />
    public IChunk? GetChunk(GlobalVoxelCoordinates coordinates)
    {
        var regionCoordinates = (RegionCoordinates) coordinates;
        IRegion region;

        lock (_regions)
        {
            if (!_regions.ContainsKey(regionCoordinates))
            {
                return null;
            }

            region = _regions[regionCoordinates];

            var chunkCoordinates = (LocalChunkCoordinates) coordinates;

            if (!region.IsChunkLoaded(chunkCoordinates))
            {
                return null;
            }

            return region.GetChunk(chunkCoordinates);
        }
    }

    /// <inheritdoc />
    public IChunk? GetChunk(GlobalChunkCoordinates coordinates)
    {
        var regionCoordinates = (RegionCoordinates) coordinates;
        IRegion region;

        lock (_regions)
        {
            if (!_regions.ContainsKey(regionCoordinates))
            {
                return null;
            }

            region = _regions[regionCoordinates];

            var chunkCoordinates = (LocalChunkCoordinates) coordinates;

            if (!region.IsChunkLoaded(chunkCoordinates))
            {
                return null;
            }

            return region.GetChunk(chunkCoordinates);
        }
    }

    /// <inheritdoc />
    public IChunk? GetChunk(GlobalChunkCoordinates coordinates, LoadEffort loadEffort)
    {
        var regionCoordinates = (RegionCoordinates) coordinates;

        lock (_regions)
        {
            IRegion region;

            if (!_regions.ContainsKey(regionCoordinates))
            {
                if (loadEffort == LoadEffort.InMemory)
                {
                    return null;
                }

                if (!Region.DoesRegionExistOnDisk(regionCoordinates, _baseDirectory) &&
                    loadEffort == LoadEffort.Load)
                {
                    return null;
                }

                region = new Region(regionCoordinates, _baseDirectory);
                _regions[regionCoordinates] = region;
            }
            else
            {
                region = _regions[regionCoordinates];
            }

            var chunkCoordinates = (LocalChunkCoordinates) coordinates;

            if (!region.IsChunkLoaded(chunkCoordinates) && loadEffort == LoadEffort.InMemory)
            {
                return null;
            }

            var chunk = region.GetChunk(chunkCoordinates);

            if (chunk is null && (loadEffort == LoadEffort.Load || loadEffort == LoadEffort.Generate))
            {
                chunk = region.LoadChunk(chunkCoordinates);

                if (chunk is null && loadEffort == LoadEffort.Generate)
                {
                    chunk = _chunkProvider.GenerateChunk(coordinates);
                    region.AddChunk(chunk);
                    OnChunkGenerated(new ChunkLoadedEventArgs(chunk));
                }
            }

            return chunk;
        }
    }

    /// <inheritdoc />
    public byte GetBlockID(GlobalVoxelCoordinates coordinates) => GetBlockID(coordinates, LoadEffort.InMemory);

    /// <inheritdoc />
    public byte GetBlockID(GlobalVoxelCoordinates coordinates, LoadEffort loadEffort)
    {
        var chunk = GetChunk((GlobalChunkCoordinates) coordinates, loadEffort);

        return chunk?.GetBlockID((LocalVoxelCoordinates) coordinates) ?? AirBlock.BlockID;
    }

    /// <inheritdoc />
    public byte GetMetadata(GlobalVoxelCoordinates coordinates) => GetMetadata(coordinates, LoadEffort.InMemory);

    public byte GetMetadata(GlobalVoxelCoordinates coordinates, LoadEffort loadEffort)
    {
        var chunk = GetChunk((GlobalChunkCoordinates) coordinates, loadEffort);

        return chunk?.GetMetadata((LocalVoxelCoordinates) coordinates) ?? 0;
    }

    /// <inheritdoc />
    public byte GetBlockLight(GlobalVoxelCoordinates coordinates) => GetBlockLight(coordinates, LoadEffort.InMemory);

    /// <inheritdoc />
    public byte GetBlockLight(GlobalVoxelCoordinates coordinates, LoadEffort loadEffort)
    {
        var chunk = GetChunk((GlobalChunkCoordinates) coordinates, loadEffort);

        return chunk?.GetBlockLight((LocalVoxelCoordinates) coordinates) ?? 0;
    }

    /// <inheritdoc />
    public byte GetSkyLight(GlobalVoxelCoordinates coordinates) => GetSkyLight(coordinates, LoadEffort.InMemory);

    /// <inheritdoc />
    public byte GetSkyLight(GlobalVoxelCoordinates coordinates, LoadEffort loadEffort)
    {
        var chunk = GetChunk((GlobalChunkCoordinates) coordinates, loadEffort);

        return chunk?.GetSkyLight((LocalVoxelCoordinates) coordinates) ?? 0;
    }

    /// <inheritdoc />
    public NbtCompound? GetTileEntity(GlobalVoxelCoordinates coordinates)
    {
        IChunk? chunk;
        var local = FindBlockPosition(coordinates, out chunk);

        return chunk?.GetTileEntity(local) ?? null;
    }

    /// <inheritdoc />
    public BlockDescriptor GetBlockData(GlobalVoxelCoordinates coordinates)
    {
        IChunk? chunk;
        var local = FindBlockPosition(coordinates, out chunk);

        if (chunk is null)
        {
            var rv = new BlockDescriptor();
            rv.ID = AirBlock.BlockID;
            rv.Metadata = 0;
            rv.BlockLight = 0;
            rv.SkyLight = 0;
            rv.Coordinates = coordinates;

            return rv;
        }

        return GetBlockDataFromChunk(local, chunk, coordinates);
    }

    /// <inheritdoc />
    public void SetBlockData(GlobalVoxelCoordinates coordinates, BlockDescriptor descriptor)
    {
        IChunk? chunk;
        var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);

        if (chunk is null)
        {
            return;
        }

        var old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);

        if (old.ID == descriptor.ID && old.Metadata == descriptor.Metadata)
        {
            return;
        }

        chunk.SetBlockID(adjustedCoordinates, descriptor.ID);
        chunk.SetMetadata(adjustedCoordinates, descriptor.Metadata);

        OnBlockChanged(
            new BlockChangeEventArgs(
                coordinates,
                old,
                GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)
            )
        );
    }

    private static BlockDescriptor GetBlockDataFromChunk(
        LocalVoxelCoordinates adjustedCoordinates,
        IChunk chunk,
        GlobalVoxelCoordinates coordinates
    ) => new()
         {
             ID = chunk.GetBlockID(adjustedCoordinates),
             Metadata = chunk.GetMetadata(adjustedCoordinates),
             BlockLight = chunk.GetBlockLight(adjustedCoordinates),
             SkyLight = chunk.GetSkyLight(adjustedCoordinates),
             Coordinates = coordinates
         };

    /// <inheritdoc />
    public void SetBlockID(GlobalVoxelCoordinates coordinates, byte value)
    {
        IChunk? chunk;
        var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);

        if (chunk is null)
        {
            return;
        }

        var old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);

        if (old.ID == value)
        {
            return;
        }

        chunk.SetBlockID(adjustedCoordinates, value);

        OnBlockChanged(
            new BlockChangeEventArgs(
                coordinates,
                old,
                GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)
            )
        );
    }

    /// <inheritdoc />
    public void SetMetadata(GlobalVoxelCoordinates coordinates, byte value)
    {
        IChunk? chunk;
        var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);

        if (chunk is null)
        {
            return;
        }

        var old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);

        if (old.Metadata == value)
        {
            return;
        }

        chunk.SetMetadata(adjustedCoordinates, value);

        OnBlockChanged(
            new BlockChangeEventArgs(
                coordinates,
                old,
                GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)
            )
        );
    }

    /// <inheritdoc />
    public void SetSkyLight(GlobalVoxelCoordinates coordinates, byte value)
    {
        IChunk? chunk;
        var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);

        if (chunk is null)
        {
            return;
        }

        chunk.SetSkyLight(adjustedCoordinates, value);
    }

    /// <inheritdoc />
    public void SetBlockLight(GlobalVoxelCoordinates coordinates, byte value)
    {
        IChunk? chunk;
        var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);

        if (chunk is null)
        {
            return;
        }

        chunk.SetBlockLight(adjustedCoordinates, value);
    }

    /// <inheritdoc />
    public void SetTileEntity(GlobalVoxelCoordinates coordinates, NbtCompound? value)
    {
        IChunk? chunk;
        var local = FindBlockPosition(coordinates, out chunk);

        if (chunk is null)
        {
            return;
        }

        chunk.SetTileEntity(local, value);
    }

    /// <inheritdoc />
    public void Save()
    {
        lock (_regions)
        {
            foreach (var region in _regions)
            {
                region.Value.Save();
            }
        }
    }

    /// <inheritdoc />
    public LocalVoxelCoordinates FindBlockPosition(GlobalVoxelCoordinates blockCoordinates, out IChunk? chunk)
    {
        var globalChunk = (GlobalChunkCoordinates) blockCoordinates;
        chunk = GetChunk(globalChunk);

        return (LocalVoxelCoordinates) blockCoordinates;
    }

    /// <inheritdoc />
    public bool IsValidPosition(GlobalVoxelCoordinates position) => position.Y >= 0 && position.Y < Chunk.Height;

    /// <inheritdoc />
    public bool IsChunkLoaded(GlobalVoxelCoordinates blockCoordinates)
    {
        var regionCoordinates = (RegionCoordinates) blockCoordinates;

        if (!_regions.ContainsKey(regionCoordinates))
        {
            return false;
        }

        var local = (LocalChunkCoordinates) blockCoordinates;

        return _regions[regionCoordinates].IsChunkLoaded(local);
    }

    private IRegion GetRegion(RegionCoordinates coordinates)
    {
        lock (_regions)
        {
            if (_regions.ContainsKey(coordinates))
            {
                return (Region) _regions[coordinates];
            }
        }

        IRegion region = new Region(coordinates, _baseDirectory);
        region.ChunkLoaded += HandleChunkLoaded;

        lock (_regions)
        {
            _regions[coordinates] = region;
        }

        return region;
    }

    public void Dispose()
    {
        foreach (var region in _regions.Values)
        {
            region.ChunkLoaded -= HandleChunkLoaded;
            region.Dispose();
        }

        _regions.Clear();

        BlockChanged = null;
        ChunkGenerated = null;
        ChunkLoaded = null;
    }

    protected internal void OnBlockChanged(BlockChangeEventArgs e)
    {
        // TODO: Move processing of block updates to the tick method.
        _pendingBlockUpdates.Enqueue(e.Position);
        ProcessBlockUpdates();

        if (Program.ServerConfiguration?.EnableLighting ?? ServerConfiguration.EnableLightingDefault)
        {
            var oldBlock = BlockRepository.GetBlockProvider(e.OldBlock.ID);
            var newBlock = BlockRepository.GetBlockProvider(e.NewBlock.ID);

            if (newBlock.Luminance > oldBlock.Luminance)
            {
                _lightingQueue.Enqueue(
                    e.Position,
                    LightingOperationMode.Add,
                    LightingOperationKind.Block,
                    newBlock.Luminance
                );
            }
            else if (oldBlock.Luminance > newBlock.Luminance)
            {
                _lightingQueue.Enqueue(
                    e.Position,
                    LightingOperationMode.Subtract,
                    LightingOperationKind.Block,
                    newBlock.Luminance
                );
            }
            else if (oldBlock.LightOpacity != newBlock.LightOpacity)
            {
                _lightingQueue.Enqueue(e.Position, LightingOperationMode.BlockUpdate, LightingOperationKind.Block, 0);
                _lightingQueue.Enqueue(e.Position, LightingOperationMode.BlockUpdate, LightingOperationKind.Sky, 0);
            }
        }

        BlockChanged?.Invoke(this, e);
    }

    private void ProcessBlockUpdates()
    {
        if (!_blockUpdatesEnabled)
        {
            return;
        }

        while (_pendingBlockUpdates.Count != 0)
        {
            var update = _pendingBlockUpdates.Dequeue();
            var source = GetBlockData(update);

            foreach (var offset in Vector3i.Neighbors6)
            {
                var descriptor = GetBlockData(update + offset);
                var provider = BlockRepository.GetBlockProvider(descriptor.ID);
                provider?.BlockUpdate(descriptor, source, _server, this);
            }
        }
    }

    protected internal void OnChunkGenerated(ChunkLoadedEventArgs e)
    {
        if (Program.ServerConfiguration?.EnableLighting ?? true)
        {
            _lightingQueue.Enqueue(
                (GlobalVoxelCoordinates) e.Chunk.Coordinates,
                LightingOperationMode.Add,
                LightingOperationKind.Initial,
                15
            );
        }
        else
        {
            var chunk = e.Chunk;

            for (int i = 0, iul = chunk.SkyLight.Length; i < iul; i++)
            {
                chunk.SkyLight[i] = 0x0F;
                chunk.BlockLight[i] = 0x0F;
            }
        }

        ChunkGenerated?.Invoke(this, e);
    }

    private void HandleChunkLoaded(object? sender, ChunkLoadedEventArgs e)
    {
        var chunk = e.Chunk;

        if (Program.ServerConfiguration?.EnableEventLoading ?? ServerConfiguration.EnableEventLoadingDefault)
        {
            _recentlyLoadedChunks.Enqueue(chunk);
        }

        if (Program.ServerConfiguration?.EnableLighting ?? ServerConfiguration.EnableLightingDefault)
        {
            _lightingQueue.Enqueue(
                (GlobalVoxelCoordinates) chunk.Coordinates,
                LightingOperationMode.Add,
                LightingOperationKind.Initial,
                15
            );
        }

        ChunkLoaded?.Invoke(this, e);
    }

    /// <inheritdoc />
    public string ChunkProvider => _chunkProvider.GetType().FullName!;

    #region IEnumerable<IChunk>

    public IEnumerator<IChunk> GetEnumerator()
    {
        var chunks = new List<IChunk>();

        foreach (var region in _regions.Values)
        foreach (var chunk in region.Chunks)
        {
            chunks.Add(chunk);
        }

        return chunks.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}