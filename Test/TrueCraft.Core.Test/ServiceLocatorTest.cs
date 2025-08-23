using System;
using Moq;
using NUnit.Framework;
using TrueCraft.Core;
using TrueCraft.Core.Logic;

namespace TrueCraft.Test;

public class ServiceLocatorTest
{
    [Test]
    public void ctor()
    {
        var mockBlockRepository = new Mock<IBlockRepository>(MockBehavior.Strict);
        var mockItemRepository = new Mock<IItemRepository>(MockBehavior.Strict);
        var mockCraftingRepository = new Mock<ICraftingRepository>(MockBehavior.Strict);

        var locator = new ServiceLocator(
            mockBlockRepository.Object,
            mockItemRepository.Object,
            mockCraftingRepository.Object
        );

        Assert.True(ReferenceEquals(mockBlockRepository.Object, locator.BlockRepository));
        Assert.True(ReferenceEquals(mockItemRepository.Object, locator.ItemRepository));
        Assert.True(ReferenceEquals(mockCraftingRepository.Object, locator.CraftingRepository));
    }

    [Test]
    public void ctor_Throws()
    {
        var mockBlockRepository = new Mock<IBlockRepository>(MockBehavior.Strict);
        var mockItemRepository = new Mock<IItemRepository>(MockBehavior.Strict);
        var mockCraftingRepository = new Mock<ICraftingRepository>(MockBehavior.Strict);

        Assert.Throws<ArgumentNullException>(
            () =>
                new ServiceLocator(null!, mockItemRepository.Object, mockCraftingRepository.Object)
        );

        Assert.Throws<ArgumentNullException>(
            () =>
                new ServiceLocator(mockBlockRepository.Object, null!, mockCraftingRepository.Object)
        );

        Assert.Throws<ArgumentNullException>(
            () =>
                new ServiceLocator(mockBlockRepository.Object, mockItemRepository.Object, null!)
        );
    }
}