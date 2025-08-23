using System;
using NUnit.Framework;
using TrueCraft.Core.Lighting;

namespace TrueCraft.Core.Test.Lighting;

public class TestBool3D
{
    [Test]
    public void ctor_true()
    {
        var xsize = 30;
        var ysize = 40;
        var zsize = 50;
        var tst = new Bool3D(xsize, ysize, zsize, true);

        for (var x = 0; x < xsize; x++)
        for (var y = 0; y < ysize; y++)
        for (var z = 0; z < zsize; z++)
        {
            Assert.True(tst[x, y, z]);
        }
    }

    [Test]
    public void ctor_false()
    {
        var xsize = 20;
        var ysize = 30;
        var zsize = 35;
        var tst = new Bool3D(xsize, ysize, zsize, false);

        for (var x = 0; x < xsize; x++)
        for (var y = 0; y < ysize; y++)
        for (var z = 0; z < zsize; z++)
        {
            Assert.False(tst[x, y, z]);
        }
    }

    [Test]
    public void ctor_size()
    {
        var xsize = 30;
        var ysize = 40;
        var zsize = 50;
        var tst = new Bool3D(xsize, ysize, zsize, true);

        Assert.AreEqual(xsize, tst.XSize);
        Assert.AreEqual(ysize, tst.YSize);
        Assert.AreEqual(zsize, tst.ZSize);
    }

    [Test]
    public void Indexer_Throws()
    {
        var xsize = 15;
        var ysize = 25;
        var zsize = 35;

        var tst = new Bool3D(xsize, ysize, zsize, false);

        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                var b = tst[-1, 0, 0];
            }
        );

        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                var b = tst[0, -1, 0];
            }
        );

        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                var b = tst[0, 0, -1];
            }
        );

        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                var b = tst[xsize, 0, 0];
            }
        );

        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                var b = tst[0, ysize, 0];
            }
        );

        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                var b = tst[0, 0, zsize];
            }
        );

        Assert.DoesNotThrow(
            () =>
            {
                var b = tst[0, 0, 0];
            }
        );

        Assert.DoesNotThrow(
            () =>
            {
                var b = tst[xsize - 1, ysize - 1, zsize - 1];
            }
        );
    }

    [Test]
    public void Indexer()
    {
        var xsize = 13;
        var ysize = 17;
        var zsize = 19;
        var rnd = new Random(123);

        // Test that a bit is successfully set and retrieved.
        // No other bits are altered.
        for (var j = 0; j < 50; j++)
        {
            var tst = new Bool3D(xsize, ysize, zsize, false);
            var x1 = rnd.Next(xsize);
            var y1 = rnd.Next(ysize);
            var z1 = rnd.Next(zsize);
            tst[x1, y1, z1] = true;

            for (var x = 0; x < xsize; x++)
            for (var y = 0; y < ysize; y++)
            for (var z = 0; z < zsize; z++)
            {
                Assert.AreEqual(x == x1 && y == y1 && z == z1, tst[x, y, z]);
            }
        }

        // Test that a bit is successfully cleared.
        // No other bits are altered.
        for (var j = 0; j < 50; j++)
        {
            var tst = new Bool3D(xsize, ysize, zsize, true);
            var x1 = rnd.Next(xsize);
            var y1 = rnd.Next(ysize);
            var z1 = rnd.Next(zsize);
            tst[x1, y1, z1] = false;

            for (var x = 0; x < xsize; x++)
            for (var y = 0; y < ysize; y++)
            for (var z = 0; z < zsize; z++)
            {
                Assert.AreEqual(x != x1 || y != y1 || z != z1, tst[x, y, z]);
            }
        }
    }
}