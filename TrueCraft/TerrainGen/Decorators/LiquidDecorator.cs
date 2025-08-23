using System;
using TrueCraft.World;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.World;

namespace TrueCraft.TerrainGen.Decorators;

public class LiquidDecorator : IChunkDecorator
{
    public static readonly int WaterLevel = 40;

    public void Decorate(int seed, IChunk chunk, IBlockRepository _, IBiomeRepository biomes)
    {
        for (var x = 0; x < Chunk.Width; x++)
        for (var z = 0; z < Chunk.Depth; z++)
        {
            var biome = biomes.GetBiome(chunk.GetBiome(x, z));
            var height = chunk.GetHeight(x, z);

            for (var y = height; y <= WaterLevel; y++)
            {
                var blockLocation = new LocalVoxelCoordinates(x, y, z);
                int blockId = chunk.GetBlockID(blockLocation);

                if (blockId.Equals(AirBlock.BlockID))
                {
                    chunk.SetBlockID(blockLocation, biome.WaterBlock);
                    var below = new LocalVoxelCoordinates(blockLocation.X, blockLocation.Y - 1, blockLocation.Z);

                    if (!chunk.GetBlockID(below).Equals(AirBlock.BlockID) &&
                        !chunk.GetBlockID(below).Equals(biome.WaterBlock))
                    {
                        if (!biome.WaterBlock.Equals(LavaBlock.BlockID) &&
                            !biome.WaterBlock.Equals(StationaryLavaBlock.BlockID))
                        {
                            var random = new Random(seed);

                            if (random.Next(100) < 40)
                            {
                                chunk.SetBlockID(below, ClayBlock.BlockID);
                            }
                            else
                            {
                                chunk.SetBlockID(below, SandBlock.BlockID);
                            }
                        }
                    }
                }
            }

            for (var y = 4; y < height / 8; y++)
            {
                var blockLocation = new LocalVoxelCoordinates(x, y, z);
                int blockId = chunk.GetBlockID(blockLocation);

                if (blockId.Equals(AirBlock.BlockID))
                {
                    chunk.SetBlockID(blockLocation, LavaBlock.BlockID);
                }
            }
        }
    }
}