using System;

namespace TrueCraft.Core.Logic.Blocks;

public class BookshelfBlock : BlockProvider, IBurnableItem
{
    public static readonly byte BlockID = 0x2F;

    public override byte ID => 0x2F;

    public override double BlastResistance => 7.5;

    public override double Hardness => 1.5;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Bookshelf";

    public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override bool Flammable => true;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(3, 2);
}