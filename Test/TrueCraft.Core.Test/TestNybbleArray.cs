using System;
using fNbt;
using NUnit.Framework;

namespace TrueCraft.Core.Test;

[TestFixture]
public class TestNybbleArray
{
    [Test]
    public void Ctor()
    {
        var data = new byte[1024];

        var actual = new NybbleArray(data, 0, data.Length);

        Assert.AreEqual(actual.Length, data.Length);
    }

    [Test]
    public void Indexer()
    {
        var data = new byte[512];
        var offset = 128;
        var length = 64;

        var actual = new NybbleArray(data, offset, length);

        for (int j = 0, jul = actual.Length; j < jul; j++)
        {
            actual[j] = (byte) (j & 0x0f);
        }

        for (int j = 0, jul = actual.Length; j < jul; j++)
        {
            Assert.AreEqual(j & 0x0f, actual[j]);
        }
    }

    [Test]
    public void Indexer_Throws()
    {
        var data = new byte[1024];
        var offset = 32;
        var length = 128;

        var actual = new NybbleArray(data, offset, length);

        // Getter
        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                var b = actual[-1];
            }
        );

        Assert.DoesNotThrow(
            () =>
            {
                var b = actual[0];
            }
        );

        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                var b = actual[actual.Length];
            }
        );

        Assert.DoesNotThrow(
            () =>
            {
                var b = actual[actual.Length - 1];
            }
        );

        // Setter
        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                actual[-1] = 0x0f;
            }
        );

        Assert.DoesNotThrow(
            () =>
            {
                actual[0] = 0x0e;
            }
        );

        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                actual[actual.Length] = 0x0d;
            }
        );

        Assert.DoesNotThrow(
            () =>
            {
                actual[actual.Length - 1] = 0x0c;
            }
        );
    }

    [Test]
    public void ToArray()
    {
        var data = new byte[1024];
        var offset = 17;
        var length = 64;

        for (var j = 0; j < data.Length; j++)
        {
            var v = j & 0x0f;
            v = (v << 4) | v;
            data[j] = (byte) v;
        }

        var actual = new NybbleArray(data, offset, 2 * length);

        var array = actual.ToArray();

        Assert.AreEqual(length, array.Length);

        for (var j = 0; j < array.Length; j++)
        {
            Assert.AreEqual(data[j + offset], array[j]);
        }
    }

    [Test]
    public void Serialize()
    {
        var data = new byte[512];
        var offset = 0;
        var length = data.Length;
        var tagName = "test";

        for (var k = 0; k < 2 * data.Length; k += 2)
        {
            data[k / 2] = (byte) ((k & 0x0f) | (((k + 1) & 0x0f) << 4));
        }

        var array = new NybbleArray(data, offset, 2 * length);

        var actual = array.Serialize(tagName);
        Assert.NotNull(actual);

        var byteArray = actual as NbtByteArray;
        Assert.NotNull(byteArray);

        Assert.AreEqual(tagName, byteArray?.Name);
        Assert.AreEqual(length, byteArray?.ByteArrayValue.Length);

        for (var j = 0; j < data.Length; j++)
        {
            Assert.AreEqual(data[j + offset], byteArray?.ByteArrayValue[j]);
        }
    }

    [Test]
    public void Deserialize()
    {
        var data = new byte[256];
        var length = data.Length;
        var tagName = "Fred";

        for (var j = 0; j < length; j++)
        {
            data[j] = (byte) (j & 0x00ff);
        }

        var byteArray = new NbtByteArray(tagName, data);

        var actual = new NybbleArray(new byte[1], 0, 0);
        actual.Deserialize(byteArray);

        Assert.AreEqual(2 * length, actual.Length);

        // Each j must be divided by two before calculating the
        // expected values because we are now indexing nybbles instead
        // of the bytes when we set this up.
        for (var j = 0; j < actual.Length; j += 2)
        {
            Assert.AreEqual((j >> 1) & 0x0f, actual[j]);
        }

        for (var j = 1; j < actual.Length; j += 2)
        {
            Assert.AreEqual(j >> 5, actual[j]);
        }
    }
}