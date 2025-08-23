using System;
using TrueCraft.Core.Logic.Items;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Logic.Blocks;

public class SnowBlock : BlockProvider
{
    public static readonly byte BlockID = 0x50;

    public override byte ID => 0x50;

    public override double BlastResistance => 1;

    public override double Hardness => 0.2;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Snow Block";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Snow;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(2, 4);
}

public class SnowfallBlock : BlockProvider
{
    public static readonly byte BlockID = 0x4E;

    public override byte ID => 0x4E;

    public override double BlastResistance => 0.5;

    public override double Hardness => 0.6;

    public override byte Luminance => 0;

    public override bool RenderOpaque => true;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Snow";

    public override BoundingBox? BoundingBox => null;

    public override SoundEffectClass SoundEffect => SoundEffectClass.Snow;

    public override BoundingBox? InteractiveBoundingBox =>
        // TODO: This is metadata-aware
        new BoundingBox(Vector3.Zero, new Vector3(1, 1 / 16.0, 1));

    public override Vector3i GetSupportDirection(BlockDescriptor descriptor) => Vector3i.Down;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(2, 4);

    public override ToolType EffectiveTools => ToolType.Shovel;

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item) => new[] { new ItemStack(SnowballItem.ItemID) };
}