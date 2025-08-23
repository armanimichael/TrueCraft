using System;

namespace TrueCraft.Core.Logic.Blocks;

public class MonsterSpawnerBlock : BlockProvider
{
    public static readonly byte BlockID = 0x34;

    public override byte ID => 0x34;

    public override double BlastResistance => 25;

    public override double Hardness => 5;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Monster Spawner";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(1, 4);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new ItemStack[0];
}