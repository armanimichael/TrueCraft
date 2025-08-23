using TrueCraft.Core;
using TrueCraft.Core.World;

namespace TrueCraft.TerrainGen.Biomes;

public class SwamplandBiome : BiomeProvider
{
    public override byte ID => (byte) Biome.Swampland;

    public override double Temperature => 0.8f;

    public override double Rainfall => 0.9f;

    public override TreeSpecies[] Trees => new TreeSpecies[0];

    public override PlantSpecies[] Plants
    {
        get { return new[] { PlantSpecies.SugarCane, PlantSpecies.TallGrass }; }
    }
}