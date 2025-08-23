using System;
using TrueCraft.Core.World;
using TrueCraft.Core.Networking;

namespace TrueCraft.Core.Logic.Blocks;

public class IceBlock : BlockProvider
{
    public static readonly byte BlockID = 0x4F;

    public override byte ID => 0x4F;

    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override byte LightOpacity => 2;

    public override string GetDisplayName(short metadata) => "Ice";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Glass;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(3, 4);

    public override void BlockMined(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        dimension.SetBlockID(descriptor.Coordinates, WaterBlock.BlockID);
        dimension.BlockRepository.GetBlockProvider(WaterBlock.BlockID).BlockPlaced(descriptor, face, dimension, user);
    }
}