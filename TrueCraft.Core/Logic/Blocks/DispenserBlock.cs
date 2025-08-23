using System;
using TrueCraft.Core.Networking;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Logic.Blocks;

public class DispenserBlock : BlockProvider
{
    public static readonly byte BlockID = 0x17;

    public override byte ID => 0x17;

    public override double BlastResistance => 17.5;

    public override double Hardness => 3.5;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Dispenser";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(13, 2);

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    ) => dimension.SetMetadata(descriptor.Coordinates, (byte) MathHelper.DirectionByRotationFlat(user.Entity!.Yaw, true));
}