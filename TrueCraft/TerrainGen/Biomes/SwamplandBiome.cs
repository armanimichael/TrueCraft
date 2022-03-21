﻿using System;

namespace TrueCraft.TerrainGen.Biomes
{
    public class SwamplandBiome : BiomeProvider
    {
        public override byte ID
        {
            get { return (byte)Biome.Swampland; }
        }

        public override double Temperature
        {
            get { return 0.8f; }
        }

        public override double Rainfall
        {
            get { return 0.9f; }
        }

        public override TreeSpecies[] Trees
        {
            get
            {
                return new TreeSpecies[0];
            }
        }

        public override PlantSpecies[] Plants
        {
            get
            {
                return new[] { PlantSpecies.SugarCane, PlantSpecies.TallGrass };
            }
        }
    }
}
