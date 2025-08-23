using System;

namespace TrueCraft.Core.Logic.Blocks;

public class LockedChestBlock : BlockProvider, IBurnableItem
{
    public static readonly byte BlockID = 0x5F;

    public override byte ID => 0x5F;

    public override double BlastResistance => 0;

    public override double Hardness => 0;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Locked Chest";

    public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(10, 1);
}