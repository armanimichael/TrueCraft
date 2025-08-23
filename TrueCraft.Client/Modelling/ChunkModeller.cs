using System.Collections.Generic;
using TrueCraft.Core.World;
using TrueCraft.Core.Logic;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using TrueCraft.Client.Rendering;
using TrueCraft.Client.Modelling.Blocks;

namespace TrueCraft.Client.Modelling;

/// <summary>
/// A daemon of sorts that creates meshes from chunks.
/// Passing meshes back is NOT thread-safe.
/// </summary>
public class ChunkModeller : Modeller<IChunk>
{
    public int PendingChunks => _items.Count + _priorityItems.Count;

    private IDimension _dimension;
    private TrueCraftGame _game;

    public ChunkModeller(TrueCraftGame game, IDimension dimension)
        : base()
    {
        _dimension = dimension;
        _game = game;
    }

    private static readonly Vector3i[] AdjacentCoordinates =
    {
        Vector3i.Up,
        Vector3i.Down,
        Vector3i.North,
        Vector3i.South,
        Vector3i.East,
        Vector3i.West
    };

    private static readonly VisibleFaces[] AdjacentCoordFaces =
    {
        VisibleFaces.Bottom,
        VisibleFaces.Top,
        VisibleFaces.South,
        VisibleFaces.North,
        VisibleFaces.West,
        VisibleFaces.East
    };

    protected override bool TryRender(IChunk item, out MeshBase result)
    {
        var state = new RenderState();
        ProcessChunk(item, state);

        result = new ChunkMesh(
            _game,
            item,
            state.Verticies.ToArray(),
            state.OpaqueIndicies.ToArray(),
            state.TransparentIndicies.ToArray()
        );

        return result != null;
    }

    private sealed class RenderState
    {
        public readonly List<VertexPositionNormalColorTexture> Verticies = new();
        public readonly List<int> OpaqueIndicies = new();
        public readonly List<int> TransparentIndicies = new();
        public readonly Dictionary<LocalVoxelCoordinates, VisibleFaces> DrawableCoordinates = new();
    }

    private static void AddBottomBlock(LocalVoxelCoordinates coords, RenderState state, IChunk chunk)
    {
        var desiredFaces = VisibleFaces.None;

        if (coords.X == 0)
        {
            desiredFaces |= VisibleFaces.West;
        }
        else if (coords.X == WorldConstants.ChunkWidth - 1)
        {
            desiredFaces |= VisibleFaces.East;
        }

        if (coords.Z == 0)
        {
            desiredFaces |= VisibleFaces.North;
        }
        else if (coords.Z == WorldConstants.ChunkDepth - 1)
        {
            desiredFaces |= VisibleFaces.South;
        }

        if (coords.Y == 0)
        {
            desiredFaces |= VisibleFaces.Bottom;
        }
        else if (coords.Y == WorldConstants.ChunkDepth - 1)
        {
            desiredFaces |= VisibleFaces.Top;
        }

        VisibleFaces faces;
        state.DrawableCoordinates.TryGetValue(coords, out faces);
        faces |= desiredFaces;
        state.DrawableCoordinates[coords] = desiredFaces;
    }

    private void AddAdjacentBlocks(LocalVoxelCoordinates coords, RenderState state, IChunk chunk)
    {
        // Add adjacent blocks
        var blockRepository = _dimension.BlockRepository;

        for (var i = 0; i < AdjacentCoordinates.Length; i++)
        {
            var adjacent = AdjacentCoordinates[i];
            var nextX = coords.X + adjacent.X;
            var nextY = coords.Y + adjacent.Y;
            var nextZ = coords.Z + adjacent.Z;

            if (nextX < 0 || nextX >= WorldConstants.ChunkWidth || nextY < 0 || nextY >= WorldConstants.Height
                || nextZ < 0 || nextZ >= WorldConstants.ChunkDepth)
            {
                continue;
            }

            var next = new LocalVoxelCoordinates(nextX, nextY, nextZ);
            var provider = blockRepository.GetBlockProvider(chunk.GetBlockID(next));

            if (provider.Opaque)
            {
                VisibleFaces faces;

                if (!state.DrawableCoordinates.TryGetValue(next, out faces))
                {
                    faces = VisibleFaces.None;
                }

                faces |= AdjacentCoordFaces[i];
                state.DrawableCoordinates[next] = faces;
            }
        }
    }

    private static void AddTransparentBlock(LocalVoxelCoordinates coords, RenderState state, IChunk chunk)
    {
        // Add adjacent blocks
        var faces = VisibleFaces.None;

        for (var i = 0; i < AdjacentCoordinates.Length; i++)
        {
            var adjacent = AdjacentCoordinates[i];
            var nextX = coords.X + adjacent.X;
            var nextY = coords.Y + adjacent.Y;
            var nextZ = coords.Z + adjacent.Z;

            if (nextX < 0 || nextX >= WorldConstants.ChunkWidth || nextY < 0 || nextY >= WorldConstants.Height
                || nextZ < 0 || nextZ >= WorldConstants.ChunkDepth)
            {
                faces |= AdjacentCoordFaces[i];

                continue;
            }

            var next = new LocalVoxelCoordinates(nextX, nextY, nextZ);

            if (chunk.GetBlockID(next) == 0) // TODO hard-coded Air Block ID.
            {
                faces |= AdjacentCoordFaces[i];
            }
        }

        if (faces != VisibleFaces.None)
        {
            state.DrawableCoordinates[coords] = faces;
        }
    }

    private void UpdateFacesFromAdjacent(
        LocalVoxelCoordinates adjacent,
        IChunk chunk,
        VisibleFaces mod,
        ref VisibleFaces faces
    )
    {
        var provider = _dimension.BlockRepository.GetBlockProvider(chunk.GetBlockID(adjacent));

        if (!provider.Opaque)
        {
            faces |= mod;
        }
    }

