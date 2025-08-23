using System;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class ClayBlock : BlockProvider
{
    public static readonly byte BlockID = 0x52;

    public override byte ID => 0x52;

    public override double BlastResistance => 3;

    public override double Hardness => 0.6;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Clay";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Gravel;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(8, 4);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new[] { new ItemStack(ClayItem.ItemID, 4) };
}