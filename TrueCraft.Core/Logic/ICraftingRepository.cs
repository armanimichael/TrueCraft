namespace TrueCraft.Core.Logic;

public interface ICraftingRepository
{
    ICraftingRecipe? GetRecipe(CraftingPattern craftingArea);
}