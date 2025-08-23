using System;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class StoneBlock : BlockProvider
{
    public static readonly byte BlockID = 0x01;

    public override byte ID => 0x01;

    public override double BlastResistance => 30;

    public override double Hardness => 1.5;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Stone";

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(1, 0);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
    {
        var provider = ItemRepository.Get().GetItemProvider(item.ID);

        if (provider is PickaxeItem)
        {
            return new[] { new ItemStack(CobblestoneBlock.BlockID, 1, descriptor.Metadata) };
        }
        else
        {
            return new ItemStack[0];
        }
    }
}