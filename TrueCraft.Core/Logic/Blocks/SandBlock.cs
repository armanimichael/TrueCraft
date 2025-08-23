using System;
using TrueCraft.Core.Entities;
using TrueCraft.Core.Server;
using TrueCraft.Core.Networking;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Logic.Blocks;

public class SandBlock : BlockProvider
{
    public static readonly byte BlockID = 0x0C;

    public override byte ID => 0x0C;

    public override double BlastResistance => 2.5;

    public override double Hardness => 0.5;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Sand";

    public override SoundEffectClass SoundEffect => SoundEffectClass.Sand;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(2, 1);

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
            var entityManager = ((IDimensionServer) server).EntityManager;

            entityManager.SpawnEntity(
                new FallingSandEntity(dimension, entityManager, (Vector3) descriptor.Coordinates)
            );
        }
    }
}