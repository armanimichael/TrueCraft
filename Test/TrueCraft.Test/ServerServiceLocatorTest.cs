using System;
using Moq;
using NUnit.Framework;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Server;
using TrueCraft.World;

namespace TrueCraft.Test;

public class ServerServiceLocatorTest
{
    [Test]
    public void Ctor()
    {
        var mockServer = new Mock<IMultiplayerServer>(MockBehavior.Strict);
        var mockBlockRepository = new Mock<IBlockRepository>(MockBehavior.Strict);
        var mockItemRepository = new Mock<IItemRepository>(MockBehavior.Strict);
        var mockCraftingRepository = new Mock<ICraftingRepository>(MockBehavior.Strict);
        var mockServiceLocator = new Mock<IServiceLocator>(MockBehavior.Strict);
        mockServiceLocator.Setup(x => x.ItemRepository).Returns(mockItemRepository.Object);
        mockServiceLocator.Setup(x => x.BlockRepository).Returns(mockBlockRepository.Object);
        mockServiceLocator.Setup(x => x.CraftingRepository).Returns(mockCraftingRepository.Object);

        var locator = new ServerServiceLocator(
            mockServer.Object,
            mockServiceLocator.Object
        );

        Assert.Throws<InvalidOperationException>(
            () =>
            {
                var t = locator.World;
            }
        );

        Assert.True(ReferenceEquals(mockServer.Object, locator.Server));
        Assert.True(ReferenceEquals(mockBlockRepository.Object, locator.BlockRepository));
        Assert.True(ReferenceEquals(mockItemRepository.Object, locator.ItemRepository));
        Assert.True(ReferenceEquals(mockCraftingRepository.Object, locator.CraftingRepository));
    }

    [Test]
    public void ctor_Throws()
    {
        var mockServer = new Mock<IMultiplayerServer>(MockBehavior.Strict);
        var mockBlockRepository = new Mock<IBlockRepository>(MockBehavior.Strict);
        var mockItemRepository = new Mock<IItemRepository>(MockBehavior.Strict);
        var mockCraftingRepository = new Mock<ICraftingRepository>(MockBehavior.Strict);
        var mockServiceLocator = new Mock<IServiceLocator>(MockBehavior.Strict);
        mockServiceLocator.Setup(x => x.ItemRepository).Returns(mockItemRepository.Object);
        mockServiceLocator.Setup(x => x.BlockRepository).Returns(mockBlockRepository.Object);
        mockServiceLocator.Setup(x => x.CraftingRepository).Returns(mockCraftingRepository.Object);

        Assert.Throws<ArgumentNullException>(() => new ServerServiceLocator(null!, mockServiceLocator.Object));
    }

    [Test]
    public void WorldSetter()
    {
        var mockServer = new Mock<IMultiplayerServer>(MockBehavior.Strict);
        var mockBlockRepository = new Mock<IBlockRepository>(MockBehavior.Strict);
        var mockItemRepository = new Mock<IItemRepository>(MockBehavior.Strict);
        var mockCraftingRepository = new Mock<ICraftingRepository>(MockBehavior.Strict);
        var mockServiceLocator = new Mock<IServiceLocator>(MockBehavior.Strict);
        mockServiceLocator.Setup(x => x.ItemRepository).Returns(mockItemRepository.Object);
        mockServiceLocator.Setup(x => x.BlockRepository).Returns(mockBlockRepository.Object);
        mockServiceLocator.Setup(x => x.CraftingRepository).Returns(mockCraftingRepository.Object);

        var locator = new ServerServiceLocator(
            mockServer.Object,
            mockServiceLocator.Object
        );

        var mockWorld = new Mock<IWorld>(MockBehavior.Strict);

        Assert.Throws<InvalidOperationException>(
            () =>
            {
                var t = locator.World;
            }
        );

        locator.World = mockWorld.Object;
        Assert.Throws<InvalidOperationException>(() => locator.World = mockWorld.Object);
        Assert.True(ReferenceEquals(mockWorld.Object, locator.World));
    }
}