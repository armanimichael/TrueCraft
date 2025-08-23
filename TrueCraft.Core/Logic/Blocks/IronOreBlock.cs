using System;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class IronOreBlock : BlockProvider, ISmeltableItem
{
    public static readonly byte BlockID = 0x0F;

    public override byte ID => 0x0F;

    public override double BlastResistance => 15;

    public override double Hardness => 3;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Iron Ore";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(1, 2);

    public override ToolMaterial EffectiveToolMaterials =>
        ToolMaterial.Stone | ToolMaterial.Iron | ToolMaterial.Diamond;

    public override ToolType EffectiveTools => ToolType.Pickaxe;

    public ItemStack SmeltingOutput => new((short) ItemIDs.IronIngot);
}