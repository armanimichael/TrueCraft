using System;
using TrueCraft.Core.World;
using TrueCraft.Profiling;

namespace TrueCraft.Core.Lighting;

public class OverWorldLighter : Lighter
{
    public OverWorldLighter(IDimension dimension, ILightingQueue queue)
        : base(dimension, queue) { }

    /// <inheritdoc />
    protected override void DoInitialLightOperation(GlobalChunkCoordinates coordinates)
    {
        var chunk = _dimension.GetChunk(coordinates);

        if (chunk is null)
        {
            return;
        }

        Profiler.Start("lighting.overworld.initial");

        for (var x = 0; x < WorldConstants.ChunkWidth; x++)
        for (var z = 0; z < WorldConstants.ChunkDepth; z++)
        {
            byte skyLight = 15;

            for (var y = WorldConstants.Height - 1; y >= 0 && skyLight > 0; y--)
            {
                var blockCoordinates = new LocalVoxelCoordinates(x, y, z);
                var blockID = chunk.GetBlockID(blockCoordinates);
                var provider = _dimension.BlockRepository.GetBlockProvider(blockID);
                var opacity = provider?.LightOpacity ?? 0;

                if (skyLight >= opacity)
                {
                    skyLight -= opacity;
                }
                // TODO Add Sky Light operation at this block.
                else
                {
                    skyLight = 0;
                }

                chunk.SetSkyLight(blockCoordinates, skyLight);
            }
        }

        Profiler.Done();
    }

    /// <inheritdoc />
    protected override void DoAddSkyLightOperation(GlobalVoxelCoordinates seed, byte lightLevel)
    {
        // TODO: can we find a situation involving caves and chunk boundaries where this
        //       won't suffice?
        var chunk = _dimension.GetChunk(seed);

        do
        {
            FloodFill(seed, lightLevel, GetSkyLight, SetSkyLight);
            seed = new GlobalVoxelCoordinates(seed.X, seed.Y + 1, seed.Z);
        } while (seed.Y < chunk?.MaxHeight);
    }

    /// <inheritdoc />
    protected override void DoSubtractSkyLightOperation(GlobalVoxelCoordinates seed, byte lightLevel) => throw new NotImplementedException();

    /// <inheritdoc />
    protected override void DoBlockUpdateSkyLightOperation(GlobalVoxelCoordinates seed, byte lightLevel) => throw new NotImplementedException();

    /// <inheritdoc />
    protected override void DoAddBlockLightOperation(GlobalVoxelCoordinates seed, byte lightLevel) => FloodFill(seed, lightLevel, GetBlockLight, SetBlockLight);

    /// <inheritdoc />
    protected override void DoSubtractBlockLightOperation(GlobalVoxelCoordinates seed, byte lightLevel) => throw new NotImplementedException();

    /// <inheritdoc />
    protected override void DoBlockUpdateBlockLightOperation(GlobalVoxelCoordinates seed, byte lightLeve) => throw new NotImplementedException();
}