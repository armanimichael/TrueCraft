using System;

namespace TrueCraft.Core.Logic.Blocks;

public class NoteBlockBlock : BlockProvider, IBurnableItem
{
    public static readonly byte BlockID = 0x19;

    public override byte ID => 0x19;

    public override double BlastResistance => 4;

    public override double Hardness => 0.8;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Note Block";

    public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(10, 4);
}