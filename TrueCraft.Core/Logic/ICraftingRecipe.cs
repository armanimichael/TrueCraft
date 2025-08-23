using System;

namespace TrueCraft.Core.Logic;

public interface ICraftingRecipe : IEquatable<ICraftingRecipe>
{
    CraftingPattern Pattern { get; }

    ItemStack Output { get; }
}