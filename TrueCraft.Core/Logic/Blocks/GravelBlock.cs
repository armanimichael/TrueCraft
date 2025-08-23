using System;
using TrueCraft.Core.Logic.Items;
using TrueCraft.Core.Networking;
using TrueCraft.Core.World;
using TrueCraft.Core.Server;
using TrueCraft.Core.Entities;

namespace TrueCraft.Core.Logic.Blocks;

public class GravelBlock : BlockProvider
{
    public static readonly byte BlockID = 0x0D;

    public override byte ID => 0x0D;

    public override double BlastResistance => 3;

    public override double Hardness => 0.6;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Gravel";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Gravel;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(3, 1);

    protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
    {
        //Gravel has a 10% chance of dropping flint.
        if (MathHelper.Random.Next(10) == 0)
        {
            return new[] { new ItemStack((short) ItemIDs.Flint, 1, descriptor.Metadata) };
        }
        else
            // TODO: shouldn't this be a gravel block item???
        {
            return new ItemStack[0];
        }
    }

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    ) => BlockUpdate(descriptor, descriptor, user.Server, dimension);

    public override void BlockUpdate(
        BlockDescriptor descriptor,
        BlockDescriptor source,
        IMultiplayerServer server,
        IDimension dimension
    )
    {
        ServerOnly.Assert();

        if (dimension.GetBlockID(descriptor.Coordinates + Vector3i.Down) == AirBlock.BlockID)
        {
            dimension.SetBlockID(descriptor.Coordinates, AirBlock.BlockID);
            var entityManager = ((IDimensionServer) dimension).EntityManager;

            entityManager.SpawnEntity(
                new FallingGravelEntity(dimension, entityManager, (Vector3) descriptor.Coordinates)
            );
        }
    }
}