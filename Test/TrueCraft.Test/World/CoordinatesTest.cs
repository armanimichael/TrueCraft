using System;
using NUnit.Framework;
using TrueCraft.Core.World;
using TrueCraft.World;

namespace TrueCraft.Test.World;

[TestFixture]
public class CoordinatesTest
{
    [TestCase(0, 0, 0, 0)] // origin maps to origin
    [TestCase(Chunk.Width - 1, 0, Chunk.Width - 1, 0)] // going east - last block in chunk 0, 0 maps to 15,0
    [TestCase(Chunk.Width, 0, 0, 0)] // going east - first block in chunk 1,0 maps to 0, 0
    [TestCase(0, Chunk.Depth - 1, 0, Chunk.Depth - 1)] // going south - last block in chunk 0,0 maps to 0,15
    [TestCase(0, Chunk.Depth, 0, 0)] // going south - first block in chunk 0,1 maps to 0,0
    [TestCase(-1, 0, Chunk.Width - 1, 0)] // going west - first block in chunk -1,0 maps to 15,0
    [TestCase(-Chunk.Width, 0, 0, 0)] // going west - last block in chunk -1,0 maps to 0, 0
    [TestCase(-Chunk.Width - 1, 0, Chunk.Width - 1, 0)] // going west - first block in chunk -2,0 maps to 15,0
    [TestCase(0, -1, 0, Chunk.Depth - 1)] // going north - first block in chunk 0,-1 maps to 0,15
    [TestCase(0, -Chunk.Depth, 0, 0)] // going north - last block in chunk 0,-1 maps to 0,0
    [TestCase(0, -Chunk.Width - 1, 0, Chunk.Width - 1)] // going north - first block in chunk 0,-2 mapsto 0,15
    public void GlobalBlockToLocalBlock(int globalX, int globalZ, int localX, int localZ)
    {
        var y = new Random().Next(0, Chunk.Height);
        var global = new GlobalVoxelCoordinates(globalX, y, globalZ);
        var expected = new LocalVoxelCoordinates(localX, y, localZ);
        LocalVoxelCoordinates actual;

        actual = (LocalVoxelCoordinates) global;

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.X, actual.X);
        Assert.AreEqual(expected.Y, actual.Y);
        Assert.AreEqual(expected.Z, actual.Z);
    }

    [TestCase(0, 0, 0, 0)] // origin maps to origin
    [TestCase(Region.Width - 1, 0, Region.Width - 1, 0)] // going east - last chunk in region 0, 0 maps to 31, 0
    [TestCase(Region.Width, 0, 0, 0)] // going east - first chunk in region 1, 0 maps to 0, 0
    [TestCase(0, Region.Depth - 1, 0, Region.Depth - 1)] // going south - last chunk in region 0, 0 maps to 0, 31
    [TestCase(0, Region.Depth, 0, 0)] // going south - first chunk in region 1, 0 maps to 0, 0
    [TestCase(-1, 0, Region.Width - 1, 0)] // going west - first chunk in region -1, 0 maps to 31, 0
    [TestCase(-Region.Width, 0, 0, 0)] // going west - last chunk in region -1, 0 maps to 0, 0
    [TestCase(-Region.Width - 1, 0, 31, 0)] // going west - first chunk in region -2, 0 maps to 31, 0
    [TestCase(0, -1, 0, Region.Depth - 1)] // going north - first chunk in region 0, -1 maps to 0, 31
    [TestCase(0, -Region.Depth, 0, 0)] // going north - last chunk in region 0, -1 maps to 0, 0
    [TestCase(0, -Region.Depth - 1, 0, Region.Depth - 1)] // going north - first chunk in region 0, -2 maps to 0, 31
    public void GlobalChunkToLocalChunk_Test(int globalX, int globalZ, int localX, int localZ)
    {
        var global = new GlobalChunkCoordinates(globalX, globalZ);
        var expected = new LocalChunkCoordinates(localX, localZ);
        LocalChunkCoordinates actual;

        actual = (LocalChunkCoordinates) global;

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.X, actual.X);
        Assert.AreEqual(expected.Z, actual.Z);
    }

    [TestCase(0, 0, 0, 0)] // origin maps to origin
    [TestCase(Region.Width - 1, 0, 0, 0)] // going east - last chunk in region 0, 0
    [TestCase(Region.Width, 0, 1, 0)] // going east - first chunk in region 1, 0
    [TestCase(-1, 0, -1, 0)] // going west - first chunk in region -1, 0
    [TestCase(-Region.Width, 0, -1, 0)] // going west - last chunk in region -1, 0
    [TestCase(-Region.Width - 1, 0, -2, 0)] // going west - first chunk in region -2, 0
    [TestCase(0, Region.Depth - 1, 0, 0)] // going south - last  chunk in region 0, 0
    [TestCase(0, Region.Depth, 0, 1)] // going south - first chunk in region 0, 1
    [TestCase(0, -Region.Depth, 0, -1)] // going north - last  chunk in region 0, -1
    [TestCase(0, -Region.Depth - 1, 0, -2)] // going north - first chunk in Region 0, -2
    public void GlobalChunkToRegion_Test(int globalX, int globalZ, int regionX, int regionZ)
    {
        var global = new GlobalChunkCoordinates(globalX, globalZ);
        var expected = new RegionCoordinates(regionX, regionZ);
        RegionCoordinates actual;

        actual = (RegionCoordinates) global;

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.X, actual.X);
        Assert.AreEqual(expected.Z, actual.Z);
    }

    [TestCase(0, 0, 0, 0)] // origin maps to origin
    [TestCase(Chunk.Width - 1, 0, 0, 0)] // going east - last block in chunk 0, 0
    [TestCase(Chunk.Width, 0, 1, 0)] // going east - first block in chunk 1, 0
    [TestCase(0, Chunk.Depth - 1, 0, 0)] // going south - last block in chunk 0, 0
    [TestCase(0, Chunk.Depth, 0, 1)] // going south - first block in chunk 0, 1
    [TestCase(-1, 0, -1, 0)] // going west - first block in chunk -1, 0
    [TestCase(-Chunk.Width, 0, -1, 0)] // going west - last block in chunk -1, 0
    [TestCase(-Chunk.Width - 1, 0, -2, 0)] // going west - first block in chunk -2, 0
    [TestCase(0, -1, 0, -1)] // going north - first block in chunk 0, -1
    [TestCase(0, -Chunk.Depth, 0, -1)] // going north - last block in chunk 0, -1
    [TestCase(0, -Chunk.Depth - 1, 0, -2)] // going north - first block in chunk 0, -2
    public void BlockToGlobalChunk_C2D_Test(int blockX, int blockZ, int globalX, int globalZ)
    {
        var global = new GlobalVoxelCoordinates(blockX, 0, blockZ);
        var expected = new GlobalChunkCoordinates(globalX, globalZ);

        var actual = (GlobalChunkCoordinates) global;

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.X, actual.X);
        Assert.AreEqual(expected.Z, actual.Z);
    }

    [TestCase(0, 0, 0, 0)] // origin maps to origin
    [TestCase((Region.Width * Chunk.Width) - 1, 0, Region.Width - 1, 0)] // going east - last block in region 0, 0
    [TestCase(Region.Width * Chunk.Width, 0, 0, 0)] // going east - first block in region 1, 0
    [TestCase(0, (Region.Width * Chunk.Depth) - 1, 0, Region.Depth - 1)] // going south - last block in region 0, 0
    [TestCase(0, Region.Width * Chunk.Depth, 0, 0)] // going south - first block in region 0, 1
    [TestCase(-1, 0, Region.Width - 1, 0)] // going west - first block in region -1, 0
    [TestCase(-Region.Width * Chunk.Width, 0, 0, 0)] // going west - last block in region -1, 0
    [TestCase((-Region.Width * Chunk.Width) - 1, 0, Region.Width - 1, 0)] // going west - first block in region -2, 0
    [TestCase(0, -1, 0, Region.Depth - 1)] // going north - first block in region 0, -1
    [TestCase(0, -Region.Width * Chunk.Depth, 0, 0)] // going north - last block in region 0, -1
    [TestCase(0, (-Region.Width * Chunk.Depth) - 1, 0, Region.Depth - 1)] // going north - first block in region 0, -2
    public void BlockToLocalChunk_Test(int blockX, int blockZ, int localX, int localZ)
    {
        var global = new GlobalVoxelCoordinates(blockX, 63, blockZ);
        var expected = new LocalChunkCoordinates(localX, localZ);
        LocalChunkCoordinates actual;

        actual = (LocalChunkCoordinates) global;

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.X, actual.X);
        Assert.AreEqual(expected.Z, actual.Z);
    }
}