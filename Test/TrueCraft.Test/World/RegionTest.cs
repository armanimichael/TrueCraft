using System.IO;
using System.Reflection;
using NUnit.Framework;
using TrueCraft.Core.World;
using TrueCraft.World;

namespace TrueCraft.Test.World;

[TestFixture]
public class RegionTest
{
    private Region? _region;

    [OneTimeSetUp]
    public void SetUp()
    {
        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        _region = new Region(
            RegionCoordinates.Zero,
            Path.Combine(assemblyDir, "Files")
        );
    }

    [Test]
    public void TestGetChunk()
    {
        var chunk = _region!.GetChunk(LocalChunkCoordinates.Zero);

        // No chunk was added, and Region must not generate chunks.
        Assert.IsNull(chunk);
    }

    [Test]
    public void TestGetRegionFileName() => Assert.AreEqual("r.0.0.mcr", Region.GetRegionFileName(_region!.Position));
}