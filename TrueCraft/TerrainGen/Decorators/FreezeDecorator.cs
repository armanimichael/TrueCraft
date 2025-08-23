using System.Linq;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.World;
using TrueCraft.World;

namespace TrueCraft.TerrainGen.Decorators;

internal class FreezeDecorator : IChunkDecorator
{
    public void Decorate(int seed, IChunk chunk, IBlockRepository _, IBiomeRepository biomes)
    {
        for (var x = 0; x < Chunk.Width; x++)
        for (var z = 0; z < Chunk.Depth; z++)
        {
            var biome = biomes.GetBiome(chunk.GetBiome(x, z));

            if (biome.Temperature < 0.15)
            {
                var height = chunk.GetHeight(x, z);

                for (var y = height; y < Chunk.Height; y++)
                {
                    var location = new LocalVoxelCoordinates(x, y, z);

                    if (chunk.GetBlockID(location).Equals(StationaryWaterBlock.BlockID) ||
                        chunk.GetBlockID(location).Equals(WaterBlock.BlockID))
                    {
                        chunk.SetBlockID(location, IceBlock.BlockID);
                    }
                    else
                    {
                        var below = chunk.GetBlockID(location);

                        byte[] whitelist =
                        {
                            DirtBlock.BlockID,
                            GrassBlock.BlockID,
                            IceBlock.BlockID,
                            LeavesBlock.BlockID
                        };

                        if (y == height && whitelist.Any(w => w == below))
                        {
                            if (chunk.GetBlockID(location).Equals(IceBlock.BlockID) &&
                                CoverIce(chunk, biomes, location))
                            {
                                chunk.SetBlockID(
                                    new LocalVoxelCoordinates(location.X, location.Y + 1, location.Z),
                                    SnowfallBlock.BlockID
                                );
                            }
                            else if (!chunk.GetBlockID(location).Equals(SnowfallBlock.BlockID) &&
                                     !chunk.GetBlockID(location).Equals(AirBlock.BlockID))
                            {
                                chunk.SetBlockID(
                                    new LocalVoxelCoordinates(location.X, location.Y + 1, location.Z),
                                    SnowfallBlock.BlockID
                                );
                            }
                        }
                    }
                }
            }
        }
    }

    private static bool CoverIce(IChunk chunk, IBiomeRepository biomes, LocalVoxelCoordinates location)
    {
        const int maxDistance = 4;

        var nearby = new Vector3i[]
                     {
                         maxDistance * Vector3i.West,
                         maxDistance * Vector3i.East,
                         maxDistance * Vector3i.South,
                         maxDistance * Vector3i.North
                     };

        for (var i = 0; i < nearby.Length; i++)
        {
            var checkX = location.X + nearby[i].X;
            var checkZ = location.Z + nearby[i].Z;

            // TODO: does the order of the nearby array produce peculiar direction-dependent variations
            //       in snow cover near chunk boundaries?
            if (checkX < 0 || checkX >= Chunk.Width || checkZ < 0 || checkZ >= Chunk.Depth)
            {
                return false;
            }

            var check = new LocalVoxelCoordinates(checkX, location.Y, checkZ);
            var biome = biomes.GetBiome(chunk.GetBiome(checkX, checkZ));

            if (chunk.GetBlockID(check).Equals(biome.SurfaceBlock) || chunk.GetBlockID(check).Equals(biome.FillerBlock))
            {
                return true;
            }
        }

        return false;
    }
}