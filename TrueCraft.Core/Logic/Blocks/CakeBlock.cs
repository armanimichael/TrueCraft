using System;
using TrueCraft.Core.World;
using TrueCraft.Core.Networking;

namespace TrueCraft.Core.Logic.Blocks;

public class CakeBlock : BlockProvider
{
    public static readonly byte BlockID = 0x5C;

    public override byte ID => 0x5C;

    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Cake";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Cloth;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(9, 7);

    public override bool BlockRightClicked(
        IServiceLocator serviceLocator,
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        if (descriptor.Metadata == 5)
        {
            dimension.SetBlockID(descriptor.Coordinates, AirBlock.BlockID);
        }
        else
        {
            dimension.SetMetadata(descriptor.Coordinates, (byte) (descriptor.Metadata + 1));
        }

        return false;
    }
}