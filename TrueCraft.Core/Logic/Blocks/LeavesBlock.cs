using System;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class LeavesBlock : BlockProvider
{
    public static readonly byte BlockID = 0x12;

    public override byte ID => 0x12;

    public override double BlastResistance => 1;

    public override double Hardness => 0.2;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override bool DiffuseSkyLight => true;

    public override byte LightOpacity => 2;

    public override string GetDisplayName(short metadata) => "Leaves";

    public override bool Flammable => true;

    public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(4, 3);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
    {
        var provider = ItemRepository.Get().GetItemProvider(item.ID);

        if (provider is ShearsItem)
        {
            return base.GetDrop(descriptor, item);
        }
        else
        {
            if (MathHelper.Random.Next(20) == 0) // 5% chance
            {
                return new[] { new ItemStack(SaplingBlock.BlockID, 1, descriptor.Metadata) };
            }
            else
            {
                return new ItemStack[0];
            }
        }
    }
}