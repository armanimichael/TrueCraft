using System;

namespace TrueCraft.Core.Logic.Blocks;

public class GoldBlock : BlockProvider
{
    public static readonly byte BlockID = 0x29;

    public override byte ID => 0x29;

    public override double BlastResistance => 30;

    public override double Hardness => 3;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Block of Gold";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(7, 1);

    public override ToolMaterial EffectiveToolMaterials => ToolMaterial.Iron | ToolMaterial.Diamond;

    public override ToolType EffectiveTools => ToolType.Pickaxe;
}