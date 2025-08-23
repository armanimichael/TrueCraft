using System;

namespace TrueCraft.Core.Logic.Blocks;

public class WoodenPlanksBlock : BlockProvider, IBurnableItem
{
    public static readonly byte BlockID = 0x05;

    public override byte ID => 0x05;

    public override double BlastResistance => 15;

    public override double Hardness => 2;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Wooden Planks";

    public override bool Flammable => true;

    public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(4, 0);
}