using System;

namespace TrueCraft.Core.Logic.Blocks;

public class DirtBlock : BlockProvider
{
    public static readonly byte BlockID = 0x03;

    public override byte ID => 0x03;

    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Dirt";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Gravel;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(2, 0);
}