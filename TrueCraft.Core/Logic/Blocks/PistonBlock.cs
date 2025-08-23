using System;
using TrueCraft.Core.Networking;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Logic.Blocks;

public class PistonBlock : BlockProvider
{
    public static readonly byte BlockID = 0x21;

    public override byte ID => 0x21;

    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Piston";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(11, 6);

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    ) => dimension.SetMetadata(
        descriptor.Coordinates,
        (byte) MathHelper.DirectionByRotation(
            user.Entity!.Position,
            user.Entity.Yaw,
            descriptor.Coordinates,
            true
        )
    );
}

public class StickyPistonBlock : BlockProvider
{
    public static readonly byte BlockID = 0x1D;

    public override byte ID => 0x1D;

    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Sticky Piston";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(10, 6);

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    ) => dimension.SetMetadata(
        descriptor.Coordinates,
        (byte) MathHelper.DirectionByRotation(
            user.Entity!.Position,
            user.Entity.Yaw,
            descriptor.Coordinates,
            true
        )
    );
}

public class PistonPlungerBlock : BlockProvider
{
    public static readonly byte BlockID = 0x22;

    public override byte ID => 0x22;

    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Piston Plunger";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(11, 6);
}

public class PistonPlaceholderBlock : BlockProvider
{
    public static readonly byte BlockID = 0x24;

    public override byte ID => 0x24;

    public override double BlastResistance => 0;

    public override double Hardness => 0;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Piston Placeholder";
}