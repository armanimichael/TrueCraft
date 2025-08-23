using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;

namespace TrueCraft.Core.Test.Assets;

public class TestXmlAssets
{
    [Test]
    public void TestXsdCompiles()
    {
        var doc = new XmlDocument();

        var thatAssembly = AppDomain.CurrentDomain.GetAssemblies()
                                    .Where(a => a.GetName().Name == "TrueCraft.Core")
                                    .First();

        using (var xsd = thatAssembly.GetManifestResourceStream("TrueCraft.Core.Assets.TrueCraft.xsd")!)
        {
            doc.Schemas.Add(XmlSchema.Read(xsd, null)!);
        }

        doc.Schemas.Compile();
    }

    [Test]
    public void TestTrueCraftXmlValid()
    {
        var doc = new XmlDocument();

        var thatAssembly = AppDomain.CurrentDomain.GetAssemblies()
                                    .Where(a => a.GetName().Name == "TrueCraft.Core")
                                    .First();

        using (var xsd = thatAssembly.GetManifestResourceStream("TrueCraft.Core.Assets.TrueCraft.xsd")!)
        {
            doc.Schemas.Add(XmlSchema.Read(xsd, null)!);
        }

        using (var sz = thatAssembly.GetManifestResourceStream("TrueCraft.Core.Assets.TrueCraft.xml.gz")!)
        using (Stream s = new GZipStream(sz, CompressionMode.Decompress))
        using (var xmlr = XmlReader.Create(s))
        {
            doc.Load(xmlr);
            doc.Validate(null);
        }
    }
}