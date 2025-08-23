using Moq;
using NUnit.Framework;
using TrueCraft.Core;
using TrueCraft.Core.Logic;
using TrueCraft.Inventory;

namespace TrueCraft.Test.Inventory;

[TestFixture]
public class ServerSlotTest
{
    [TestCase(2)]
    [TestCase(5)]
    public void Ctor(int index)
    {
        var mock = new Mock<IItemRepository>(MockBehavior.Strict);
        var slot = new ServerSlot(mock.Object, index);

        Assert.AreEqual(index, slot.Index);
    }

    [TestCase(280, 14, 0)]
    [TestCase(17, 12, 1)]
    public void Item(short itemId, sbyte itemCount, short itemMetadata)
    {
        var mockProvider = new Mock<IItemProvider>(MockBehavior.Strict);
        mockProvider.Setup((p) => p.MaximumStack).Returns(64);
        var mockRepo = new Mock<IItemRepository>(MockBehavior.Strict);
        mockRepo.Setup(m => m.GetItemProvider(It.IsAny<short>())).Returns(mockProvider.Object);

        var item = new ItemStack(itemId, itemCount, itemMetadata);

        var slot = new ServerSlot(mockRepo.Object, 42);
        var dirtyChanged = false;

        slot.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "Dirty")
            {
                dirtyChanged = true;
            }
        };

        slot.Item = item;

        Assert.True(slot.Dirty);
        Assert.True(dirtyChanged);

        dirtyChanged = false;
        slot.Item = item;

        Assert.False(dirtyChanged);
        Assert.True(slot.Dirty);
    }

    [TestCase(0, 12, 280, 14, 0)]
    [TestCase(1, 7, -1, 0, 0)]
    public void GetSetSlotPacket(sbyte windowId, short index, short itemId, sbyte itemCount, short itemMetadata)
    {
        var mockRepo = new Mock<IItemRepository>(MockBehavior.Strict);
        var item = new ItemStack(itemId, itemCount, itemMetadata);
        var slot = new ServerSlot(mockRepo.Object, index);

        slot.Item = item;

        var dirtyChanged = false;

        slot.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "Dirty")
            {
                dirtyChanged = true;
            }
        };

        var isDirty = slot.Dirty;
        var packet = slot.GetSetSlotPacket(windowId);

        Assert.True((isDirty && dirtyChanged) || (!isDirty && !dirtyChanged));
        Assert.False(slot.Dirty);
        Assert.AreEqual(windowId, packet.WindowID);
        Assert.AreEqual(index, packet.SlotIndex);
        Assert.AreEqual(itemId, packet.ItemID);
        Assert.AreEqual(itemCount, packet.Count);
        Assert.AreEqual(itemMetadata, packet.Metadata);
    }
}