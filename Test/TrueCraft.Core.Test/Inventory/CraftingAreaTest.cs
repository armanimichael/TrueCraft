using System;
using System.Collections.Generic;
using NUnit.Framework;
using TrueCraft.Core.Inventory;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Test.Inventory;

[TestFixture]
public class CraftingAreaTest
{
    private static ICraftingRepository GetCraftingRepository() => CoreSetup.CraftingRepository;

    private static IItemRepository GetItemRepository() =>
        // TODO mock Item Repository.
        ItemRepository.Get();

    private sealed class MockSlotFactory : ISlotFactory<ISlot>
    {
        public ISlot GetSlot(IItemRepository itemRepository) => throw new NotImplementedException();

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

    [TestCase(2, 2)]
    [TestCase(3, 3)]
    public void ctor(int width, int height)
    {
        var actual = new CraftingArea<ISlot>(
            GetItemRepository(),
            GetCraftingRepository(),
            new MockSlotFactory(),
            width,
            height
        );

        Assert.AreEqual(1 + (width * height), actual.Count);
        Assert.AreEqual(width, actual.Width);
        Assert.AreEqual(height, actual.Height);
    }

    [Test]
    public void Not_A_Recipe_2x2()
    {
        var area = new CraftingArea<ISlot>(
            GetItemRepository(),
            GetCraftingRepository(),
            new MockSlotFactory(),
            2,
            2
        );

        area[1].Item = new ItemStack(CobblestoneBlock.BlockID);

        Assert.Null(area.Recipe);
        Assert.True(area[0].Item.Empty);
        Assert.False(area[1].Item.Empty);
        Assert.True(area[2].Item.Empty);
        Assert.True(area[3].Item.Empty);
        Assert.True(area[4].Item.Empty);
    }

    [Test]
    public void Is_A_Recipe_2x2()
    {
        var area = new CraftingArea<ISlot>(
            GetItemRepository(),
            GetCraftingRepository(),
            new MockSlotFactory(),
            2,
            2
        );

        var planks = new ItemStack(WoodenPlanksBlock.BlockID);

        area[1].Item = planks;
        area[3].Item = planks;

        Assert.NotNull(area[0]);
        Assert.NotNull(area[0].Item);
        Assert.AreNotEqual(ItemStack.EmptyStack, area[0].Item);

        Assert.AreEqual(StickItem.ItemID, area[0].Item.ID);
        Assert.AreEqual(4, area[0].Item.Count);
        Assert.False(area[1].Item.Empty);
        Assert.True(area[2].Item.Empty);
        Assert.False(area[3].Item.Empty);
        Assert.True(area[4].Item.Empty);
    }

    [Test]
    public void Not_A_Recipe_3x3()
    {
        var area = new CraftingArea<ISlot>(
            GetItemRepository(),
            GetCraftingRepository(),
            new MockSlotFactory(),
            3,
            3
        );

        area[7].Item = new ItemStack(StickItem.ItemID);

        Assert.Null(area.Recipe);
        Assert.True(area[0].Item.Empty);

        for (var j = 1; j < area.Count; j++)
        {
            Assert.True(
                j != 7
                    ? area[j].Item.Empty
                    : !area[j].Item.Empty
            );
        }
    }

    [Test]
    public void Is_A_Recipe_3x3()
    {
        var area = new CraftingArea<ISlot>(
            GetItemRepository(),
            GetCraftingRepository(),
            new MockSlotFactory(),
            3,
            3
        );

        var planks = new ItemStack(WoodenPlanksBlock.BlockID);

        area[3].Item = planks;
        area[6].Item = planks;

        Assert.NotNull(area[0]);
        Assert.AreEqual(StickItem.ItemID, area[0].Item.ID);
        Assert.AreEqual(4, area[0].Item.Count);

        Assert.True(area[1].Item.Empty);
        Assert.True(area[2].Item.Empty);
        Assert.False(area[3].Item.Empty);
        Assert.True(area[4].Item.Empty);
        Assert.True(area[5].Item.Empty);
        Assert.False(area[6].Item.Empty);
        Assert.True(area[7].Item.Empty);
        Assert.True(area[8].Item.Empty);
        Assert.True(area[9].Item.Empty);
    }

    [Test]
    public void Is_Multiple_Recipes()
    {
        var area = new CraftingArea<ISlot>(
            GetItemRepository(),
            GetCraftingRepository(),
            new MockSlotFactory(),
            3,
            3
        );

        // Enough items for two recipes.
        // Note there is an extra item in area[2].
        area[2].Item = new ItemStack(WoodenPlanksBlock.BlockID, 3);
        area[5].Item = new ItemStack(WoodenPlanksBlock.BlockID, 2);

        Assert.NotNull(area[0]);
        Assert.AreEqual(StickItem.ItemID, area[0].Item.ID);

        // Note: play-testing on Beta 1.7.3 indicates it only shows
        // one recipe's worth of output even when multiple inputs are
        // placed.
        Assert.AreEqual(4, area[0].Item.Count);
    }

