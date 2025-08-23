using System;
using TrueCraft.Core.World;
using TrueCraft.Core.Networking;

namespace TrueCraft.Core.Logic.Blocks;

public class JackoLanternBlock : BlockProvider
{
    public static readonly byte BlockID = 0x5B;

    public override byte ID => 0x5B;

    public override double BlastResistance => 5;

    public override double Hardness => 1;

    public override byte Luminance => 15;

    public override bool Opaque => false;

    public override byte LightOpacity => 255;

    public override string GetDisplayName(short metadata) => "Jack 'o' Lantern";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(6, 6);

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    ) => dimension.SetMetadata(descriptor.Coordinates, (byte) MathHelper.DirectionByRotationFlat(user.Entity!.Yaw, true));
}