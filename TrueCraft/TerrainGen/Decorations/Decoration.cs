using System;
using TrueCraft.Core;
using TrueCraft.Core.World;
using TrueCraft.World;

namespace TrueCraft.TerrainGen.Decorations;

public abstract class Decoration : IDecoration
{
    public virtual bool ValidLocation(LocalVoxelCoordinates location) => true;

    public abstract bool GenerateAt(int seed, IChunk chunk, LocalVoxelCoordinates location);

    protected static bool IsCuboidWall(LocalVoxelCoordinates location, LocalVoxelCoordinates start, Vector3 size) => location.X.Equals(start.X)
                                                                                                                     || location.Z.Equals(start.Z)
                                                                                                                     || location.X.Equals(start.X + (int) size.X - 1)
                                                                                                                     || location.Z.Equals(start.Z + (int) size.Z - 1);

    protected static bool IsCuboidCorner(LocalVoxelCoordinates location, LocalVoxelCoordinates start, Vector3 size) => (location.X.Equals(start.X) && location.Z.Equals(start.Z))
                                                                                                                       || (location.X.Equals(start.X) &&
                                                                                                                           location.Z.Equals(start.Z + (int) size.Z - 1))
                                                                                                                       || (location.X.Equals(start.X + (int) size.X - 1) &&
                                                                                                                           location.Z.Equals(start.Z))
                                                                                                                       || (location.X.Equals(start.X + (int) size.X - 1) &&
                                                                                                                           location.Z.Equals(start.Z + (int) size.Z - 1));

    public static bool NeighboursBlock(IChunk chunk, LocalVoxelCoordinates location, byte block, byte meta = 0x0)
    {
        foreach (var neighbor in Vector3i.Neighbors4)
        {
            var x = location.X + neighbor.X;
            var y = location.Y + neighbor.Y;
            var z = location.Z + neighbor.Z;

            if (x < 0 || x >= WorldConstants.ChunkWidth || z < 0 || z >= WorldConstants.ChunkDepth || y < 0 ||
                y >= WorldConstants.Height)
            {
                return false;
            }

            var toCheck = new LocalVoxelCoordinates(x, y, z);

            if (chunk.GetBlockID(toCheck).Equals(block))
            {
                if (meta != 0x0 && chunk.GetMetadata(toCheck) != meta)
                {
                    return false;
                }

                return true;
            }
        }

        return false;
    }

    public static void GenerateColumn(
        IChunk chunk,
        LocalVoxelCoordinates location,
        int height,
        byte block,
        byte meta = 0x0
    )
    {
        for (var offset = 0; offset < height; offset++)
        {
            if (location.Y + offset >= WorldConstants.Height)
            {
                return;
            }

            var blockLocation = new LocalVoxelCoordinates(location.X, location.Y + offset, location.Z);
            chunk.SetBlockID(blockLocation, block);
            chunk.SetMetadata(blockLocation, meta);
        }
    }

    /*
     * Cuboid Modes
     * 0x0 - Solid cuboid of the specified block
     * 0x1 - Hollow cuboid of the specified block
     * 0x2 - Outlines the area of the cuboid using the specified block
     */
    protected static void GenerateCuboid(
        IChunk chunk,
        LocalVoxelCoordinates location,
        Vector3 size,
        byte block,
        byte meta = 0x0,
        byte mode = 0x0
    )
    {
        //If mode is 0x2 offset the size by 2 and change mode to 0x1
        if (mode.Equals(0x2))
        {
            size += new Vector3(2, 2, 2);
            mode = 0x1;
        }

        for (var w = location.X; w < location.X + size.X; w++)
        for (var l = location.Z; l < location.Z + size.Z; l++)
        for (var h = location.Y; h < location.Y + size.Y; h++)
        {
            if (w < 0 || w >= WorldConstants.ChunkWidth || l < 0 || l >= WorldConstants.ChunkDepth || h < 0 ||
                h >= WorldConstants.Height)
            {
                continue;
            }

            var BlockLocation = new LocalVoxelCoordinates(w, h, l);

            if (!h.Equals(location.Y) && !h.Equals(location.Y + (int) size.Y - 1)
                                      && !IsCuboidWall(new LocalVoxelCoordinates(w, 0, l), location, size)
                                      && !IsCuboidCorner(new LocalVoxelCoordinates(w, 0, l), location, size))
            {
                continue;
            }

            chunk.SetBlockID(BlockLocation, block);

            if (meta != 0x0)
            {
                chunk.SetMetadata(BlockLocation, meta);
            }
        }
    }

