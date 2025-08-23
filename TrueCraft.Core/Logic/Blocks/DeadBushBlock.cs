using System;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Logic.Blocks;

public class DeadBushBlock : BlockProvider
{
    public static readonly byte BlockID = 0x20;

    public override byte ID => 0x20;

    public override double BlastResistance => 0;

    public override double Hardness => 0;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Dead Bush";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

    public override BoundingBox? BoundingBox => null;

    public override BoundingBox? InteractiveBoundingBox => new BoundingBox(new Vector3(4 / 16.0), Vector3.One);

    public override Vector3i GetSupportDirection(BlockDescriptor descriptor) => Vector3i.Down;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(7, 3);
}