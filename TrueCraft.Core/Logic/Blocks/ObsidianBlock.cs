using System;

namespace TrueCraft.Core.Logic.Blocks;

public class ObsidianBlock : BlockProvider
{
    public static readonly byte BlockID = 0x31;

    public override byte ID => 0x31;

    public override double BlastResistance => 6000;

    public override double Hardness => 10;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Obsidian";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(5, 2);

    public override ToolMaterial EffectiveToolMaterials => ToolMaterial.Diamond;

    public override ToolType EffectiveTools => ToolType.Pickaxe;
}