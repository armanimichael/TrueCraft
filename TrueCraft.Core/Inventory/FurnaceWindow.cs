using System.Collections.Generic;
using TrueCraft.Core.Logic;

namespace TrueCraft.Core.Inventory;

public abstract class FurnaceWindow<T> : Window<T>, IFurnaceWindow<T>
    where T : ISlot
{
    // NOTE: these values must match the order in which the slots
    //    collections are added in the constructors.
    public enum AreaIndices
    {
        Ingredient = 0,
        Fuel = 1,
        Output = 2,
        Main = 3,
        Hotbar = 4
    }

    private const int _outputSlotIndex = 2;

    public FurnaceWindow(
        IItemRepository itemRepository,
        ISlotFactory<T> slotFactory,
        sbyte windowID,
        ISlots<T> mainInventory,
        ISlots<T> hotBar
    )
        :
        base(
            itemRepository,
            windowID,
            Windows.WindowType.Furnace,
            "Furnace",
            new ISlots<T>[]
            {
                GetSlots(itemRepository, slotFactory),
                GetSlots(itemRepository, slotFactory),
                GetSlots(itemRepository, slotFactory),
                mainInventory, hotBar
            }
        )
    {
        IngredientSlotIndex = 0;
        FuelSlotIndex = 1;
        OutputSlotIndex = 2;
        MainSlotIndex = 3;
    }

    private static Slots<T> GetSlots(
        IItemRepository itemRepository,
        ISlotFactory<T> slotFactory
    )
    {
        var lst = new List<T>();
        lst.Add(slotFactory.GetSlot(itemRepository));

        return new Slots<T>(itemRepository, lst, 1);
    }

    public ISlots<T> Ingredient => Slots[(int) AreaIndices.Ingredient];

    /// <inheritdoc />
    public int IngredientSlotIndex { get; }

    public ISlots<T> Fuel => Slots[(int) AreaIndices.Fuel];

    /// <inheritdoc />
    public int FuelSlotIndex { get; }

    public ISlots<T> Output => Slots[(int) AreaIndices.Output];

    /// <inheritdoc />
    public int OutputSlotIndex { get; }

    public override bool IsOutputSlot(int slotIndex) => slotIndex == _outputSlotIndex;
}