    [Test]
    public void TakeOutput1()
    {
        var area = new CraftingArea<ISlot>(
            GetItemRepository(),
            GetCraftingRepository(),
            new MockSlotFactory(),
            2,
            2
        );

        var planks = new ItemStack(WoodenPlanksBlock.BlockID, 1);
        area[1].Item = planks;
        area[3].Item = planks;

        // Take one output
        var actual = area.TakeOutput();

        // This output should be 4 sticks
        Assert.AreEqual(StickItem.ItemID, actual.ID);
        Assert.AreEqual(4, actual.Count);

        // The entire grid should now be empty.
        for (var j = 0; j < area.Count; j++)
        {
            Assert.True(area[j].Item.Empty);
        }
    }

    [Test]
    public void TakeOutput2()
    {
        var area = new CraftingArea<ISlot>(
            GetItemRepository(),
            GetCraftingRepository(),
            new MockSlotFactory(),
            2,
            2
        );

        // Add enough for two recipes
        var planks = new ItemStack(WoodenPlanksBlock.BlockID, 2);
        area[1].Item = planks;
        area[3].Item = planks;

        // Take one output
        var actual = area.TakeOutput();

        // This output should be 4 sticks
        Assert.AreEqual(StickItem.ItemID, actual.ID);
        Assert.AreEqual(4, actual.Count);

        // There should still be input for one more recipe
        Assert.AreEqual(StickItem.ItemID, area[0].Item.ID);
        Assert.AreEqual(4, area[0].Item.Count);
        Assert.False(area[1].Item.Empty);
        Assert.AreEqual(WoodenPlanksBlock.BlockID, area[1].Item.ID);
        Assert.AreEqual(1, area[1].Item.Count);
        Assert.True(area[2].Item.Empty);
        Assert.False(area[3].Item.Empty);
        Assert.AreEqual(WoodenPlanksBlock.BlockID, area[3].Item.ID);
        Assert.AreEqual(1, area[3].Item.Count);
        Assert.True(area[4].Item.Empty);
    }

    // Put in items for a Pickaxe, hoe, and shovel.
    // Take them out one after the other.
    [Test]
    public void TakeOutput3()
    {
        var area = new CraftingArea<ISlot>(
            GetItemRepository(),
            GetCraftingRepository(),
            new MockSlotFactory(),
            3,
            3
        );

        // Add Cobblestone & sticks for a pickaxe, hoe, and shovel
        area[1].Item = new ItemStack(CobblestoneBlock.BlockID, 2);
        area[2].Item = new ItemStack(CobblestoneBlock.BlockID, 3);
        area[3].Item = new ItemStack(CobblestoneBlock.BlockID, 1);
        area[5].Item = new ItemStack(StickItem.ItemID, 3);
        area[8].Item = new ItemStack(StickItem.ItemID, 3);

        // Take out a pickaxe & confirm
        var ax = area.TakeOutput();
        Assert.AreEqual((short) ItemIDs.StonePickaxe, ax.ID);
        Assert.AreEqual(1, ax.Count);

        // Confirm remaining inputs
        Assert.AreEqual(1, area[1].Item.Count);
        Assert.AreEqual(2, area[2].Item.Count);
        Assert.True(area[3].Item.Empty);
        Assert.AreEqual(2, area[5].Item.Count);
        Assert.AreEqual(2, area[8].Item.Count);

        // Take out a Hoe and confirm
        var hoe = area.TakeOutput();
        Assert.AreEqual((short) ItemIDs.StoneHoe, hoe.ID);
        Assert.AreEqual(1, hoe.Count);

        // confirm remaining inputs
        Assert.True(area[1].Item.Empty);
        Assert.AreEqual(1, area[2].Item.Count);
        Assert.True(area[3].Item.Empty);
        Assert.AreEqual(1, area[5].Item.Count);
        Assert.AreEqual(1, area[8].Item.Count);

        // Take out a shovel & confirm
        var shovel = area.TakeOutput();
        Assert.AreEqual((short) ItemIDs.StoneShovel, shovel.ID);
        Assert.AreEqual(1, shovel.Count);

        // Confirm entire grid is now empty
        for (var j = 0; j < area.Count; j++)
        {
            Assert.True(area[j].Item.Empty);
        }
    }
}