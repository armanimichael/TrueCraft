using Moq;
using NUnit.Framework;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;
using TrueCraft.World;

namespace TrueCraft.Test.World;

public class DimensionFactoryTest
{
    [Test]
    public void TestBuildDimensions()
    {
        var baseDirectory = "FakeBaseDirectory";
        var mockServer = new Mock<IMultiplayerServer>(MockBehavior.Strict);

        var mockBlockRepository = new Mock<IBlockRepository>(MockBehavior.Strict);

        var mockItemRepository = new Mock<IItemRepository>(MockBehavior.Strict);

        var mockServiceLocator = new Mock<IServerServiceLocator>(MockBehavior.Strict);
        mockServiceLocator.Setup(x => x.Server).Returns(mockServer.Object);
        mockServiceLocator.Setup(x => x.BlockRepository).Returns(mockBlockRepository.Object);
        mockServiceLocator.Setup(x => x.ItemRepository).Returns(mockItemRepository.Object);

        var factory = new DimensionFactory();

        var actual = factory.BuildDimensions(mockServiceLocator.Object, baseDirectory, 314159);

        Assert.AreEqual(2, actual.Count);

        Assert.IsNull(actual[0]); // TODO Update for Nether

        IDimension overWorld = actual[1];
        Assert.AreEqual("OverWorld", overWorld.Name);
        Assert.AreEqual(DimensionID.Overworld, overWorld.ID);
    }
}