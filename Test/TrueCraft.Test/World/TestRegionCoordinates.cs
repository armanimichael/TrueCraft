using System;
using NUnit.Framework;
using TrueCraft.Core.World;
using TrueCraft.World;

namespace TrueCraft.Test.World;

[TestFixture]
public class TestRegionCoordinates
{
    private readonly Random _random = new(1234);

    [TestCase(2, 3)]
    public void Ctor(int x, int z)
    {
        var a = new RegionCoordinates(x, z);

        Assert.AreEqual(x, a.X);
        Assert.AreEqual(z, a.Z);
    }

    [Test]
    public void Equals_Object()
    {
        var a = new RegionCoordinates(2, 3);
        var b = new GlobalColumnCoordinates(2, 3);
        var c = new RegionCoordinates(a.X, a.Z);

        Assert.False(a.Equals(null));
        Assert.False(false);
        Assert.False(a != null && false);
        Assert.True(a != null && a.Equals(c));
    }

    [Test]
    public void Equals_operator()
    {
        var a = new RegionCoordinates(2, 3);
        var b = new RegionCoordinates(a.X, a.Z);

        Assert.False(ReferenceEquals(a, b));
        Assert.True(a == b);
        Assert.False(a != b);

        Assert.False(a == null);
        Assert.True(a != null);

        Assert.False(null == a);
        Assert.True(null != a);
    }

    [TestCase("<2,3>", 2, 3)]
    public void Test_ToString(string expected, int x, int z)
    {
        var a = new RegionCoordinates(x, z);

        var actual = a.ToString();

        Assert.AreEqual(expected, actual);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(-1, 0, -WorldConstants.RegionWidth, WorldConstants.RegionDepth - 1)]
    [TestCase(0, 0, WorldConstants.RegionWidth - 1, WorldConstants.RegionDepth - 1)]
    [TestCase(1, 1, WorldConstants.RegionWidth, WorldConstants.RegionDepth)]
    [TestCase(1, 1, (2 * WorldConstants.RegionWidth) - 1, (2 * WorldConstants.RegionDepth) - 1)]
    [TestCase(2, 2, 2 * WorldConstants.RegionWidth, 2 * WorldConstants.RegionDepth)]
    [TestCase(-1, -1, -1, -1)]
    [TestCase(-1, -1, -WorldConstants.RegionWidth, -WorldConstants.RegionDepth)]
    [TestCase(-2, -2, -WorldConstants.RegionWidth - 1, -WorldConstants.RegionDepth - 1)]
    public void Convert_From_GlobalChunkCoordinates(
        int regionX,
        int regionZ,
        int chunkX,
        int chunkZ
    )
    {
        var chunk = new GlobalChunkCoordinates(chunkX, chunkZ);

        var actual = (RegionCoordinates) chunk;

        Assert.AreEqual(regionX, actual.X);
        Assert.AreEqual(regionZ, actual.Z);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(-1, 0, -WorldConstants.RegionWidth * WorldConstants.ChunkDepth, 0)]
    [TestCase(
        0,
        0,
        (WorldConstants.RegionWidth * WorldConstants.ChunkWidth) - 1,
        (WorldConstants.RegionDepth * WorldConstants.ChunkDepth) - 1
    )]
    [TestCase(
        1,
        1,
        WorldConstants.RegionWidth * WorldConstants.ChunkWidth,
        WorldConstants.RegionDepth * WorldConstants.ChunkDepth
    )]
    [TestCase(
        1,
        1,
        (2 * WorldConstants.RegionWidth * WorldConstants.ChunkWidth) - 1,
        (2 * WorldConstants.RegionDepth * WorldConstants.ChunkDepth) - 1
    )]
    [TestCase(
        2,
        2,
        2 * WorldConstants.RegionWidth * WorldConstants.ChunkWidth,
        2 * WorldConstants.RegionDepth * WorldConstants.ChunkDepth
    )]
    [TestCase(-1, -1, -1, -1)]
    [TestCase(
        -1,
        -1,
        -WorldConstants.RegionWidth * WorldConstants.ChunkWidth,
        -WorldConstants.RegionDepth * WorldConstants.ChunkDepth
    )]
    [TestCase(
        -2,
        -2,
        (-WorldConstants.RegionWidth * WorldConstants.ChunkWidth) - 1,
        (-WorldConstants.RegionDepth * WorldConstants.ChunkDepth) - 1
    )]
    public void Convert_From_GlobalColumnCoordinates(
        int regionX,
        int regionZ,
        int voxelX,
        int voxelZ
    )
    {
        var coords = new GlobalColumnCoordinates(voxelX, voxelZ);

        var actual = (RegionCoordinates) coords;

        Assert.AreEqual(regionX, actual.X);
        Assert.AreEqual(regionZ, actual.Z);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(-1, 0, -WorldConstants.RegionWidth * WorldConstants.ChunkDepth, 0)]
    [TestCase(
        0,
        0,
        (WorldConstants.RegionWidth * WorldConstants.ChunkWidth) - 1,
        (WorldConstants.RegionDepth * WorldConstants.ChunkDepth) - 1
    )]
    [TestCase(
        1,
        1,
        WorldConstants.RegionWidth * WorldConstants.ChunkWidth,
        WorldConstants.RegionDepth * WorldConstants.ChunkDepth
    )]
    [TestCase(
        1,
        1,
        (2 * WorldConstants.RegionWidth * WorldConstants.ChunkWidth) - 1,
        (2 * WorldConstants.RegionDepth * WorldConstants.ChunkDepth) - 1
    )]
    [TestCase(
        2,
        2,
        2 * WorldConstants.RegionWidth * WorldConstants.ChunkWidth,
        2 * WorldConstants.RegionDepth * WorldConstants.ChunkDepth
    )]
    [TestCase(-1, -1, -1, -1)]
    [TestCase(
        -1,
        -1,
        -WorldConstants.RegionWidth * WorldConstants.ChunkWidth,
        -WorldConstants.RegionDepth * WorldConstants.ChunkDepth
    )]
    [TestCase(
        -2,
        -2,
        (-WorldConstants.RegionWidth * WorldConstants.ChunkWidth) - 1,
        (-WorldConstants.RegionDepth * WorldConstants.ChunkDepth) - 1
    )]
    public void Convert_From_GlobalVoxelCoordinates(
        int regionX,
        int regionZ,
        int voxelX,
        int voxelZ
    )
    {
        var coords = new GlobalVoxelCoordinates(voxelX, _random.Next(0, 127), voxelZ);

        var actual = (RegionCoordinates) coords;

        Assert.AreEqual(regionX, actual.X);
        Assert.AreEqual(regionZ, actual.Z);
    }

    [TestCase(0, 0, 0, 0, 0, 0)]
    [TestCase(0, 1, 0, 0, 0, 1)]
    [TestCase(32, 32, 1, 1, 0, 0)]
    [TestCase(63, 63, 1, 1, 31, 31)]
    [TestCase(-1, -1, -1, -1, 31, 31)]
    [TestCase(-32, -32, -1, -1, 0, 0)]
    public void GetGlobalChunkCoordinates(
        int expectedX,
        int expectedZ,
        int regionX,
        int regionZ,
        int localChunkX,
        int localChunkZ
    )
    {
        var region = new RegionCoordinates(regionX, regionZ);
        var local = new LocalChunkCoordinates(localChunkX, localChunkZ);

        var actual = region.GetGlobalChunkCoordinates(local);

        Assert.AreEqual(expectedX, actual.X);
        Assert.AreEqual(expectedZ, actual.Z);
    }
}