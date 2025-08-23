using System;
using TrueCraft.Core.World;
using TrueCraft.Core.Entities;
using TrueCraft.Core.Networking;
using TrueCraft.Core.Inventory;
using TrueCraft.Core.Server;

namespace TrueCraft.Core.Logic.Blocks;

public class CraftingTableBlock : BlockProvider, IBurnableItem
{
    public static readonly byte BlockID = 0x3A;

    public override byte ID => 0x3A;

    public override double BlastResistance => 12.5;

    public override double Hardness => 2.5;

    public override byte Luminance => 0;

    public override string GetDisplayName(short metadata) => "Crafting Table";

    public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

    public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

    public override bool BlockRightClicked(
        IServiceLocator serviceLocator,
        BlockDescriptor descriptor,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        ServerOnly.Assert();

        var factory = new InventoryFactory<IServerSlot>();

        var window = factory.NewCraftingBenchWindow(
            serviceLocator.ItemRepository,
            serviceLocator.CraftingRepository,
            SlotFactory<IServerSlot>.Get(),
            WindowIDs.GetWindowID(),
            user.Inventory,
            user.Hotbar,
            "Crafting",
            3,
            3
        );

        user.OpenWindow(window);

        // TODO: this should be called in response to Close Window packet, not Disposed.
        window.WindowClosed += (sender, e) =>
        {
            // TODO BUG: this does not appear to be called (Items do not spawn, and remain in 2x2 (3x3?) Crafting Grid for next opening).
            var entityManager = ((IDimensionServer) dimension).EntityManager;
            var inputs = window.CraftingGrid.GetItemStacks();

            foreach (var item in inputs)
            {
                if (!item.Empty)
                {
                    IEntity entity = new ItemEntity(
                        dimension,
                        entityManager,
                        (Vector3) (descriptor.Coordinates + Vector3i.Up),
                        item
                    );

                    entityManager.SpawnEntity(entity);
                }
            }
        };

        return true;
    }

    public override Tuple<int, int> GetTextureMap(byte metadata) => new(11, 3);
}