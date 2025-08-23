using System;
using TrueCraft.Core.World;
using NUnit.Framework;

namespace TrueCraft.Core.Test.World;

[TestFixture]
public class TestLocalColumnCoordinates
{
    [TestCase(1, 2)]
    public void LocalColumnCoordinates_ctor(int x, int z)
    {
        var actual = new LocalColumnCoordinates(x, z);

        Assert.AreEqual(x, actual.X);
        Assert.AreEqual(z, actual.Z);
    }

    [TestCase(true, 1, 2, 1, 2)]
    [TestCase(false, 2, 3, 3, 2)]
    public void LocalColumnCoordinates_Equals(bool expected, int x1, int z1, int x2, int z2)
    {
        var a = new LocalColumnCoordinates(x1, z1);
        var b = new LocalColumnCoordinates(x2, z2);

        var actual = a.Equals(b);

        Assert.AreEqual(expected, actual);
    }

    [TestCase(true, 1, 2, 1, 2)]
    [TestCase(false, 2, 3, 3, 2)]
    public void LocalColumnCoordinates_Equality_op(bool expected, int x1, int z1, int x2, int z2)
    {
        var a = new LocalColumnCoordinates(x1, z1);
        var b = new LocalColumnCoordinates(x2, z2);

        var actual = a == b;

        Assert.AreEqual(expected, actual);

        actual = a != b;
        Assert.AreEqual(!expected, actual);
    }

    [TestCase("<1,2>", 1, 2)]
    [TestCase("<5,3>", 5, 3)]
    public void LocalColumnCoordinates_ToString(string expected, int x, int z)
    {
        var a = new LocalColumnCoordinates(x, z);

        var actual = a.ToString();

        Assert.AreEqual(expected, actual);
    }

    [TestCase(1, 4, 4, 8)]
    public void LocalColumnCoordinates_DistanceTo(int x1, int z1, int x2, int z2)
    {
        var a = new LocalColumnCoordinates(x1, z1);
        var b = new LocalColumnCoordinates(x2, z2);

        var dx = x2 - x1;
        var dz = z2 - z1;
        var expected = Math.Sqrt((dx * dx) + (dz * dz));

        var actual = a.DistanceTo(b);
        var actual2 = b.DistanceTo(a);

        Assert.AreEqual(expected, actual);
        Assert.AreEqual(expected, actual2);
    }
}