using System;

namespace TrueCraft.Core.Logic.Blocks;

public class LapisLazuliBlock : BlockProvider
{
    public static readonly byte BlockID = 0x16;

    public override byte ID => 0x16;

    public override double BlastResistance => 15;

    public override double Hardness => 3;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Lapis Lazuli Block";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(0, 9);

    public override ToolMaterial EffectiveToolMaterials =>
        ToolMaterial.Stone | ToolMaterial.Iron | ToolMaterial.Diamond;

    public override ToolType EffectiveTools => ToolType.Pickaxe;
}