using System;

namespace TrueCraft.Core.Logic.Blocks;

public class WoolBlock : BlockProvider
{
    public static readonly byte BlockID = 0x23;

    public override byte ID => 0x23;

    public override double BlastResistance => 4;

    public override double Hardness => 0.8;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Wool";

    public override bool Flammable => true;

    public override SoundEffectClass SoundEffect => SoundEffectClass.Cloth;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(0, 4);
}