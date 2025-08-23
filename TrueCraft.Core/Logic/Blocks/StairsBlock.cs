using System;
using TrueCraft.Core.Networking;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Logic.Blocks;

public abstract class StairsBlock : BlockProvider
{
    public enum StairDirection
    {
        East = 0,
        West = 1,
        South = 2,
        North = 3
    }

    public override double Hardness => 0;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override byte LightOpacity => 255;

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        byte meta = 0;

        switch (MathHelper.DirectionByRotationFlat(user.Entity!.Yaw))
        {
            case Direction.East:
                meta = (byte) StairDirection.East;

                break;
            case Direction.West:
                meta = (byte) StairDirection.West;

                break;
            case Direction.North:
                meta = (byte) StairDirection.North;

                break;
            case Direction.South:
                meta = (byte) StairDirection.South;

                break;
            default:
                meta = 0; // Should never happen

                break;
        }

        dimension.SetMetadata(descriptor.Coordinates, meta);
    }
}

public class WoodenStairsBlock : StairsBlock, IBurnableItem
{
    public static readonly byte BlockID = 0x35;

    public override byte ID => 0x35;

    public override double BlastResistance => 15;

    public override string GetDisplayName(short metadata) => "Wooden Stairs";

    public override bool Flammable => true;

    public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;
}

public class StoneStairsBlock : StairsBlock
{
    public static readonly byte BlockID = 0x43;

    public override byte ID => 0x43;

    public override double BlastResistance => 30;

    public override string GetDisplayName(short metadata) => "Stone Stairs";
}