namespace TrueCraft.Core.Logic.Blocks;

public abstract class PressurePlateBlock : BlockProvider
{
    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override bool Opaque => false;
}

public class WoodenPressurePlateBlock : PressurePlateBlock
{
    public static readonly byte BlockID = 0x48;

    public override byte ID => 0x48;

    public override string GetDisplayName(short metadata) => "Wooden Pressure Plate";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;
}

public class StonePressurePlateBlock : PressurePlateBlock
{
    public static readonly byte BlockID = 0x46;

    public override byte ID => 0x46;

    public override string GetDisplayName(short metadata) => "Stone Pressure Plate";
}