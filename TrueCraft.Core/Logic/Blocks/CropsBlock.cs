using System;
using TrueCraft.Core.Logic.Items;
using TrueCraft.Core.Networking;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Logic.Blocks;

public class CropsBlock : BlockProvider
{
    public static readonly byte BlockID = 0x3B;

    public override byte ID => 0x3B;

    public override double BlastResistance => 0;

    public override double Hardness => 0;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Crops";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

    public override BoundingBox? BoundingBox => null;

    public override BoundingBox? InteractiveBoundingBox => new BoundingBox(Vector3.Zero, new Vector3(1, 3 / 16.0, 1));

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(8, 5);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
    {
        if (descriptor.Metadata >= 7)
        {
            return new[]
                   {
                       new ItemStack((short) ItemIDs.Wheat), new ItemStack(SeedsItem.ItemID, (sbyte) MathHelper.Random.Next(3))
                   };
        }
        else
        {
            return new[] { new ItemStack(SeedsItem.ItemID) };
        }
    }

    private static void GrowBlock(IMultiplayerServer server, IChunk chunk, LocalVoxelCoordinates coords)
    {
        if (chunk.GetBlockID(coords) != BlockID)
        {
            return;
        }

        var meta = chunk.GetMetadata(coords);
        meta++;
        chunk.SetMetadata(coords, meta);

        if (meta < 7)
        {
            server.Scheduler.ScheduleEvent(
                "crops",
                chunk,
                TimeSpan.FromSeconds(MathHelper.Random.Next(30, 60)),
                (_server) => GrowBlock(_server, chunk, coords)
            );
        }
    }

    public override void BlockUpdate(
        BlockDescriptor descriptor,
        BlockDescriptor source,
        IMultiplayerServer server,
        IDimension dimension
    )
    {
        if (dimension.GetBlockID(descriptor.Coordinates + Vector3i.Down) != FarmlandBlock.BlockID)
        {
            GenerateDropEntity(descriptor, dimension, server, ItemStack.EmptyStack);
            dimension.SetBlockID(descriptor.Coordinates, 0);
        }
    }

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        var coordinates = descriptor.Coordinates + MathHelper.BlockFaceToCoordinates(face);
        var chunk = dimension.GetChunk(coordinates)!;

        user.Server.Scheduler.ScheduleEvent(
            "crops",
            chunk,
            TimeSpan.FromSeconds(MathHelper.Random.Next(30, 60)),
            (server) => GrowBlock(server, chunk, (LocalVoxelCoordinates) coordinates)
        );
    }

    public override void BlockLoadedFromChunk(
        IMultiplayerServer server,
        IDimension dimension,
        GlobalVoxelCoordinates coordinates
    )
    {
        var chunk = dimension.GetChunk(coordinates)!;

        server.Scheduler.ScheduleEvent(
            "crops",
            chunk,
            TimeSpan.FromSeconds(MathHelper.Random.Next(30, 60)),
            (s) => GrowBlock(s, chunk, (LocalVoxelCoordinates) coordinates)
        );
    }
}