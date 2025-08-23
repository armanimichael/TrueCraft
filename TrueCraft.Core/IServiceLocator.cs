using TrueCraft.Core.Logic;

namespace TrueCraft;

public interface IServiceLocator
{
    /// <summary>
    /// Gets the Block Repository.
    /// </summary>
    IBlockRepository BlockRepository { get; }

    /// <summary>
    /// Gets the Item Repository.
    /// </summary>
    IItemRepository ItemRepository { get; }

    /// <summary>
    /// Gets the Crafting Repository
    /// </summary>
    ICraftingRepository CraftingRepository { get; }
}