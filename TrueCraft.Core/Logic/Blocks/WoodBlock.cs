using System;
using System.Collections.Generic;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class WoodBlock : BlockProvider, IBurnableItem, ISmeltableItem
{
    public enum WoodType
    {
        Oak = 0,
        Spruce = 1,
        Birch = 2
    }

    public static readonly byte BlockID = 0x11;

    public override byte ID => 0x11;

    public override double BlastResistance => 10;

    public override double Hardness => 2;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Wood";

    public override bool Flammable => true;

    public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

    public ItemStack SmeltingOutput => new(CoalItem.ItemID, 1, (short) CoalItem.MetaData.Charcoal);

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(4, 1);

    public override IEnumerable<short> VisibleMetadata
    {
        get
        {
            yield return (short) WoodType.Oak;
            yield return (short) WoodType.Spruce;
            yield return (short) WoodType.Birch;
        }
    }
}