using System;

namespace TrueCraft.Core.Logic.Blocks;

public class JukeboxBlock : BlockProvider, IBurnableItem
{
    public static readonly byte BlockID = 0x54;

    public override byte ID => 0x54;

    public override double BlastResistance => 30;

    public override double Hardness => 2;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Jukebox";

    public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(10, 4);
}