using System;

namespace TrueCraft.Core.Logic.Blocks;

public class GlassBlock : BlockProvider
{
    public static readonly byte BlockID = 0x14;

    public override byte ID => 0x14;

    public override double BlastResistance => 1.5;

    public override double Hardness => 0.3;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Glass";

    public override byte LightOpacity => 0;

    public override SoundEffectClass SoundEffect => SoundEffectClass.Glass;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(1, 3);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new ItemStack[0];
}