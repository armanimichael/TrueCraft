using System;
using System.IO;
using System.Reflection;
using Moq;
using NUnit.Framework;
using TrueCraft.Core;
using TrueCraft.Core.Lighting;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;
using TrueCraft.TerrainGen;
using TrueCraft.World;

namespace TrueCraft.Test.World;

[TestFixture]
public class DimensionTest
{
    private readonly string _assemblyDir;

    private readonly IServerServiceLocator _serviceLocator;

    private readonly ILightingQueue _lightingQueue;

    private readonly IEntityManager _entityManager;

    public DimensionTest()
    {
        var mockProvider = new Mock<IBlockProvider>(MockBehavior.Strict);
        mockProvider.Setup(x => x.ID).Returns(3);

        var mockRepository = new Mock<IBlockRepository>(MockBehavior.Strict);
        mockRepository.Setup(x => x.GetBlockProvider(It.Is<byte>(b => b == 3))).Returns(mockProvider.Object);

        var mockItemRepository = new Mock<IItemRepository>(MockBehavior.Strict);

        var mockServer = new Mock<IMultiplayerServer>(MockBehavior.Strict);

        var mockServiceLocator = new Mock<IServerServiceLocator>(MockBehavior.Strict);
        mockServiceLocator.Setup(x => x.BlockRepository).Returns(mockRepository.Object);
        mockServiceLocator.Setup(x => x.ItemRepository).Returns(mockItemRepository.Object);
        mockServiceLocator.Setup(x => x.Server).Returns(mockServer.Object);
        _serviceLocator = mockServiceLocator.Object;

        var mockLightingQueue = new Mock<ILightingQueue>(MockBehavior.Strict);

        mockLightingQueue.Setup(
            x => x.Enqueue(
                It.IsAny<GlobalVoxelCoordinates>(),
                It.IsAny<LightingOperationMode>(),
                It.IsAny<LightingOperationKind>(),
                It.IsAny<byte>()
            )
        );

        _lightingQueue = mockLightingQueue.Object;

        var mockEntityManager = new Mock<IEntityManager>(MockBehavior.Strict);
        _entityManager = mockEntityManager.Object;

        _assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    }

    [OneTimeSetUp]
    public void Setup()
    {
        try
        {
            WhoAmI.Answer = IAm.Server;
        }
        catch (InvalidOperationException)
        {
            // Disregard the error of setting this more than once.
            // Other tests may have set it, and we can't check, but
            // we need to be sure it is set.
        }
    }

    private Dimension BuildDimension()
    {
        var filePath = Path.Combine(_assemblyDir, "Files");

        return new Dimension(
            _serviceLocator,
            filePath,
            DimensionID.Overworld,
            new FlatlandGenerator(1234),
            _lightingQueue,
            _entityManager
        );
    }

    /// <summary>
    /// Tests that the GetChunk(GlobalVoxelCoordinates) method does not
    /// generate/load a Chunk that is not already loaded.
    /// </summary>
    [Test]
    public void TestGetChunk_Global_NoGenerate()
    {
        var dimension = BuildDimension();

        var a = dimension.GetChunk(new GlobalVoxelCoordinates(0, 0, 0));
        var b = dimension.GetChunk(new GlobalVoxelCoordinates(-1, 0, 0));
        var c = dimension.GetChunk(new GlobalVoxelCoordinates(-1, 0, -1));
        var d = dimension.GetChunk(new GlobalVoxelCoordinates(0, 0, -1));

        Assert.IsNull(a);
        Assert.IsNull(b);
        Assert.IsNull(c);
        Assert.IsNull(d);
    }

    /// <summary>
    /// Tests that the GetChunk(GlobalChunkCoordinates) method does not
    /// generate/load a Chunk that is not already loaded.
    /// </summary>
    [Test]
    public void TestGetChunk_Chunk_NoGenerate()
    {
        var dimension = BuildDimension();

        var a = dimension.GetChunk(new GlobalChunkCoordinates(0, 0));
        var b = dimension.GetChunk(new GlobalChunkCoordinates(-1, 0));
        var c = dimension.GetChunk(new GlobalChunkCoordinates(-1, -1));
        var d = dimension.GetChunk(new GlobalChunkCoordinates(0, -1));

        Assert.IsNull(a);
        Assert.IsNull(b);
        Assert.IsNull(c);
        Assert.IsNull(d);
    }

    /// <summary>
    /// Tests that the GetChunk(GlobalChunkCoordinates) method will not
    /// return a Chunk unless it is already in memory.
    /// </summary>
    [Test]
    public void TestGetChunk_Chunk_InMemory()
    {
        var dimension = BuildDimension();
        // The Chunk at 16,6 is not present in the region file in the Files
        // folder, so it cannot be loaded from disk.
        var chunkCoordinates = new GlobalChunkCoordinates(16, 6);

        // Try to get the Chunk from memory
        var chunk = dimension.GetChunk(chunkCoordinates, LoadEffort.InMemory);
        Assert.IsNull(chunk);

        // Generate the Chunk
        dimension.GetChunk(chunkCoordinates, LoadEffort.Generate);

        // Try again to get the Chunk from memory.
        chunk = dimension.GetChunk(chunkCoordinates, LoadEffort.InMemory);
        Assert.IsNotNull(chunk);
        Assert.AreEqual(chunkCoordinates, chunk?.Coordinates);
    }

    /// <summary>
    /// Tests that the GetChunk(GlobalChunkCoordinates) method will load
    /// an existing Chunk from disk with LoadEffort.Load.
    /// </summary>
    [Test]
    public void TestGetChunk_Chunk_Load()
    {
        var dimension = BuildDimension();
        var chunkCoordinates = new GlobalChunkCoordinates(0, 0);

        // The chunk is known to be not in memory as we just created the
        // Dimension.
        var chunk = dimension.GetChunk(chunkCoordinates, LoadEffort.Load);
        Assert.IsNotNull(chunk);
        Assert.AreEqual(chunkCoordinates, chunk?.Coordinates);
    }

    /// <summary>
    /// Tests that the GetChunk(GlobalChunkCoordinates) method will generate
    /// a new Chunk.
    /// </summary>
    [Test]
    public void TestGetChunk_Chunk_Generate()
    {
        var dimension = BuildDimension();
        // The Chunk at 16,6 is not present in the region file in the Files
        // folder, so it cannot be loaded from disk.
        var chunkCoordinates = new GlobalChunkCoordinates(16, 6);

        // The Chunk is known not to be in memory because we just created
        // a brand-new dimension.

        // Assert that the Chunk can not be loaded from disk.
        var chunk = dimension.GetChunk(chunkCoordinates, LoadEffort.Load);
        Assert.IsNull(chunk);

        // Generate the Chunk
        chunk = dimension.GetChunk(chunkCoordinates, LoadEffort.Generate);
        Assert.IsNotNull(chunk);
        Assert.AreEqual(chunkCoordinates, chunk?.Coordinates);
    }
}