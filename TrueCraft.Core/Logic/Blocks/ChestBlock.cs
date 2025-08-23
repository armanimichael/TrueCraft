using System;
using TrueCraft.Core.World;
using fNbt;
using TrueCraft.Core.Entities;
using TrueCraft.Core.Networking;
using TrueCraft.Core.Server;
using TrueCraft.Core.Inventory;

namespace TrueCraft.Core.Logic.Blocks;

public class ChestBlock : BlockProvider, IBurnableItem
{
    public static readonly byte BlockId = 0x36;

    public override byte ID => 0x36;

    public override double BlastResistance => 12.5;

    public override double Hardness => 2.5;

    public override byte Luminance => 0;

    public override bool Opaque => false;

    public override string GetDisplayName(short metadata) => "Chest";

    public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(10, 1);

    private static readonly Vector3i[] AdjacentBlocks =
    {
        Vector3i.North,
        Vector3i.South,
        Vector3i.West,
        Vector3i.East
    };

    public override void ItemUsedOnBlock(
        GlobalVoxelCoordinates coordinates,
        ItemStack item,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        var adjacent = 0;
        var coords = coordinates + MathHelper.BlockFaceToCoordinates(face);
        GlobalVoxelCoordinates? t = null;

        // Check for adjacent chests. We can only allow one adjacent check block.
        for (var i = 0; i < AdjacentBlocks.Length; i++)
        {
            if (dimension.GetBlockID(coords + AdjacentBlocks[i]) == BlockId)
            {
                t = coords + AdjacentBlocks[i];
                adjacent++;
            }
        }

        if (adjacent <= 1)
        {
            if (t is not null)
                // Confirm that adjacent chest is not a double chest
            {
                for (var i = 0; i < AdjacentBlocks.Length; i++)
                {
                    if (dimension.GetBlockID(t + AdjacentBlocks[i]) == BlockId)
                    {
                        adjacent++;
                    }
                }
            }

            if (adjacent <= 1)
            {
                base.ItemUsedOnBlock(coordinates, item, face, dimension, user);
            }
        }
    }

    public override void BlockPlaced(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    ) => dimension.SetMetadata(descriptor.Coordinates, (byte) MathHelper.DirectionByRotationFlat(user.Entity!.Yaw, true));

    public override bool BlockRightClicked(
        IServiceLocator serviceLocator,
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        ServerOnly.Assert();

        GlobalVoxelCoordinates? adjacent = null; // No adjacent chest
        var self = descriptor.Coordinates;

        for (var i = 0; i < AdjacentBlocks.Length; i++)
        {
            var test = self + AdjacentBlocks[i];

            if (dimension.GetBlockID(test) == BlockId)
            {
                adjacent = test;
                var up = dimension.BlockRepository.GetBlockProvider(dimension.GetBlockID(test + Vector3i.Up));

                if (up.Opaque && !(up is WallSignBlock)) // Wall sign blocks are an exception
                {
                    return false; // Obstructed
                }

                break;
            }
        }

        var upSelf = dimension.BlockRepository.GetBlockProvider(dimension.GetBlockID(self + Vector3i.Up));

        if (upSelf.Opaque && !(upSelf is WallSignBlock))
        {
            return false; // Obstructed
        }

        if (adjacent is not null)
            // TODO LATER: this assumes that chests cannot be placed next to each other.
            // Ensure that chests are always opened in the same arrangement
        {
            if (adjacent.X < self.X ||
                adjacent.Z < self.Z)
            {
                var _ = adjacent;
                adjacent = self;
                self = _; // Swap
            }
        }

        var factory = new InventoryFactory<IServerSlot>();
        var slotFactory = SlotFactory<IServerSlot>.Get();
        var windowId = WindowIDs.GetWindowID();

        var window = factory.NewChestWindow(
            serviceLocator.ItemRepository,
            slotFactory,
            windowId,
            user.Inventory,
            user.Hotbar,
            dimension,
            descriptor.Coordinates,
            adjacent
        );

        user.OpenWindow(window);

        return false;
    }

    public override void BlockMined(
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        ServerOnly.Assert();

        var dimensionServer = (IDimensionServer) dimension;
        var self = descriptor.Coordinates;
        var entity = dimensionServer.GetTileEntity(self);
        var manager = ((IDimensionServer) dimension).EntityManager;

        if (entity is not null)
        {
            foreach (var item in (NbtList) entity["Items"])
            {
                var slot = ItemStack.FromNbt((NbtCompound) item);

                manager.SpawnEntity(
                    new ItemEntity(
                        dimension,
                        manager,
                        new Vector3(
                            descriptor.Coordinates.X + 0.5,
                            descriptor.Coordinates.Y + 0.5,
                            descriptor.Coordinates.Z + 0.5
                        ),
                        slot
                    )
                );
            }
        }

        dimensionServer.SetTileEntity(self, null);
        base.BlockMined(descriptor, face, dimension, user);
    }
}