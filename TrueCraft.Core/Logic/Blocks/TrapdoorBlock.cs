using System;
using TrueCraft.Core.World;
using TrueCraft.Core.Networking;

namespace TrueCraft.Core.Logic.Blocks;

public class TrapdoorBlock : BlockProvider, IBurnableItem
{
    public enum TrapdoorDirection
    {
        West = 0x0,
        East = 0x1,
        South = 0x2,
        North = 0x3
    }

    [Flags]
    public enum TrapdoorFlags
    {
        Closed = 0x0,
        Open = 0x4
    }

    public static readonly byte BlockID = 0x60;

    public override byte ID => 0x60;

    public override double BlastResistance => 15;

    public override double Hardness => 3;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Trapdoor";

    public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(4, 5);

    public override void BlockLeftClicked(
        IServiceLocator serviceLocator,
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    ) => BlockRightClicked(serviceLocator, descriptor, face, dimension, user);

    public override bool BlockRightClicked(
        IServiceLocator serviceLocator,
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        // Flip bit back and forth between Open and Closed
        dimension.SetMetadata(descriptor.Coordinates, (byte) (descriptor.Metadata ^ (byte) TrapdoorFlags.Open));

        return false;
    }

    public override void ItemUsedOnBlock(
        GlobalVoxelCoordinates coordinates,
        ItemStack item,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        if (face == BlockFace.PositiveY || face == BlockFace.NegativeY)
            // Trapdoors are not placed when the user clicks on the top or bottom of a block
        {
            return;
        }

        // NOTE: These directions are rotated by 90 degrees so that the hinge of the trapdoor is placed
        // where the user had their cursor.
        switch (face)
        {
            case BlockFace.NegativeZ:
                item.Metadata = (byte) TrapdoorDirection.West;

                break;
            case BlockFace.PositiveZ:
                item.Metadata = (byte) TrapdoorDirection.East;

                break;
            case BlockFace.NegativeX:
                item.Metadata = (byte) TrapdoorDirection.South;

                break;
            case BlockFace.PositiveX:
                item.Metadata = (byte) TrapdoorDirection.North;

                break;
            default:
                return;
        }

        base.ItemUsedOnBlock(coordinates, item, face, dimension, user);
    }

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new[] { new ItemStack(ID) };
}