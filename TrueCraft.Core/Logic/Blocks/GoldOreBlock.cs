using System;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class GoldOreBlock : BlockProvider, ISmeltableItem
{
    public static readonly byte BlockID = 0x0E;

    public override byte ID => 0x0E;

    public override double BlastResistance => 15;

    public override double Hardness => 3;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Gold Ore";

    public ItemStack SmeltingOutput => new((short) ItemIDs.GoldIngot);

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(0, 2);

    public override ToolMaterial EffectiveToolMaterials => ToolMaterial.Iron | ToolMaterial.Diamond;

    public override ToolType EffectiveTools => ToolType.Pickaxe;
}