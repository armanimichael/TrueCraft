using System;

namespace TrueCraft.Core.Logic.Blocks;

public class RedstoneTorchBlock : TorchBlock
{
    public new static readonly byte BlockID = 0x4C;

    public override byte ID => 0x4C;

    public override double BlastResistance => 0;

    public override double Hardness => 0;

    public override byte Luminance => 7;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Redstone Torch";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(3, 6);
}

public class InactiveRedstoneTorchBlock : RedstoneTorchBlock
{
    public new static readonly byte BlockID = 0x4B;

    public override byte ID => 0x4B;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Redstone Torch (inactive)";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(3, 7);
}