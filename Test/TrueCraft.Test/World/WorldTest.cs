using System.IO;
using System.Reflection;
using Moq;
using NUnit.Framework;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Test.World;

[TestFixture]
public class WorldTest
{
    [Test]
    public void TestManifestLoaded()
    {
        var mockServer = new Mock<IMultiplayerServer>(MockBehavior.Strict);

        var mockBlockRepository = new Mock<IBlockRepository>(MockBehavior.Strict);

        var mockItemRepository = new Mock<IItemRepository>(MockBehavior.Strict);

        var mockServiceLocator = new Mock<IServerServiceLocator>(MockBehavior.Strict);
        mockServiceLocator.Setup(x => x.Server).Returns(mockServer.Object);
        mockServiceLocator.Setup(x => x.BlockRepository).Returns(mockBlockRepository.Object);
        mockServiceLocator.Setup(x => x.ItemRepository).Returns(mockItemRepository.Object);

        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var world = TrueCraft.World.World.LoadWorld(mockServiceLocator.Object, Path.Combine(assemblyDir, "Files"));

        // Constants from manifest.nbt
        Assert.AreEqual(new PanDimensionalVoxelCoordinates(DimensionID.Overworld, 0, 60, 0), world.SpawnPoint);
        Assert.AreEqual(1168393583, world.Seed);
    }
}