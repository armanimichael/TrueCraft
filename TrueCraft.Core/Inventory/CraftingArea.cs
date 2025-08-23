using System.ComponentModel;
using TrueCraft.Core.Logic;

namespace TrueCraft.Core.Inventory;

public class CraftingArea<T> : Slots<T>, ICraftingArea<T>
    where T : ISlot
{
    public static readonly int CraftingOutput = 0;

    private ICraftingRepository _repository;

    public CraftingArea(
        IItemRepository itemRepository,
        ICraftingRepository repository,
        ISlotFactory<T> slotFactory,
        int width,
        int height
    )
        : base(itemRepository, slotFactory.GetSlots(itemRepository, (width * height) + 1), width)
    {
        _repository = repository;
        Height = height;

        for (int j = 1, jul = width * height; j <= jul; j++)
        {
            this[j].PropertyChanged += HandleSlotPropertyChanged;
        }
    }

    private void HandleSlotPropertyChanged(object? sender, PropertyChangedEventArgs e) => UpdateOutput();

    /// <inheritdoc />
    public int Height { get; }

    private void UpdateOutput()
    {
        var pattern = CraftingPattern.GetCraftingPattern(GetItemStacks());

        if (pattern is null)
        {
            base[0].Item = ItemStack.EmptyStack;

            return;
        }

        Recipe = _repository.GetRecipe(pattern);
        base[0].Item = Recipe?.Output ?? ItemStack.EmptyStack;
    }

    /// <inheritdoc />
    public ICraftingRecipe? Recipe { get; private set; }

    /// <inheritdoc />
    public ItemStack TakeOutput()
    {
        var rv = Recipe?.Output ?? ItemStack.EmptyStack;

        if (rv.Empty)
        {
            return rv;
        }

        base[0].Item = base[0].Item.GetReducedStack(rv.Count);
        RemoveItemsFromInput();
        UpdateOutput();

        return rv;
    }

    private void RemoveItemsFromInput()
    {
        var recipe = Recipe;

        if (recipe is null)
        {
            return;
        }

        // Locate area on crafting bench
        int x, y = 0;

        for (x = 0; x < Width; x++)
        {
            var found = false;

            for (y = 0; y < Height; y++)
            {
                if (TestRecipe(recipe, x, y))
                {
                    found = true;

                    break;
                }
            }

            if (found)
            {
                break;
            }
        }

        // Remove items
        for (var _x = 0; _x < recipe.Pattern.Width; _x++)
        for (var _y = 0; _y < recipe.Pattern.Height; _y++)
        {
            var idx = ((y + _y) * Width) + x + _x + 1;
            base[idx].Item = base[idx].Item.GetReducedStack(recipe.Pattern[_x, _y].Count);
        }
    }

    private bool TestRecipe(ICraftingRecipe recipe, int x, int y)
    {
        if (x + recipe.Pattern.Width > Width || y + recipe.Pattern.Height > Height)
        {
            return false;
        }

        for (var _x = 0; _x < recipe.Pattern.Width; _x++)
        for (var _y = 0; _y < recipe.Pattern.Height; _y++)
        {
            var supplied = GetItemStack(x + _x, y + _y);
            var required = recipe.Pattern[_x, _y];

            if (supplied.ID != required.ID || supplied.Count < required.Count ||
                required.Metadata != supplied.Metadata)
            {
                return false;
            }
        }

        return true;
    }

    public ItemStack GetItemStack(int x, int y) => this[(y * Width) + x + 1].Item;

    public ItemStack[,] GetItemStacks()
    {
        var rv = new ItemStack[Width, Height];

        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            rv[x, y] = GetItemStack(x, y);
        }

        return rv;
    }
}