using TrueCraft.Core;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.World;

namespace TrueCraft.TerrainGen.Biomes;

public class TundraBiome : BiomeProvider
{
    public override byte ID => (byte) Biome.Tundra;

    public override double Temperature => 0.1f;

    public override double Rainfall => 0.7f;

    public override TreeSpecies[] Trees
    {
        get { return new[] { TreeSpecies.Spruce }; }
    }

    public override PlantSpecies[] Plants => new PlantSpecies[0];

    public override double TreeDensity => 50;

    public override byte SurfaceBlock => GrassBlock.BlockID;

    public override byte FillerBlock => DirtBlock.BlockID;
}