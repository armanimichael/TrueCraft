﻿using TrueCraft.Core.World;
using TrueCraft.World;

namespace TrueCraft.TerrainGen;

public class EmptyGenerator : Generator
{
    public EmptyGenerator(int seed) : base(seed)
    {

    }

    public override IChunk GenerateChunk(GlobalChunkCoordinates coordinates)
    {
        return new Chunk(coordinates);
    }

    public override GlobalVoxelCoordinates GetSpawn(IDimension dimension)
    {
        return GlobalVoxelCoordinates.Zero;
    }
}