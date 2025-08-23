using System;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class CoalOreBlock : BlockProvider, ISmeltableItem
{
    public static readonly byte BlockID = 0x10;

    public override byte ID => 0x10;

    public override double BlastResistance => 15;

    public override double Hardness => 3;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Coal Ore";

    public ItemStack SmeltingOutput => new(CoalItem.ItemID);

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(2, 2);

    public override ToolType EffectiveTools => ToolType.Pickaxe;

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new[] { new ItemStack(CoalItem.ItemID, 1, descriptor.Metadata) };
}