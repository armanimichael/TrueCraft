using System;

namespace TrueCraft.Core.Logic.Blocks;

public class SpongeBlock : BlockProvider
{
    public static readonly byte BlockID = 0x13;

    public override byte ID => 0x13;

    public override double BlastResistance => 3;

    public override double Hardness => 0.6;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Sponge";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(0, 3);
}