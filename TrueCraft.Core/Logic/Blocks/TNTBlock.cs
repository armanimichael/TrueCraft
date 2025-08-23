using System;

namespace TrueCraft.Core.Logic.Blocks;

public class TNTBlock : BlockProvider
{
    public static readonly byte BlockID = 0x2E;

    public override byte ID => 0x2E;

    public override double BlastResistance => 0;

    public override double Hardness => 0;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "TNT";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(8, 0);
}