using System;
using System.Collections;
using System.Collections.Generic;
using TrueCraft.Core.Logic;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Test.World;

public class FakeDimension : IDimension
{
    private readonly byte _surfaceHeight;

    private readonly IBlockRepository _blockRepository;

    private readonly IItemRepository _itemRepository;

    private Dictionary<GlobalChunkCoordinates, IChunk> _chunks;

    public FakeDimension(IBlockRepository blockRepository, IItemRepository itemRepository)
    {
        _surfaceHeight = FakeChunk.DefaultSurfaceHeight;
        _blockRepository = blockRepository;
        _itemRepository = itemRepository;
        _chunks = new Dictionary<GlobalChunkCoordinates, IChunk>(434);

        var chunkCoordinates = new GlobalChunkCoordinates(0, 0);
        _chunks[chunkCoordinates] = new FakeChunk(chunkCoordinates);
    }

    public FakeDimension(
        IBlockRepository blockRepository,
        IItemRepository itemRepository,
        byte surfaceHeight
    )
    {
        _surfaceHeight = surfaceHeight;
        _blockRepository = blockRepository;
        _itemRepository = itemRepository;
        _chunks = new Dictionary<GlobalChunkCoordinates, IChunk>(434);

        var chunkCoordinates = new GlobalChunkCoordinates(0, 0);
        _chunks[chunkCoordinates] = new FakeChunk(chunkCoordinates, _surfaceHeight);
    }

    public DimensionID ID => DimensionID.Overworld;

    public string Name => "Fake";

    public IBlockRepository BlockRepository => _blockRepository;

    /// <inheritdoc/>
    public IItemRepository ItemRepository => _itemRepository;

    public long TimeOfDay
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IChunk? GetChunk(GlobalVoxelCoordinates coordinates) => GetChunk((GlobalChunkCoordinates) coordinates);

    public IChunk? GetChunk(GlobalChunkCoordinates coordinates)
    {
        if (_chunks.ContainsKey(coordinates))
        {
            return _chunks[coordinates];
        }

        return null;
    }

    public LocalVoxelCoordinates FindBlockPosition(GlobalVoxelCoordinates coordinates, out IChunk? chunk)
    {
        chunk = GetChunk(coordinates);

        return (LocalVoxelCoordinates) coordinates;
    }

    public BlockDescriptor GetBlockData(GlobalVoxelCoordinates coordinates) => throw new NotImplementedException();

    public byte GetBlockID(GlobalVoxelCoordinates coordinates)
    {
        var chunk = GetChunk(coordinates);

        return chunk?.GetBlockID((LocalVoxelCoordinates) coordinates) ?? 0;
    }

    public byte GetBlockLight(GlobalVoxelCoordinates coordinates)
    {
        var chunk = GetChunk(coordinates);

        return chunk?.GetBlockLight((LocalVoxelCoordinates) coordinates) ?? 0;
    }

    public byte GetMetadata(GlobalVoxelCoordinates coordinates)
    {
        var chunk = GetChunk(coordinates);

        return chunk?.GetMetadata((LocalVoxelCoordinates) coordinates) ?? 0;
    }

    public byte GetSkyLight(GlobalVoxelCoordinates coordinates)
    {
        var chunk = GetChunk(coordinates);

        return chunk?.GetSkyLight((LocalVoxelCoordinates) coordinates) ?? 0;
    }

    public bool IsChunkLoaded(GlobalVoxelCoordinates coordinates) => _chunks.ContainsKey((GlobalChunkCoordinates) coordinates);

    public bool IsValidPosition(GlobalVoxelCoordinates position) => throw new NotImplementedException();

    public void SetBlockData(GlobalVoxelCoordinates coordinates, BlockDescriptor block) => throw new NotImplementedException();

    public void SetBlockID(GlobalVoxelCoordinates coordinates, byte value)
    {
        var chunk = GetChunk(coordinates);
        chunk?.SetBlockID((LocalVoxelCoordinates) coordinates, value);
    }

    public void SetBlockLight(GlobalVoxelCoordinates coordinates, byte value)
    {
        var chunk = GetChunk(coordinates);
        chunk?.SetBlockLight((LocalVoxelCoordinates) coordinates, value);
    }

    public void SetMetadata(GlobalVoxelCoordinates coordinates, byte value)
    {
        var chunk = GetChunk(coordinates);
        chunk?.SetMetadata((LocalVoxelCoordinates) coordinates, value);
    }

    public void SetSkyLight(GlobalVoxelCoordinates coordinates, byte value)
    {
        var chunk = GetChunk(coordinates);
        chunk?.SetSkyLight((LocalVoxelCoordinates) coordinates, value);
    }

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    public IEnumerator<IChunk> GetEnumerator() => throw new NotImplementedException();
}