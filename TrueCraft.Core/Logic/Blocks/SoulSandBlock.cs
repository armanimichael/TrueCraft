using System;

namespace TrueCraft.Core.Logic.Blocks;

public class SoulSandBlock : BlockProvider
{
    public static readonly byte BlockID = 0x58;

    public override byte ID => 0x58;

    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Soul Sand";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Sand;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(8, 6);
}