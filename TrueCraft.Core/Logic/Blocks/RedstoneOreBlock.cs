using System;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class RedstoneOreBlock : BlockProvider, ISmeltableItem
{
    public static readonly byte BlockID = 0x49;

    public override byte ID => 0x49;

    public override double BlastResistance => 15;

    public override double Hardness => 3;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Redstone Ore";

    public ItemStack SmeltingOutput => new(RedstoneItem.ItemID);

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(3, 3);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        => new[] { new ItemStack(RedstoneItem.ItemID, (sbyte) new Random().Next(4, 5), descriptor.Metadata) };
}

public class GlowingRedstoneOreBlock : RedstoneOreBlock
{
    public new static readonly byte BlockID = 0x4A;

    public override byte ID => 0x4A;

    public override byte Luminance => 9;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Redstone Ore (glowing)";
}