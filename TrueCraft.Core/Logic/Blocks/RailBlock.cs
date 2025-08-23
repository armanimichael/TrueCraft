using System;

namespace TrueCraft.Core.Logic.Blocks;

public class RailBlock : BlockProvider
{
    public static readonly byte BlockID = 0x42;

    public override byte ID => 0x42;

    public override double BlastResistance => 3.5;

    public override double Hardness => 0.7;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Rail";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(0, 8);
}

public class PoweredRailBlock : RailBlock
{
    public new static readonly byte BlockID = 0x1B;

    public override byte ID => 0x1B;

    public override string GetDisplayName(short metadata) => "Powered Rail";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(3, 11);
}

public class DetectorRailBlock : RailBlock
{
    public new static readonly byte BlockID = 0x1C;

    public override byte ID => 0x1C;

    public override string GetDisplayName(short metadata) => "Detector Rail";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(3, 12);
}