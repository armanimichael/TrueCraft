﻿using System;
using System.Collections.Generic;

namespace TrueCraft.Core.Logic;

public class ItemRepository : IItemRepository, IRegisterItemProvider
{
    private readonly List<IItemProvider> ItemProviders;

    // Consumers must call Init before this class is used.
    private static ItemRepository _singleton = null!;

    private ItemRepository()
    {
        ItemProviders = new List<IItemProvider>();
    }

    internal static IItemRepository Init(IDiscover discover)
    {
        if (!object.ReferenceEquals(_singleton, null))
            return _singleton;

        _singleton = new ItemRepository();
        discover.DiscoverItemProviders(_singleton);

        return _singleton;
    }

    [Obsolete("Inject IItemRepository instead")]
    public static IItemRepository Get()
    {
#if DEBUG
        if (object.ReferenceEquals(_singleton, null))
            throw new ApplicationException("Call to ItemRepository.Get without initialization.");
#endif
        return _singleton;
    }

    public IItemProvider? GetItemProvider(short id)
    {
        // TODO: Binary search
        for (int i = 0; i < ItemProviders.Count; i++)
        {
            if (ItemProviders[i].ID == id)
                return ItemProviders[i];
        }
        return null;
    }

    /// <inheritdoc />
    public void RegisterItemProvider(IItemProvider provider)
    {
        int i;
        for (i = ItemProviders.Count - 1; i >= 0; i--)
        {
            if (provider.ID == ItemProviders[i].ID)
            {
                ItemProviders[i] = provider; // Override
                return;
            }
            if (ItemProviders[i].ID < provider.ID)
                break;
        }
        ItemProviders.Insert(i + 1, provider);
    }
}
