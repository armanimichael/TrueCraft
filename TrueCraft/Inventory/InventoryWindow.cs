using System;
using System.Collections.Generic;
using TrueCraft.Core;
using TrueCraft.Core.Inventory;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Core.Server;

namespace TrueCraft.Inventory;

public class InventoryWindow : InventoryWindow<IServerSlot>, IServerWindow
{
    public InventoryWindow(
        IItemRepository itemRepository,
        ICraftingRepository craftingRepository,
        ISlotFactory<IServerSlot> slotFactory,
        ISlots<IServerSlot> mainInventory,
        ISlots<IServerSlot> hotBar
    )
        :
        base(itemRepository, craftingRepository, slotFactory, mainInventory, hotBar) { }

    public CloseWindowPacket GetCloseWindowPacket() => throw
        // The Server should never send a Close Window Packet for this Window.
        new NotImplementedException();

    public List<SetSlotPacket> GetDirtySetSlotPackets()
    {
        var offset = Armor.Count + CraftingGrid.Count;
        var packets = ((IServerSlots) MainInventory).GetSetSlotPackets(WindowID, (short) offset);
        offset += MainInventory.Count;

        packets.AddRange(((IServerSlots) Hotbar).GetSetSlotPackets(WindowID, (short) offset));

        return packets;
    }

    public OpenWindowPacket GetOpenWindowPacket() => throw
        // The Server should never send an Open Window Packet for the Inventory Window.
        new NotImplementedException();

    public WindowItemsPacket GetWindowItemsPacket()
    {
        var count = Count;
        var items = new ItemStack[count];

        for (var j = 0; j < count; j++)
        {
            items[j] = this[j];
        }

        return new WindowItemsPacket(0, items);
    }

    public override void SetSlots(ItemStack[] slotContents)
    {
#if DEBUG
        if (slotContents.Length != Count)
        {
            throw new ApplicationException(
                $"{nameof(slotContents)}.Length has value of {slotContents.Length}, but {Count} was expected."
            );
        }
#endif
        var index = 0;

        for (int j = 0, jul = Slots.Length; j < jul; j++)
        for (int k = 0, kul = Slots[j].Count; k < kul; k++)
        {
            Slots[j][k].Item = slotContents[index];
            Slots[j][k].SetClean();
            index++;
        }
    }

    public void HandleClick(IRemoteClient client, ClickWindowPacket packet)
    {
        int slotIndex = packet.SlotIndex;
        var itemStaging = client.ItemStaging;
        bool handled;

        if (packet.RightClick)
        {
            if (packet.Shift)
            {
                handled = HandleShiftRightClick(slotIndex, ref itemStaging);
            }
            else
            {
                handled = HandleRightClick(slotIndex, ref itemStaging);
            }
        }
        else
        {
            if (packet.Shift)
            {
                handled = HandleShiftLeftClick(slotIndex, ref itemStaging);
            }
            else
            {
                handled = HandleLeftClick(slotIndex, ref itemStaging);
            }
        }

        if (handled)
        {
            client.ItemStaging = itemStaging;
        }

        client.QueuePacket(new TransactionStatusPacket(packet.WindowID, packet.TransactionID, handled));
    }

    protected bool HandleLeftClick(int slotIndex, ref ItemStack itemStaging)
    {
        if (IsOutputSlot(slotIndex))
        {
            if (!itemStaging.Empty)
            {
                if (itemStaging.CanMerge(this[slotIndex]))
                {
                    // The mouse pointer has some items in it, and they
                    // are compatible with the output
                    var maxItems =
                        ItemRepository.GetItemProvider(itemStaging.ID)!
                                      .MaximumStack; // itemStaging is known to not be Empty

                    var totalItems = itemStaging.Count + this[slotIndex].Count;

                    if (totalItems > maxItems)
                    {
                        // There are too many items, so this is a No-Op.
                        // It is assumed that this will follow the pattern of
                        // sending a Window Click packet and responding
                        // with accepted = true;
                        return true;
                    }
                    else
                    {
                        // There's enough room to pick some up, so pick up o
                        // Recipe's worth.
                        itemStaging = new ItemStack(
                            itemStaging.ID,
                            (sbyte) totalItems,
                            itemStaging.Metadata,
                            itemStaging.Nbt
                        );

                        CraftingGrid.TakeOutput();

                        return true;
                    }
                }
                else
                {
                    // The mouse pointer contains an item incompatible with
                    // the output, so this is a No-Op.
                    // Play-testing Beta 1.7.3 with tcpdump shows that the client
                    // sends a Window Click Packet, and the server responds
                    // with accepted = true, even though it is a No-Op
                    return true;
                }
            }
            else
            {
                // The mouse pointer is empty.  Take one Recipe worth of output
                itemStaging = CraftingGrid.TakeOutput();

                return true;
            }
        }

        if (!itemStaging.Empty)
        {
            // Is the slot compatible
            if (itemStaging.CanMerge(this[slotIndex]))
            {
                // How many Items can be placed?
                var maxItems =
                    ItemRepository.GetItemProvider(itemStaging.ID)!
                                  .MaximumStack; // itemStaging is known to not be Empty

                var totalItems = itemStaging.Count + this[slotIndex].Count;
                var old = this[slotIndex];

                if (totalItems > maxItems)
                {
                    // Fill the Slot to the max, retaining remaining items.
                    this[slotIndex] = new ItemStack(old.ID, maxItems, old.Metadata, old.Nbt);

                    itemStaging = new ItemStack(
                        itemStaging.ID,
                        (sbyte) (totalItems - maxItems),
                        itemStaging.Metadata,
                        itemStaging.Nbt
                    );

                    return true;
                }
                else
                {
                    // Place all items, the mouse pointer becomes empty.
                    this[slotIndex] = new ItemStack(
                        itemStaging.ID,
                        (sbyte) totalItems,
                        itemStaging.Metadata,
                        itemStaging.Nbt
                    );

                    itemStaging = ItemStack.EmptyStack;

                    return true;
                }
            }
            else
            {
                // The slot is not compatible with the mouse pointer, so
                // swap them.
                var tmp = itemStaging;
                itemStaging = this[slotIndex];
                this[slotIndex] = tmp;

                return true;
            }
        }
        else
        {
            // The mouse pointer is empty, so pick up everything.
            itemStaging = this[slotIndex];
            this[slotIndex] = ItemStack.EmptyStack;

            return true;
        }
    }

