using System;
using TrueCraft.Core.World;
using TrueCraft.Core.Networking;

namespace TrueCraft.Core.Logic.Blocks;

public class PumpkinBlock : BlockProvider
{
    public static readonly byte BlockID = 0x56;

    public override byte ID => 0x56;

    public override double BlastResistance => 5;

    public override double Hardness => 1;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Pumpkin";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(6, 6);

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    ) => dimension.SetMetadata(descriptor.Coordinates, (byte) MathHelper.DirectionByRotationFlat(user.Entity!.Yaw, true));
}