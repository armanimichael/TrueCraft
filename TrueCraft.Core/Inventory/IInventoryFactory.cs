using TrueCraft.Core.Logic;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Inventory;

public interface IInventoryFactory<T>
    where T : ISlot
{
    IWindow<T> NewInventoryWindow(
        IItemRepository itemRepository,
        ICraftingRepository craftingRepository,
        ISlotFactory<T> slotFactory,
        ISlots<T> mainInventory,
        ISlots<T> hotBar
    );

    ICraftingBenchWindow<T> NewCraftingBenchWindow(
        IItemRepository itemRepository,
        ICraftingRepository craftingRepository,
        ISlotFactory<T> slotFactory,
        sbyte windowID,
        ISlots<T> mainInventory,
        ISlots<T> hotBar,
        string name,
        int width,
        int height
    );

    IChestWindow<T> NewChestWindow(
        IItemRepository itemRepository,
        ISlotFactory<T> slotFactory,
        sbyte windowID,
        ISlots<T> mainInventory,
        ISlots<T> hotBar,
        IDimension dimension,
        GlobalVoxelCoordinates location,
        GlobalVoxelCoordinates? otherHalf
    );

    IFurnaceWindow<T> NewFurnaceWindow(
        IServiceLocator serviceLocator,
        ISlotFactory<T> slotFactory,
        sbyte windowID,
        IFurnaceSlots furnaceSlots,
        ISlots<T> mainInventory,
        ISlots<T> hotBar,
        IDimension dimension,
        GlobalVoxelCoordinates location
    );
}