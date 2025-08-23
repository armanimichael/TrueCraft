using TrueCraft.Core.Inventory;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Client.Inventory;

public class InventoryFactory : IInventoryFactory<ISlot>
{
    public IWindow<ISlot> NewInventoryWindow(
        IItemRepository itemRepository,
        ICraftingRepository craftingRepository,
        ISlotFactory<ISlot> slotFactory,
        ISlots<ISlot> mainInventory,
        ISlots<ISlot> hotBar
    ) => new InventoryWindow(itemRepository, craftingRepository, slotFactory, mainInventory, hotBar);

    public IChestWindow<ISlot> NewChestWindow(
        IItemRepository itemRepository,
        ISlotFactory<ISlot> slotFactory,
        sbyte windowID,
        ISlots<ISlot> mainInventory,
        ISlots<ISlot> hotBar,
        IDimension dimension,
        GlobalVoxelCoordinates location,
        GlobalVoxelCoordinates? otherHalf
    ) => new ChestWindow(
        itemRepository,
        slotFactory,
        windowID,
        mainInventory,
        hotBar,
        otherHalf is not null
    );

    public ICraftingBenchWindow<ISlot> NewCraftingBenchWindow(
        IItemRepository itemRepository,
        ICraftingRepository craftingRepository,
        ISlotFactory<ISlot> slotFactory,
        sbyte windowID,
        ISlots<ISlot> mainInventory,
        ISlots<ISlot> hotBar,
        string name,
        int width,
        int height
    ) => new CraftingBenchWindow(
        itemRepository,
        craftingRepository,
        slotFactory,
        windowID,
        mainInventory,
        hotBar,
        name,
        width,
        height
    );

    public IFurnaceWindow<ISlot> NewFurnaceWindow(
        IServiceLocator serviceLocator,
        ISlotFactory<ISlot> slotFactory,
        sbyte windowID,
        IFurnaceSlots furnaceSlots,
        ISlots<ISlot> mainInventory,
        ISlots<ISlot> hotBar,
        IDimension dimension,
        GlobalVoxelCoordinates location
    ) => new FurnaceWindow(
        serviceLocator.ItemRepository,
        slotFactory,
        windowID,
        mainInventory,
        hotBar
    );
}