    protected static void GenerateVanillaLeaves(
        IChunk chunk,
        LocalVoxelCoordinates location,
        int radius,
        byte block,
        byte meta = 0x0
    )
    {
        var radiusOffset = radius;

        for (var yOffset = -radius; yOffset <= radius; yOffset = yOffset + 1)
        {
            var y = location.Y + yOffset;

            if (y > WorldConstants.Height)
            {
                continue;
            }

            GenerateVanillaCircle(
                chunk,
                new LocalVoxelCoordinates(location.X, y, location.Z),
                radiusOffset,
                block,
                meta
            );

            if (yOffset != -radius && yOffset % 2 == 0)
            {
                radiusOffset--;
            }
        }
    }

    protected static void GenerateVanillaCircle(
        IChunk chunk,
        LocalVoxelCoordinates location,
        int radius,
        byte block,
        byte meta = 0x0,
        double corner = 0
    )
    {
        for (var i = -radius; i <= radius; i = i + 1)
        for (var j = -radius; j <= radius; j = j + 1)
        {
            var max = (int) Math.Sqrt((i * i) + (j * j));

            if (max <= radius)
            {
                if ((i.Equals(-radius) && j.Equals(-radius))
                    || (i.Equals(-radius) && j.Equals(radius))
                    || (i.Equals(radius) && j.Equals(-radius))
                    || (i.Equals(radius) && j.Equals(radius)))
                {
                    if (corner + (radius * 0.2) < 0.4 || corner + (radius * 0.2) > 0.7 || corner.Equals(0))
                    {
                        continue;
                    }
                }

                var x = location.X + i;
                var z = location.Z + j;
                var currentBlock = new LocalVoxelCoordinates(x, location.Y, z);

                if (chunk.GetBlockID(currentBlock).Equals(0))
                {
                    chunk.SetBlockID(currentBlock, block);
                    chunk.SetMetadata(currentBlock, meta);
                }
            }
        }
    }

    protected static void GenerateCircle(IChunk chunk, LocalVoxelCoordinates location, int radius, byte block, byte meta = 0x0)
    {
        for (var i = -radius; i <= radius; i = i + 1)
        for (var j = -radius; j <= radius; j = j + 1)
        {
            var max = (int) Math.Sqrt((i * i) + (j * j));

            if (max <= radius)
            {
                var x = location.X + i;
                var z = location.Z + j;

                if (x < 0 || x >= WorldConstants.ChunkWidth || z < 0 || z >= WorldConstants.ChunkDepth)
                {
                    continue;
                }

                var currentBlock = new LocalVoxelCoordinates(x, location.Y, z);

                if (chunk.GetBlockID(currentBlock).Equals(0))
                {
                    chunk.SetBlockID(currentBlock, block);
                    chunk.SetMetadata(currentBlock, meta);
                }
            }
        }
    }

    protected static void GenerateSphere(
        IChunk chunk,
        LocalVoxelCoordinates location,
        int radius,
        byte block,
        byte meta = 0x0
    )
    {
        for (var i = -radius; i <= radius; i = i + 1)
        for (var j = -radius; j <= radius; j = j + 1)
        for (var k = -radius; k <= radius; k = k + 1)
        {
            var max = (int) Math.Sqrt((i * i) + (j * j) + (k * k));

            if (max <= radius)
            {
                var x = location.X + i;
                var y = location.Y + k;
                var z = location.Z + j;

                if (x < 0 || x >= WorldConstants.ChunkWidth || z < 0 || z >= WorldConstants.ChunkDepth || y < 0 ||
                    y >= WorldConstants.Height)
                {
                    continue;
                }

                var currentBlock = new LocalVoxelCoordinates(x, y, z);

                if (chunk.GetBlockID(currentBlock).Equals(0))
                {
                    chunk.SetBlockID(currentBlock, block);
                    chunk.SetMetadata(currentBlock, meta);
                }
            }
        }
    }
}