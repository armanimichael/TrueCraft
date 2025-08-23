using System;
using System.Xml;

namespace TrueCraft.Core.Logic;

public class CraftingRecipe : ICraftingRecipe, IEquatable<ICraftingRecipe>
{
    private CraftingPattern _input;

    // TODO this needs to be immutable
    private ItemStack _output;

    public CraftingRecipe(XmlNode recipe)
    {
        var pattern = recipe.FirstChild;

        if (pattern is null)
        {
            throw new ArgumentException("The given recipe node has no children.");
        }

        _input = CraftingPattern.GetCraftingPattern(pattern)!;

        var output = pattern.NextSibling;

        if (output is null)
        {
            throw new ArgumentException("The given recipe has no output.");
        }

        _output = new ItemStack(output);
    }

    #region object overrides

    public override bool Equals(object? obj) => Equals(obj as ICraftingRecipe);

    public override int GetHashCode() => base.GetHashCode();

    #endregion

    #region interface ICraftingRecipe

    public CraftingPattern Pattern => _input;

    public ItemStack Output => _output;

    public bool Equals(ICraftingRecipe? other)
    {
        if (other is null)
        {
            return false;
        }

        return Output == other.Output && Pattern == other.Pattern;
    }

    #endregion
}