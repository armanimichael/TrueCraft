using System;

namespace TrueCraft.Core.Logic.Blocks;

public class SaplingBlock : BlockProvider, IBurnableItem
{
    public enum SaplingType
    {
        Oak = 0,
        Spruce = 1,
        Birch = 2
    }

    public static readonly byte BlockID = 0x06;

    public override byte ID => 0x06;

    public override double BlastResistance => 0;

    public override double Hardness => 0;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Sapling";

    public override BoundingBox? BoundingBox => null;

    public TimeSpan BurnTime => TimeSpan.FromSeconds(5);

    public override BoundingBox? InteractiveBoundingBox =>
        new BoundingBox(new Vector3(1 / 16.0, 0, 1 / 16.0), new Vector3(14 / 16.0));

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(15, 0);
}