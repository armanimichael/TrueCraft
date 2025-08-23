using System;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class CobwebBlock : BlockProvider
{
    public static readonly byte BlockID = 0x1E;

    public override byte ID => 0x1E;

    public override double BlastResistance => 20;

    public override double Hardness => 4;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override bool DiffuseSkyLight => true;

    public override ToolType EffectiveTools => ToolType.Sword;

    public override string GetDisplayName(short metadata) => "Cobweb";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(11, 0);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new[] { new ItemStack(StringItem.ItemID, 1, descriptor.Metadata) };
}