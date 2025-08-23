using System;

namespace TrueCraft.Core.Logic.Blocks;

public class IronBlock : BlockProvider
{
    public static readonly byte BlockID = 0x2A;
        
    public override byte ID { get { return 0x2A; } }
        
    public override double BlastResistance { get { return 30; } }

    public override double Hardness { get { return 5; } }

    public override byte Luminance { get { return 0; } }
        
    public override string GetDisplayName(short metadata)
    {
        return "Block of Iron";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata)
    {
        return new Tuple<int, int>(6, 1);
    }

    public override ToolMaterial EffectiveToolMaterials
    {
        get
        {
            return ToolMaterial.Stone | ToolMaterial.Iron | ToolMaterial.Diamond;
        }
    }

    public override ToolType EffectiveTools
    {
        get
        {
            return ToolType.Pickaxe;
        }
    }
}
