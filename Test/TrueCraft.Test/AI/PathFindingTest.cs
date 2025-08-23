using System;
using System.Diagnostics;
using Moq;
using NUnit.Framework;
using TrueCraft.Core.AI;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;
using TrueCraft.Test.World;
using TrueCraft.World;

namespace TrueCraft.Core.Test.AI;

[TestFixture]
public class PathFindingTest
{
    private const int StoneBlockId = 1;

    private const int SurfaceHeight = 4;

    private IBlockRepository _blockRepository;

    private IItemRepository _itemRepository;

    private IEntityManager _entityManager;

    public PathFindingTest()
    {
        var mockProvider = new Mock<IBlockProvider>(MockBehavior.Strict);
        mockProvider.Setup(x => x.ID).Returns(3);
        mockProvider.Setup(x => x.BoundingBox).Returns(new BoundingBox(Vector3.Zero, Vector3.One));

        var mockStoneBlock = new Mock<IBlockProvider>(MockBehavior.Strict);
        mockStoneBlock.Setup(x => x.ID).Returns(1);
        mockStoneBlock.Setup(x => x.BoundingBox).Returns(new BoundingBox(Vector3.Zero, Vector3.One));

        var mockAirBlock = new Mock<IBlockProvider>(MockBehavior.Strict);
        mockAirBlock.Setup(x => x.ID).Returns(0);
        mockAirBlock.Setup(x => x.BoundingBox).Returns((BoundingBox?) null);

        var mockRepository = new Mock<IBlockRepository>(MockBehavior.Strict);
        mockRepository.Setup(x => x.GetBlockProvider(It.Is<byte>(b => b == 0))).Returns(mockAirBlock.Object);
        mockRepository.Setup(x => x.GetBlockProvider(It.Is<byte>(b => b == 1))).Returns(mockStoneBlock.Object);
        mockRepository.Setup(x => x.GetBlockProvider(It.Is<byte>(b => b == 3))).Returns(mockProvider.Object);
        _blockRepository = mockRepository.Object;

        var mockItemRepository = new Mock<IItemRepository>(MockBehavior.Strict);
        _itemRepository = mockItemRepository.Object;

        var mockEntityManager = new Mock<IEntityManager>(MockBehavior.Strict);
        _entityManager = mockEntityManager.Object;
    }

