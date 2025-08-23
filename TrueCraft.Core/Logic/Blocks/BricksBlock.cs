using System;

namespace TrueCraft.Core.Logic.Blocks;

public class BricksBlock : BlockProvider
{
    public static readonly byte BlockID = 0x2D;

    public override byte ID => 0x2D;

    public override double BlastResistance => 30;

    public override double Hardness => 2;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Bricks";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(7, 0);
}