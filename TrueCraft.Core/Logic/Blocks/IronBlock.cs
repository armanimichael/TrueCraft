using System;

namespace TrueCraft.Core.Logic.Blocks;

public class IronBlock : BlockProvider
{
    public static readonly byte BlockID = 0x2A;

    public override byte ID => 0x2A;

    public override double BlastResistance => 30;

    public override double Hardness => 5;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Block of Iron";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(6, 1);

    public override ToolMaterial EffectiveToolMaterials =>
        ToolMaterial.Stone | ToolMaterial.Iron | ToolMaterial.Diamond;

    public override ToolType EffectiveTools => ToolType.Pickaxe;
}