    protected bool HandleShiftLeftClick(int slotIndex, ref ItemStack itemStaging)
    {
        if (IsOutputSlot(slotIndex))
        {
            var output = this[slotIndex];

            if (output.Empty)
                // This is a No-Op.
            {
                return true;
            }

            // Q: What if we craft 4 sticks, but only have room for 2?
            // Play-testing this in Beta 1.7.3 shows that the excess sticks
            // simply disappeared.

            output = this[slotIndex];
            var remaining = MainInventory.StoreItemStack(output, true);
            remaining = Hotbar.StoreItemStack(remaining, false);
            remaining = MainInventory.StoreItemStack(remaining, false);

            if (remaining.Count != output.Count)
            {
                CraftingGrid.TakeOutput();
            }

            return true;
        }

        this[slotIndex] = MoveItemStack(slotIndex);

        return true;
    }

    private ItemStack MoveItemStack(int fromSlotIndex)
    {
        var src = (AreaIndices) GetAreaIndex(fromSlotIndex);

        if (src == AreaIndices.Main)
        {
            return Hotbar.StoreItemStack(this[fromSlotIndex], false);
        }
        else if (src == AreaIndices.Hotbar)
        {
            return MainInventory.StoreItemStack(this[fromSlotIndex], false);
        }
        else
        {
            var remaining = MainInventory.StoreItemStack(this[fromSlotIndex], true);

            if (remaining.Empty)
            {
                return remaining;
            }

            remaining = Hotbar.StoreItemStack(remaining, false);

            if (remaining.Empty)
            {
                return remaining;
            }

            return MainInventory.StoreItemStack(remaining, false);
        }
    }

    protected bool HandleRightClick(int slotIndex, ref ItemStack itemStaging)
    {
        if (IsOutputSlot(slotIndex))
        {
            var output = this[slotIndex];

            if (output.Empty)
                // It looks odd, but beta 1.7.3 client really does send a
                // window click packet, and the server responds with
                // accepted = true.  This is a No-Op.
            {
                return true;
            }

            // If the item is not compatible with the hand, do nothing
            if (!itemStaging.CanMerge(output))
                // It looks odd, but beta 1.7.3 client really does send a
                // window click packet, and the server responds with
                // accepted = true.  This is a No-Op.
            {
                return true;
            }

            // Pick up one Recipe's worth of output.
            // Q: do we have room for it?
            var itemInOutput = ItemRepository.GetItemProvider(output.ID)!; // output is known to not be Empty
            int maxHandStack = itemInOutput.MaximumStack;

            if (!output.Empty && itemStaging.CanMerge(output) && itemStaging.Count + output.Count <= maxHandStack)
            {
                output = CraftingGrid.TakeOutput();

                itemStaging = new ItemStack(
                    output.ID,
                    (sbyte) (output.Count + itemStaging.Count),
                    output.Metadata,
                    output.Nbt
                );

                return true;
            }

            return true;
        }

        if (!itemStaging.Empty)
        {
            if (this[slotIndex].CanMerge(itemStaging))
            {
                // The hand holds something, and the slot contents are compatible, place one item.
                int maxStack =
                    ItemRepository.GetItemProvider(itemStaging.ID)!
                                  .MaximumStack; // itemStaging is known to not be Empty

                if (maxStack > this[slotIndex].Count)
                {
                    this[slotIndex] = new ItemStack(
                        itemStaging.ID,
                        (sbyte) (this[slotIndex].Count + 1),
                        itemStaging.Metadata,
                        itemStaging.Nbt
                    );

                    itemStaging = itemStaging.GetReducedStack(1);

                    return true;
                }

                // Right-clicking on a full compatible slot is a No-Op.
                // The Beta 1.7.3 client does send a Window Click and it
                // is acknowledged by the server.
                return true;
            }
            else
            {
                // The slot contents are not compatible with the items in hand.
                // Swap them.
                var tmp = this[slotIndex];
                this[slotIndex] = itemStaging;
                itemStaging = tmp;

                return true;
            }
        }
        else
        {
            // If the hand is empty, pick up half the stack.
            var slotContent = this[slotIndex];

            if (slotContent.Empty)
                // Right-clicking an empty hand on an empty slot is a No-Op.
                // Beta 1.7.3 does send a Window Click and it is acknowledged
                // by the server.
            {
                return true;
            }

            int numToPickUp = slotContent.Count;
            numToPickUp = (numToPickUp / 2) + (numToPickUp & 0x0001);
            itemStaging = new ItemStack(slotContent.ID, (sbyte) numToPickUp, slotContent.Metadata, slotContent.Nbt);
            this[slotIndex] = slotContent.GetReducedStack(numToPickUp);

            return true;
        }
    }

    protected bool HandleShiftRightClick(int slotIndex, ref ItemStack itemStaging) => HandleShiftLeftClick(slotIndex, ref itemStaging);

    public void Save() => throw new NotImplementedException();
}