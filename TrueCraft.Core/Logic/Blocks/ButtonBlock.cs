namespace TrueCraft.Core.Logic.Blocks;

public class ButtonBlock : BlockProvider
{
    public static readonly byte BlockID = 0x4D;

    public override byte ID => 0x4D;

    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Button";
}