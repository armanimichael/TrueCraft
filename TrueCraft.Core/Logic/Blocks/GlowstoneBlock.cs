using System;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks;

public class GlowstoneBlock : BlockProvider
{
    public static readonly byte BlockID = 0x59;

    public override byte ID => 0x59;

    public override double BlastResistance => 1.5;

    public override double Hardness => 0.3;

    public override byte Luminance => 15;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Glowstone";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Glass;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(9, 6);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new[]
                                                                                          {
                                                                                              new ItemStack(
                                                                                                  (short) ItemIDs.GlowstoneDust,
                                                                                                  (sbyte) new Random().Next(2, 4),
                                                                                                  descriptor.Metadata
                                                                                              )
                                                                                          };
}