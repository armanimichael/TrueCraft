using System;

namespace TrueCraft.Core.Logic.Blocks;

public class SandstoneBlock : BlockProvider
{
    public static readonly byte BlockID = 0x18;

    public override byte ID => 0x18;

    public override double BlastResistance => 4;

    public override double Hardness => 0.8;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Sandstone";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(0, 12);
}