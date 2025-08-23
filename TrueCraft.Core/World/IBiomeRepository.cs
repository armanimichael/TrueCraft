namespace TrueCraft.Core.World;

public interface IBiomeRepository
{
    IBiomeProvider GetBiome(Biome id);
    IBiomeProvider GetBiome(double temperature, double rainfall, bool spawn);
    void RegisterBiomeProvider(IBiomeProvider provider);
}