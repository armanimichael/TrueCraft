using System;

namespace TrueCraft.Core.Logic.Blocks;

public class LeverBlock : BlockProvider
{
    public static readonly byte BlockID = 0x45;

    public override byte ID => 0x45;

    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Lever";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(0, 6);
}