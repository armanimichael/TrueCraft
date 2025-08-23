using System;
using TrueCraft.Core.Logic.Items;
using TrueCraft.Core.Networking.Packets;
using fNbt;
using TrueCraft.Core.Networking;
using TrueCraft.Core.World;
using TrueCraft.Core.Server;

namespace TrueCraft.Core.Logic.Blocks;

public class WallSignBlock : BlockProvider
{
    public static readonly byte BlockID = 0x44;

    public override byte ID => 0x44;

    public override double BlastResistance => 5;

    public override double Hardness => 1;

    public override byte Luminance => 0;

    public override bool Opaque => true; // This is weird. You can stack signs on signs in Minecraft.

    public override string GetDisplayName(short metadata) => "Sign";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override BoundingBox? BoundingBox => null;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(4, 0);

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    ) => dimension.SetMetadata(descriptor.Coordinates, (byte) MathHelper.DirectionByRotationFlat(user.Entity!.Yaw, true));

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new[] { new ItemStack(SignItem.ItemID) };

    public override void BlockMined(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        ((IDimensionServer) dimension).SetTileEntity(descriptor.Coordinates, null);
        base.BlockMined(descriptor, face, dimension, user);
    }

    public override void TileEntityLoadedForClient(
        BlockDescriptor descriptor,
        IDimension dimension,
        NbtCompound entity,
        IRemoteClient client
    ) => client.QueuePacket(
        new UpdateSignPacket
        {
            X = descriptor.Coordinates.X,
            Y = (short) descriptor.Coordinates.Y,
            Z = descriptor.Coordinates.Z,
            Text = new[]
                   {
                       entity["Text1"].StringValue,
                       entity["Text2"].StringValue,
                       entity["Text3"].StringValue,
                       entity["Text4"].StringValue
                   }
        }
    );
}