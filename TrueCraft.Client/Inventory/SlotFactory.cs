using System.Collections.Generic;
using TrueCraft.Core.Inventory;
using TrueCraft.Core.Logic;

namespace TrueCraft.Client.Inventory;

public class SlotFactory : ISlotFactory<ISlot>
{
    public ISlot GetSlot(IItemRepository itemRepository) => new Slot(itemRepository);

    public List<ISlot> GetSlots(IItemRepository itemRepository, int count)
    {
        var rv = new List<ISlot>(count);

        for (var j = 0; j < count; j++)
        {
            rv.Add(new Slot(itemRepository));
        }

        return rv;
    }
}