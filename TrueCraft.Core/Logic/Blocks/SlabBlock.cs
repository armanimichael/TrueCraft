using System;

namespace TrueCraft.Core.Logic.Blocks;

public class SlabBlock : BlockProvider
{
    public enum SlabMaterial
    {
        Stone = 0x0,
        Standstone = 0x1,
        Wooden = 0x2,
        Cobblestone = 0x3
    }

    public static readonly byte BlockID = 0x2C;

    public override byte ID => 0x2C;

    public override double BlastResistance => 30;

    public override double Hardness => 2;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override byte LightOpacity => 255;

    public override string GetDisplayName(short metadata) => "Stone Slab";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood; // TODO: Deal with metadata god dammit

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(6, 0);
}

public class DoubleSlabBlock : SlabBlock
{
    public new static readonly byte BlockID = 0x2B;

    public override byte ID => 0x2B;

    public override double BlastResistance => 30;

    public override double Hardness => 2;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Double Stone Slab";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(6, 0);
}