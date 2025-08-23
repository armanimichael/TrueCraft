using System.Collections.Generic;
using TrueCraft.Core.Inventory;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Server;

namespace TrueCraft.Inventory;

public class SlotFactory : ISlotFactory<IServerSlot>
{
    public IServerSlot GetSlot(IItemRepository itemRepository) => new ServerSlot(itemRepository);

    public List<IServerSlot> GetSlots(IItemRepository itemRepository, int count)
    {
        var rv = new List<IServerSlot>(count);

        for (var j = 0; j < count; j++)
        {
            rv.Add(new ServerSlot(itemRepository, j));
        }

        return rv;
    }
}