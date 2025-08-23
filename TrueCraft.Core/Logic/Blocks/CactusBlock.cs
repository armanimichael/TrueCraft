using System;
using TrueCraft.Core.World;
using TrueCraft.Core.Entities;
using TrueCraft.Core.Server;
using TrueCraft.Core.Networking;

namespace TrueCraft.Core.Logic.Blocks;

public class CactusBlock : BlockProvider
{
    public static readonly int MinGrowthSeconds = 30;
    public static readonly int MaxGrowthSeconds = 60;
    public static readonly int MaxGrowHeight = 3;

    public static readonly byte BlockID = 0x51;

    public override byte ID => 0x51;

    public override double BlastResistance => 2;

    public override double Hardness => 0.4;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Cactus";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Cloth;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(6, 4);

    public static bool ValidCactusPosition(
        BlockDescriptor descriptor,
        IBlockRepository repository,
        IDimension dimension,
        bool checkNeighbor = true,
        bool checkSupport = true
    )
    {
        if (checkNeighbor)
        {
            var coords = descriptor.Coordinates;

            foreach (var neighbor in Vector3i.Neighbors4)
            {
                if (dimension.GetBlockID(coords + neighbor) != AirBlock.BlockID)
                {
                    return false;
                }
            }
        }

        if (checkSupport)
        {
            var supportingBlock =
                repository.GetBlockProvider(dimension.GetBlockID(descriptor.Coordinates + Vector3i.Down));

            if (supportingBlock.ID != BlockID && supportingBlock.ID != SandBlock.BlockID)
            {
                return false;
            }
        }

        return true;
    }

    private static void TryGrowth(IMultiplayerServer server, IChunk chunk, LocalVoxelCoordinates coords)
    {
        if (chunk.GetBlockID(coords) != BlockID)
        {
            return;
        }

        // Find current height of stalk
        var height = 0;

        for (var y = -MaxGrowHeight; y <= MaxGrowHeight; y++)
        {
            if (chunk.GetBlockID(coords + (Vector3i.Down * y)) == BlockID)
            {
                height++;
            }
        }

        if (height < MaxGrowHeight)
        {
            var meta = chunk.GetMetadata(coords);
            meta++;
            chunk.SetMetadata(coords, meta);

            if (meta == 15)
            {
                if (chunk.GetBlockID(coords + Vector3i.Up) == 0)
                {
                    chunk.SetBlockID(coords + Vector3i.Up, BlockID);

                    server.Scheduler.ScheduleEvent(
                        "cactus",
                        chunk,
                        TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
                        (_server) => TryGrowth(_server, chunk, coords + Vector3i.Up)
                    );
                }
            }
            else
            {
                server.Scheduler.ScheduleEvent(
                    "cactus",
                    chunk,
                    TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
                    (_server) => TryGrowth(_server, chunk, coords)
                );
            }
        }
    }

    public static void DestroyCactus(BlockDescriptor descriptor, IMultiplayerServer server, IDimension dimension)
    {
        ServerOnly.Assert();

        var toDrop = 0;

        // Search upwards
        for (var y = descriptor.Coordinates.Y; y < 127; y++)
        {
            var coordinates = new GlobalVoxelCoordinates(descriptor.Coordinates.X, y, descriptor.Coordinates.Z);

            if (dimension.GetBlockID(coordinates) == BlockID)
            {
                dimension.SetBlockID(coordinates, AirBlock.BlockID);
                toDrop++;
            }
        }

        // Search downwards.
        for (var y = descriptor.Coordinates.Y - 1; y > 0; y--)
        {
            var coordinates = new GlobalVoxelCoordinates(descriptor.Coordinates.X, y, descriptor.Coordinates.Z);

            if (dimension.GetBlockID(coordinates) == BlockID)
            {
                dimension.SetBlockID(coordinates, AirBlock.BlockID);
                toDrop++;
            }
        }

        var manager = ((IDimensionServer) dimension).EntityManager;

        manager.SpawnEntity(
            new ItemEntity(
                dimension,
                manager,
                (Vector3) (descriptor.Coordinates + Vector3i.Up),
                new ItemStack(BlockID, (sbyte) toDrop)
            )
        );
    }

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        ServerOnly.Assert();

        if (ValidCactusPosition(descriptor, dimension.BlockRepository, dimension))
        {
            base.BlockPlaced(descriptor, face, dimension, user);
        }
        else
        {
            dimension.SetBlockID(descriptor.Coordinates, AirBlock.BlockID);

            var manager = ((IDimensionServer) dimension).EntityManager;

            manager.SpawnEntity(
                new ItemEntity(
                    dimension,
                    manager,
                    (Vector3) (descriptor.Coordinates + Vector3i.Up),
                    new ItemStack(BlockID, (sbyte) 1)
                )
            );
            // user.Inventory.PickUpStack() wasn't working?
        }

        var chunk = dimension.GetChunk(descriptor.Coordinates)!;

        user.Server.Scheduler.ScheduleEvent(
            "cactus",
            chunk,
            TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
            (server) => TryGrowth(server, chunk, (LocalVoxelCoordinates) descriptor.Coordinates)
        );
    }

    public override void BlockUpdate(
        BlockDescriptor descriptor,
        BlockDescriptor source,
        IMultiplayerServer server,
        IDimension dimension
    )
    {
        if (!ValidCactusPosition(descriptor, dimension.BlockRepository, dimension))
        {
            DestroyCactus(descriptor, server, dimension);
        }

        base.BlockUpdate(descriptor, source, server, dimension);
    }

    public override void BlockLoadedFromChunk(
        IMultiplayerServer server,
        IDimension dimension,
        GlobalVoxelCoordinates coordinates
    )
    {
        var chunk = dimension.GetChunk(coordinates)!;

        server.Scheduler.ScheduleEvent(
            "cactus",
            chunk,
            TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
            s => TryGrowth(s, chunk, (LocalVoxelCoordinates) coordinates)
        );
    }
}