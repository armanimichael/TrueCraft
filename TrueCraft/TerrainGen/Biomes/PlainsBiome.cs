﻿using TrueCraft.Core;
using TrueCraft.Core.World;

namespace TrueCraft.TerrainGen.Biomes;

public class PlainsBiome : BiomeProvider
{
    public override byte ID
    {
        get { return (byte)Biome.Plains; }
    }

    public override double Temperature
    {
        get { return 0.8f; }
    }

    public override double Rainfall
    {
        get { return 0.4f; }
    }

    public override TreeSpecies[] Trees
    {
        get
        {
            return new[] { TreeSpecies.Oak };
        }
    }
}