using System;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class DiamondOreBlock : BlockProvider, ISmeltableItem
{
    public static readonly byte BlockID = 0x38;

    public override byte ID => 0x38;

    public override double BlastResistance => 15;

    public override double Hardness => 3;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Diamond Ore";

    public ItemStack SmeltingOutput => new((short) ItemIDs.Diamond);

    public override ToolMaterial EffectiveToolMaterials => ToolMaterial.Iron | ToolMaterial.Diamond;

    public override ToolType EffectiveTools => ToolType.Pickaxe;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(2, 3);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new[] { new ItemStack((short) ItemIDs.Diamond, 1, descriptor.Metadata) };
}