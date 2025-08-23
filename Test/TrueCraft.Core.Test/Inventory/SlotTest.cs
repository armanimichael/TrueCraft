using System;
using Moq;
using NUnit.Framework;
using TrueCraft.Core.Inventory;
using TrueCraft.Core.Logic;

namespace TrueCraft.Core.Test.Inventory;

[TestFixture]
public class SlotTest
{
    [Test]
    public void Ctor()
    {
        var mock = new Mock<IItemRepository>(MockBehavior.Strict);
        var slot = new Slot(mock.Object);

        Assert.AreEqual(ItemStack.EmptyStack, slot.Item);
    }

    [Test]
    public void ctor_Throws() => Assert.Throws<ArgumentNullException>(() => new Slot(null!));

    [TestCase(280, 14, 0)]
    [TestCase(17, 12, 1)]
    public void Item(short itemId, sbyte itemCount, short itemMetadata)
    {
        var mockProvider = new Mock<IItemProvider>(MockBehavior.Strict);
        mockProvider.Setup((p) => p.MaximumStack).Returns(64);
        var mockRepo = new Mock<IItemRepository>(MockBehavior.Strict);
        mockRepo.Setup(m => m.GetItemProvider(It.IsAny<short>())).Returns(mockProvider.Object);

        var item = new ItemStack(itemId, itemCount, itemMetadata);

        var slot = new Slot(mockRepo.Object);
        var itemChanged = false;

        slot.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "Item")
            {
                itemChanged = true;
            }
        };

        slot.Item = item;

        Assert.True(itemChanged);
        Assert.AreEqual(item, slot.Item);

        itemChanged = false;
        slot.Item = item;

        Assert.False(itemChanged);
        Assert.AreEqual(item, slot.Item);
    }

    // Test adding to an empty slot.
    [TestCase(7, -1, 0, 0, 280, 7, 0)]
    // Test adding nothing to a slot
    [TestCase(0, 280, 7, 0, -1, 0, 0)]
    // Test adding incompatible item id
    [TestCase(0, 280, 14, 0, 14, 12, 0)]
    // Test adding incompatible metadata
    [TestCase(0, 0x107, 48, 1, 0x107, 8, 0)]
    // Test adding compatible item id (not full slot after)
    [TestCase(1, 280, 32, 0, 280, 1, 0)]
    // Test adding compatible item id (exactly full after)
    [TestCase(16, 0x107, 48, 0, 0x107, 16, 0)]
    // Test adding compatible item (too much to fit)
    [TestCase(8, 17, 56, 0, 17, 24, 0)]
    public void CanAccept(
        int expected,
        short itemId,
        sbyte itemCount,
        short itemMetadata,
        short addId,
        sbyte addCount,
        short addMetadata
    )
    {
        var mockProvider = new Mock<IItemProvider>(MockBehavior.Strict);
        mockProvider.Setup((p) => p.MaximumStack).Returns(64);
        var mockRepo = new Mock<IItemRepository>(MockBehavior.Strict);
        mockRepo.Setup(m => m.GetItemProvider(It.IsAny<short>())).Returns(mockProvider.Object);

        var item = new ItemStack(itemId, itemCount, itemMetadata);
        var add = new ItemStack(addId, addCount, addMetadata);

        var slot = new Slot(mockRepo.Object);

        slot.Item = item;
        var canAccept = slot.CanAccept(add);

        Assert.AreEqual(expected, canAccept);
    }
}