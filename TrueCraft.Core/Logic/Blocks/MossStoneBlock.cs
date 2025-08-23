using System;

namespace TrueCraft.Core.Logic.Blocks;

public class MossStoneBlock : BlockProvider
{
    public static readonly byte BlockID = 0x30;

    public override byte ID => 0x30;

    public override double BlastResistance => 30;

    public override double Hardness => 2;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Moss Stone";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(4, 2);
}