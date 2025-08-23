using System;

namespace TrueCraft.Core.Logic.Blocks;

public abstract class MushroomBlock : BlockProvider
{
    public override double BlastResistance => 0;

    public override double Hardness => 0;

    public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;
}

public class BrownMushroomBlock : MushroomBlock
{
    public static readonly byte BlockID = 0x27;

    public override byte ID => 0x27;

    public override byte Luminance => 1;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Brown Mushroom";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(13, 1);
}

public class RedMushroomBlock : MushroomBlock
{
    public static readonly byte BlockID = 0x28;

    public override byte ID => 0x28;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Red Mushroom";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(12, 1);
}