    private void AddChunkBoundaryBlocks(LocalVoxelCoordinates coords, RenderState state, IChunk chunk)
    {
        VisibleFaces faces;

        if (!state.DrawableCoordinates.TryGetValue(coords, out faces))
        {
            faces = VisibleFaces.None;
        }

        var oldFaces = faces;

        var thisChunk = chunk.Coordinates;

        if (coords.X == 0)
        {
            var westChunk = new GlobalChunkCoordinates(thisChunk.X - 1, thisChunk.Z);
            var nextChunk = _dimension.GetChunk(westChunk);

            if (nextChunk is not null)
            {
                var adjacent = new LocalVoxelCoordinates(WorldConstants.ChunkWidth - 1, coords.Y, coords.Z);
                UpdateFacesFromAdjacent(adjacent, nextChunk, VisibleFaces.West, ref faces);
            }
        }
        else if (coords.X == WorldConstants.ChunkWidth - 1)
        {
            var eastChunk = new GlobalChunkCoordinates(thisChunk.X + 1, thisChunk.Z);
            var nextChunk = _dimension.GetChunk(eastChunk);

            if (nextChunk is not null)
            {
                var adjacent = new LocalVoxelCoordinates(0, coords.Y, coords.Z);
                UpdateFacesFromAdjacent(adjacent, nextChunk, VisibleFaces.East, ref faces);
            }
        }

        if (coords.Z == 0)
        {
            var northChunk = new GlobalChunkCoordinates(thisChunk.X, thisChunk.Z - 1);
            var nextChunk = _dimension.GetChunk(northChunk);

            if (nextChunk is not null)
            {
                var adjacent = new LocalVoxelCoordinates(coords.X, coords.Y, WorldConstants.ChunkDepth - 1);
                UpdateFacesFromAdjacent(adjacent, nextChunk, VisibleFaces.North, ref faces);
            }
        }
        else if (coords.Z == WorldConstants.ChunkDepth - 1)
        {
            var southChunk = new GlobalChunkCoordinates(thisChunk.X, thisChunk.Z + 1);
            var nextChunk = _dimension.GetChunk(southChunk);

            if (nextChunk is not null)
            {
                var adjacent = new LocalVoxelCoordinates(coords.X, coords.Y, 0);
                UpdateFacesFromAdjacent(adjacent, nextChunk, VisibleFaces.South, ref faces);
            }
        }

        if (oldFaces != faces)
        {
            state.DrawableCoordinates[coords] = faces;
        }
    }

    private void ProcessChunk(IChunk chunk, RenderState state)
    {
        state.Verticies.Clear();
        state.OpaqueIndicies.Clear();
        state.TransparentIndicies.Clear();
        state.DrawableCoordinates.Clear();

        var blockRepository = _dimension.BlockRepository;

        for (byte x = 0; x < WorldConstants.ChunkWidth; x++)
        for (byte z = 0; z < WorldConstants.ChunkDepth; z++)
        for (byte y = 0; y < WorldConstants.Height; y++)
        {
            var coords = new LocalVoxelCoordinates(x, y, z);
            var id = chunk.GetBlockID(coords);
            var provider = blockRepository.GetBlockProvider(id);

            if (id != 0 && coords.Y == 0)
            {
                        AddBottomBlock(coords, state, chunk);
            }

            if (!provider.Opaque)
            {
                AddAdjacentBlocks(coords, state, chunk);

                if (id != 0)
                {
                            AddTransparentBlock(coords, state, chunk);
                }
            }
            else
            {
                if (coords.X == 0 || coords.X == WorldConstants.ChunkWidth - 1 ||
                    coords.Z == 0 || coords.Z == WorldConstants.ChunkDepth - 1)
                {
                    AddChunkBoundaryBlocks(coords, state, chunk);
                }
            }
        }

        foreach (var coords in state.DrawableCoordinates)
        {
            var c = coords.Key;

            var descriptor = new BlockDescriptor
                             {
                                 ID = chunk.GetBlockID(c),
                                 Metadata = chunk.GetMetadata(c),
                                 BlockLight = chunk.GetBlockLight(c),
                                 SkyLight = chunk.GetSkyLight(c),
                                 Coordinates = GlobalVoxelCoordinates.GetGlobalVoxelCoordinates(chunk.Coordinates, c),
                                 Chunk = chunk
                             };

            var provider = blockRepository.GetBlockProvider(descriptor.ID);

            if (provider.RenderOpaque)
            {
                int[] i;

                // TODO: fix adhoc inline coordinate conversion
                var v = BlockModeller.RenderBlock(
                    provider,
                    descriptor,
                    coords.Value,
                    new Vector3(
                        (chunk.X * WorldConstants.ChunkWidth) + c.X,
                        c.Y,
                        (chunk.Z * WorldConstants.ChunkDepth) + c.Z
                    ),
                    state.Verticies.Count,
                    out i
                );

                state.Verticies.AddRange(v);
                state.OpaqueIndicies.AddRange(i);
            }
            else
            {
                int[] i;

                // TODO: fix adhoc inline coordinate conversion
                var v = BlockModeller.RenderBlock(
                    provider,
                    descriptor,
                    coords.Value,
                    new Vector3(
                        (chunk.X * WorldConstants.ChunkWidth) + c.X,
                        c.Y,
                        (chunk.Z * WorldConstants.ChunkDepth) + c.Z
                    ),
                    state.Verticies.Count,
                    out i
                );

                state.Verticies.AddRange(v);
                state.TransparentIndicies.AddRange(i);
            }
        }
    }
}