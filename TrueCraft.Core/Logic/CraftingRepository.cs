using System.Collections.Generic;

namespace TrueCraft.Core.Logic
{
    public class CraftingRepository : ICraftingRepository, IRegisterRecipe
    {
        private readonly List<ICraftingRecipe> _recipes;

        // Consumers must call Init before using this class.
        private static CraftingRepository _singleton = null!;

        private CraftingRepository()
        {
            _recipes = new List<ICraftingRecipe>();
        }

        internal static ICraftingRepository Init(IDiscover discover)
        {
            if (!object.ReferenceEquals(_singleton, null))
                return _singleton;

            _singleton = new CraftingRepository();
            discover.DiscoverRecipes(_singleton);

            return _singleton;
        }

        public ICraftingRecipe? GetRecipe(CraftingPattern pattern)
        {
            foreach (ICraftingRecipe r in _recipes)
                if (r.Pattern == pattern)
                    return r;

            return null;
        }

        public void RegisterRecipe(ICraftingRecipe recipe)
        {
            _recipes.Add(recipe);
        }
    }
}