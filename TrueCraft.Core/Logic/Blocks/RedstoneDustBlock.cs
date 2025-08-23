using System;
using TrueCraft.Core.Logic.Items;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Logic.Blocks;

public class RedstoneDustBlock : BlockProvider
{
    public static readonly byte BlockID = 0x37;

    public override byte ID => 0x37;

    public override double BlastResistance => 0;

    public override double Hardness => 0;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Redstone Dust";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(4, 10);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new[] { new ItemStack(RedstoneItem.ItemID, 1, descriptor.Metadata) };

    public override Vector3i GetSupportDirection(BlockDescriptor descriptor) => Vector3i.Down;
}