using System;
using System.Collections.Generic;
using System.Xml;
using TrueCraft.Core.Entities;
using TrueCraft.Core.Networking;
using TrueCraft.Core.Utility;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Logic;

public class ItemProvider : IItemProvider
{
    private readonly short _id;
    private readonly sbyte _maxStack;
    private readonly CacheEntry<Metadata> _metadata;

    // Parameterless constructor to support testing
    protected ItemProvider()
    {
        _id = 1;
        _maxStack = 64;

        var xml = @"<metadata>
          <value>0</value>
        <displayname>Snowball</displayname>
        <icontexture>
          <x>14</x>
          <y>0</y>
        </icontexture>
        </metadata>";

        var doc = new XmlDocument();

        using (var sr = new System.IO.StringReader(xml))
        using (var xmlr = XmlReader.Create(sr))
        {
            doc.Load(xmlr);
        }

        var md = new Metadata(doc.FirstChild!);
        _metadata = new CacheEntry<Metadata>(md, md.Key);
    }

    public ItemProvider(XmlNode item)
    {
        if (item.LocalName != "item")
        {
            throw new ArgumentException(nameof(item));
        }

        // <id> is the first child node.
        var node = item.ChildNodes[0];

        if (node is null || node.LocalName != "id")
        {
            throw new ArgumentException("Missing <id> node");
        }

        _id = short.Parse(node.InnerText);

        // <maximumstack> is the second child node.
        node = node.NextSibling;

        if (node is null || node.LocalName != "maximumstack")
        {
            throw new ArgumentException("Missing <maximumstack> node");
        }

        _maxStack = sbyte.Parse(node.InnerText);

        // <visiblemetadata> is the third child node.
        node = node.NextSibling;

        if (node is null || node.LocalName != "visiblemetadata")
        {
            throw new ArgumentException("Missing <visiblemetadata> node");
        }

        var metadataNode = node.FirstChild;

        if (metadataNode is null || metadataNode.LocalName != "metadata")
        {
            throw new ArgumentException("Missing <metadata> node");
        }

        var md = ParseMetadata(metadataNode);
        _metadata = new CacheEntry<Metadata>(md, md.Key);
        var last = _metadata;
        metadataNode = metadataNode.NextSibling;

        while (metadataNode is not null)
        {
            md = ParseMetadata(metadataNode);
            last.Append(md, md.Key);
            last = last.Next!;
            metadataNode = metadataNode.NextSibling;
        }
    }

    protected virtual Metadata ParseMetadata(XmlNode? node)
    {
        if (node is null || node.LocalName != "metadata")
        {
            throw new ArgumentException($"{nameof(node)} must be a <metadata> node.", nameof(node));
        }

        return new Metadata(node);
    }

    /// <inheritdoc />
    public short ID => _id;

    /// <inheritdoc />
    public virtual Tuple<int, int> GetIconTexture(byte metadata) => _metadata.Find(metadata).Value.IconTexture;

    /// <inheritdoc />
    public virtual sbyte MaximumStack => _maxStack;

    /// <inheritdoc />
    public virtual IEnumerable<short> VisibleMetadata
    {
        get
        {
            var last = _metadata;

            while (last is not null)
            {
                yield return last.Value.Key;
                last = last.Next;
            }
        }
    }

    /// <inheritdoc />
    public virtual string GetDisplayName(short metadata) => _metadata.Find(metadata).Value.DisplayName;

    public virtual void ItemUsedOnEntity(ItemStack item, IEntity usedOn, IDimension dimension, IRemoteClient user)
    {
        // This space intentionally left blank
    }

    public virtual void ItemUsedOnBlock(
        GlobalVoxelCoordinates coordinates,
        ItemStack item,
        BlockFace face,
        IDimension dimension,
        IRemoteClient user
    )
    {
        // This space intentionally left blank
    }

    public virtual void ItemUsedOnNothing(ItemStack item, IDimension dimension, IRemoteClient user)
    {
        // This space intentionally left blank
    }

    protected class Metadata
    {
        private readonly byte _metadata;
        private readonly string _displayName;
        private readonly Tuple<int, int> _iconTexture;

        public Metadata(XmlNode node)
        {
            if (node.LocalName != "metadata")
            {
                throw new ArgumentException($"{nameof(node)} must have a LocalName of metadata.", nameof(node));
            }

            var n = node.FirstChild;

            if (n is null || n.LocalName != "value")
            {
                throw new ArgumentException("Missing <value> node.");
            }

            _metadata = byte.Parse(n.InnerText);

            n = n.NextSibling;

            if (n is null || n.LocalName != "displayname")
            {
                throw new ArgumentException("Missing <displayname> node.");
            }

            _displayName = n.InnerText;

            n = n.NextSibling;

            if (n is null || n.LocalName != "icontexture")
            {
                throw new ArgumentException("Missing <icontexture> node.");
            }

            n = n.FirstChild;

            if (n is null || n.LocalName != "x")
            {
                throw new ArgumentException("icontexture is missing <x> node.");
            }

            var x = int.Parse(n.InnerText);

            n = n.NextSibling;

            if (n is null || n.LocalName != "y")
            {
                throw new ArgumentException("icontexture is missing <y> node.");
            }

            var y = int.Parse(n.InnerText);

            _iconTexture = new Tuple<int, int>(x, y);
        }

        public byte Key => _metadata;

        public string DisplayName => _displayName;

        public Tuple<int, int> IconTexture => _iconTexture;
    }
}