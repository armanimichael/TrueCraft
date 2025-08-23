using TrueCraft.Core.World;
using NUnit.Framework;

namespace TrueCraft.Core.Test.World;

[TestFixture]
public class TestGlobalChunkCoordinates
{
    [TestCase(1, 2)]
    [TestCase(-2, -3)]
    public void ctor(int x, int z)
    {
        var actual = new GlobalChunkCoordinates(x, z);

        Assert.AreEqual(x, actual.X);
        Assert.AreEqual(z, actual.Z);
    }

    [Test]
    public void Test_Equals_Object()
    {
        var a = new GlobalChunkCoordinates(-1, 2);
        var b = new GlobalChunkCoordinates(a.X, a.Z);
        var c = new GlobalColumnCoordinates(a.X, a.Z);

        Assert.False(a.Equals(null));
        Assert.False(a!.Equals(a.ToString()));
        Assert.True(a.Equals((object) b));
        Assert.False(a.Equals(c));
    }

    [Test]
    public void Test_Equals_GlobalChunkCoordinates()
    {
        var a = new GlobalChunkCoordinates(-1, 2);
        var b = new GlobalChunkCoordinates(a.X, a.Z);

        Assert.True(a == b);
        Assert.False(a != b);

        Assert.False(a == null);
        Assert.True(a != null);
    }

    [TestCase("<-1,2>", -1, 2)]
    public void Test_ToString(string expected, int x, int z)
    {
        var a = new GlobalChunkCoordinates(x, z);

        var actual = a.ToString();

        Assert.AreEqual(expected, actual);
    }

    [TestCase(0, 0, 0, 0, 0)]
    [TestCase(0, 0, WorldConstants.ChunkWidth - 1, 1, WorldConstants.ChunkWidth - 1)]
    [TestCase(1, 0, WorldConstants.ChunkWidth, 60, WorldConstants.ChunkWidth - 1)]
    [TestCase(1, 1, WorldConstants.ChunkWidth, 127, WorldConstants.ChunkDepth)]
    [TestCase(1, 1, (2 * WorldConstants.ChunkWidth) - 1, 1, (2 * WorldConstants.ChunkDepth) - 1)]
    [TestCase(-1, -1, -1, 0, -1)]
    [TestCase(-1, -1, -WorldConstants.ChunkWidth, 5, -WorldConstants.ChunkDepth)]
    [TestCase(-2, -2, -WorldConstants.ChunkWidth - 1, 7, -WorldConstants.ChunkDepth - 1)]
    public void Test_Convert_From_GlobalVoxelCoordinates(
        int expectedX,
        int expectedZ,
        int voxelX,
        int voxelY,
        int voxelZ
    )
    {
        var other = new GlobalVoxelCoordinates(voxelX, voxelY, voxelZ);

        var actual = (GlobalChunkCoordinates) other;

        Assert.AreEqual(expectedX, actual.X);
        Assert.AreEqual(expectedZ, actual.Z);
    }

    [TestCase(0, 0, 0, 0, 0)]
    [TestCase(0, 0, WorldConstants.ChunkWidth - 0.001, 0, WorldConstants.ChunkWidth - 0.001)]
    [TestCase(1, 0, WorldConstants.ChunkWidth, 50, WorldConstants.ChunkWidth - 0.001)]
    [TestCase(1, 1, WorldConstants.ChunkWidth, 127, WorldConstants.ChunkDepth)]
    [TestCase(1, 1, (2 * WorldConstants.ChunkWidth) - 0.001, 5, (2 * WorldConstants.ChunkDepth) - 0.001)]
    [TestCase(2, 2, (2 * WorldConstants.ChunkWidth) + 0.001, 10, (2 * WorldConstants.ChunkDepth) + 0.001)]
    [TestCase(-1, -1, -0.001, 27, -0.001)]
    [TestCase(-1, -1, -WorldConstants.ChunkWidth + 0.001, 124, -WorldConstants.ChunkDepth + 0.001)]
    [TestCase(-2, -2, -WorldConstants.ChunkWidth - 0.001, 42, -WorldConstants.ChunkDepth - 0.001)]
    public void Test_Convert_From_Vector3(int expectedX, int expectedZ, double x, double y, double z)
    {
        var other = new Vector3(x, y, z);

        var actual = (GlobalChunkCoordinates) other;

        Assert.AreEqual(expectedX, actual.X);
        Assert.AreEqual(expectedZ, actual.Z);
    }
}