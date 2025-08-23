﻿using System;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.World;

namespace TrueCraft.TerrainGen.Decorations;

public class BalloonOakTree : Decoration
{
    private const int LeafRadius = 2;

    public override bool ValidLocation(LocalVoxelCoordinates location)
    {
        if (location.X - LeafRadius < 0
            || location.X + LeafRadius >= WorldConstants.ChunkWidth
            || location.Z - LeafRadius < 0
            || location.Z + LeafRadius >= WorldConstants.ChunkDepth
            || location.Y + LeafRadius >= WorldConstants.Height)
        {
            return false;
        }

        return true;
    }

    public override bool GenerateAt(int seed, IChunk chunk, LocalVoxelCoordinates location)
    {
        if (!ValidLocation(location))
        {
            return false;
        }

        var random = new Random(seed);
        var height = random.Next(4, 5);
        GenerateColumn(chunk, location, height, WoodBlock.BlockID, 0x0);
        var leafLocation = new LocalVoxelCoordinates(location.X, location.Y + height, location.Z);
        GenerateSphere(chunk, leafLocation, LeafRadius, LeavesBlock.BlockID, 0x0);

        return true;
    }
}