    private static void DrawGrid(PathResult path, FakeDimension dimension)
    {
        var h = SurfaceHeight;

        for (var z = -8; z < 8; z++)
        {
            for (var x = -8; x < 8; x++)
            {
                var coords = new GlobalVoxelCoordinates(x, h, z);

                if (path.Contains(coords))
                {
                    Console.Write("o");
                }
                else
                {
                    var id = dimension.GetBlockID(coords);

                    if (id != 0)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write("_");
                    }
                }
            }

            Console.WriteLine();
        }
    }

    private FakeDimension BuildDimension()
    {
        var rv = new FakeDimension(_blockRepository, _itemRepository, _entityManager);

        // Generate 4 chunks around the origin.
        var chunk = rv.GetChunk(new GlobalChunkCoordinates(0, 0), LoadEffort.Generate);
        FillChunk(chunk!);
        chunk = rv.GetChunk(new GlobalChunkCoordinates(0, -1), LoadEffort.Generate);
        FillChunk(chunk!);
        chunk = rv.GetChunk(new GlobalChunkCoordinates(-1, 0), LoadEffort.Generate);
        FillChunk(chunk!);
        chunk = rv.GetChunk(new GlobalChunkCoordinates(-1, -1), LoadEffort.Generate);
        FillChunk(chunk!);

        return rv;
    }

    private static void FillChunk(IChunk chunk)
    {
        for (var x = 0; x < Chunk.Width; x++)
        for (var z = 0; z < Chunk.Depth; z++)
        for (var y = 0; y < SurfaceHeight; y++)
        {
            chunk.SetBlockID(new LocalVoxelCoordinates(x, y, z), StoneBlockId);
        }
    }

    [Test]
    public void TestAStarLinearPath()
    {
        var dimension = BuildDimension();
        var astar = new AStarPathFinder();
        var h = SurfaceHeight;

        var watch = new Stopwatch();
        watch.Start();

        var path = astar.FindPath(
            dimension,
            new BoundingBox(),
            new GlobalVoxelCoordinates(0, h, 0),
            new GlobalVoxelCoordinates(5, h, 0)
        );

        watch.Stop();

        Assert.IsNotNull(path);
        DrawGrid(path!, dimension);
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");

        var expected = new[]
                       {
                           new GlobalVoxelCoordinates(0, h, 0),
                           new GlobalVoxelCoordinates(1, h, 0),
                           new GlobalVoxelCoordinates(2, h, 0),
                           new GlobalVoxelCoordinates(3, h, 0),
                           new GlobalVoxelCoordinates(4, h, 0),
                           new GlobalVoxelCoordinates(5, h, 0)
                       };

        Assert.AreEqual(expected.Length, path!.Count);

        for (var i = 0; i < path!.Count; i++)
        {
            Assert.AreEqual(expected[i], path![i]);
        }
    }

    [Test]
    public void TestAStarDiagonalPath()
    {
        var dimension = BuildDimension();
        var astar = new AStarPathFinder();
        var h = SurfaceHeight;

        var start = new GlobalVoxelCoordinates(0, h, 0);
        var end = new GlobalVoxelCoordinates(5, h, 5);

        var watch = new Stopwatch();
        watch.Start();
        var path = astar.FindPath(dimension, new BoundingBox(), start, end);
        watch.Stop();

        Assert.IsNotNull(path);
        DrawGrid(path!, dimension);
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");

        // Just test the start and end, the exact results need to be eyeballed
        Assert.AreEqual(start, path![0]);
        Assert.AreEqual(end, path![path.Count - 1]);
    }

    [Test]
    public void TestAStarObstacle()
    {
        var dimension = BuildDimension();
        var astar = new AStarPathFinder();
        var h = SurfaceHeight;

        var start = new GlobalVoxelCoordinates(0, h, 0);
        var end = new GlobalVoxelCoordinates(5, h, 0);

        // Obstacle
        dimension.SetBlockID(new GlobalVoxelCoordinates(3, h, 0), StoneBlockId);
        dimension.SetBlockID(new GlobalVoxelCoordinates(3, h + 1, 0), StoneBlockId);

        var watch = new Stopwatch();
        watch.Start();
        var path = astar.FindPath(dimension, new BoundingBox(), start, end);
        watch.Stop();

        Assert.IsNotNull(path);
        DrawGrid(path!, dimension);
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");

        // Just test the start and end, the exact results need to be eyeballed
        Assert.AreEqual(start, path![0]);
        Assert.AreEqual(end, path![path.Count - 1]);
        Assert.IsFalse(path!.Contains(new GlobalVoxelCoordinates(3, 4, 0)));
    }

    [Test]
    public void TestAStarImpossible()
    {
        var dimension = BuildDimension();
        var astar = new AStarPathFinder();
        var h = SurfaceHeight;

        var start = new GlobalVoxelCoordinates(0, h, 0);
        var end = new GlobalVoxelCoordinates(5, h, 0);

        dimension.SetBlockID(start + Vector3i.East, StoneBlockId);
        dimension.SetBlockID(start + Vector3i.East + Vector3i.Up, StoneBlockId);
        dimension.SetBlockID(start + Vector3i.West, StoneBlockId);
        dimension.SetBlockID(start + Vector3i.West + Vector3i.Up, StoneBlockId);
        dimension.SetBlockID(start + Vector3i.North, StoneBlockId);
        dimension.SetBlockID(start + Vector3i.North + Vector3i.Up, StoneBlockId);
        dimension.SetBlockID(start + Vector3i.South, StoneBlockId);
        dimension.SetBlockID(start + Vector3i.South + Vector3i.Up, StoneBlockId);

        var watch = new Stopwatch();
        watch.Start();
        var path = astar.FindPath(dimension, new BoundingBox(), start, end);
        watch.Stop();
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");

        Assert.IsNull(path);
    }

    [Test]
    public void TestAStarExitRoom()
    {
        var dimension = BuildDimension();
        var astar = new AStarPathFinder();
        var h = SurfaceHeight;

        var start = new GlobalVoxelCoordinates(0, h, 0);
        var end = new GlobalVoxelCoordinates(5, h, 0);

        // North wall
        for (var x = -4; x < 4; x++)
        {
            dimension.SetBlockID(new GlobalVoxelCoordinates(x, h, -4), StoneBlockId);
            dimension.SetBlockID(new GlobalVoxelCoordinates(x, h + 1, -4), StoneBlockId);
        }

        // East wall
        for (var z = -4; z < 4; z++)
        {
            dimension.SetBlockID(new GlobalVoxelCoordinates(3, h, z), StoneBlockId);
            dimension.SetBlockID(new GlobalVoxelCoordinates(3, h + 1, z), StoneBlockId);
        }

        // South wall
        for (var x = -4; x < 4; x++)
        {
            dimension.SetBlockID(new GlobalVoxelCoordinates(x, h, 4), StoneBlockId);
            dimension.SetBlockID(new GlobalVoxelCoordinates(x, h + 1, 4), StoneBlockId);
        }

        var watch = new Stopwatch();
        watch.Start();
        var path = astar.FindPath(dimension, new BoundingBox(), start, end);
        watch.Stop();

        Assert.IsNotNull(path);
        DrawGrid(path!, dimension);
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");

        // Just test the start and end, the exact results need to be eyeballed
        Assert.AreEqual(start, path![0]);
        Assert.AreEqual(end, path![path.Count - 1]);
    }

    [Test]
    public void TestAStarAvoidRoom()
    {
        var dimension = BuildDimension();
        var astar = new AStarPathFinder();
        var h = SurfaceHeight;

        var start = new GlobalVoxelCoordinates(-5, h, 0);
        var end = new GlobalVoxelCoordinates(5, h, 0);

        // North wall
        for (var x = -4; x < 4; x++)
        {
            dimension.SetBlockID(new GlobalVoxelCoordinates(x, h, -4), StoneBlockId);
            dimension.SetBlockID(new GlobalVoxelCoordinates(x, h + 1, -4), StoneBlockId);
        }

        // East wall
        for (var z = -4; z < 4; z++)
        {
            dimension.SetBlockID(new GlobalVoxelCoordinates(3, h, z), StoneBlockId);
            dimension.SetBlockID(new GlobalVoxelCoordinates(3, h + 1, z), StoneBlockId);
        }

        // South wall
        for (var x = -4; x < 4; x++)
        {
            dimension.SetBlockID(new GlobalVoxelCoordinates(x, h, 4), StoneBlockId);
            dimension.SetBlockID(new GlobalVoxelCoordinates(x, h + 1, 4), StoneBlockId);
        }

        var watch = new Stopwatch();
        watch.Start();
        var path = astar.FindPath(dimension, new BoundingBox(), start, end);
        watch.Stop();

        Assert.IsNotNull(path);
        DrawGrid(path!, dimension);
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");

        // Just test the start and end, the exact results need to be eyeballed
        Assert.AreEqual(start, path![0]);
        Assert.AreEqual(end, path![path!.Count - 1]);
    }
}