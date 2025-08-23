using System.Collections.Generic;
using System.Text;
using TrueCraft.Core.Networking;

namespace TrueCraft.Core;

/// <summary>
/// Used to send metadata with entities
/// </summary>
public class MetadataDictionary
{
    private readonly Dictionary<byte, MetadataEntry> entries;

    public MetadataDictionary()
    {
        entries = new Dictionary<byte, MetadataEntry>();
    }

    public int Count => entries.Count;

    public MetadataEntry this[byte index]
    {
        get => entries[index];
        set => entries[index] = value;
    }

    public static MetadataDictionary FromStream(IMinecraftStream stream)
    {
        var value = new MetadataDictionary();

        while (true)
        {
            var key = stream.ReadUInt8();

            if (key == 127)
            {
                break;
            }

            var type = (byte) ((key & 0xE0) >> 5);
            var index = (byte) (key & 0x1F);

            var entry = EntryTypes[type]();
            entry.FromStream(stream);
            entry.Index = index;

            value[index] = entry;
        }

        return value;
    }

    public void WriteTo(IMinecraftStream stream)
    {
        foreach (var entry in entries)
        {
            entry.Value.WriteTo(stream, entry.Key);
        }

        stream.WriteUInt8(0x7F);
    }

    private delegate MetadataEntry CreateEntryInstance();

    private static readonly CreateEntryInstance[] EntryTypes = new CreateEntryInstance[]
                                                               {
                                                                   () => new MetadataByte(), // 0
                                                                   () => new MetadataShort(), // 1
                                                                   () => new MetadataInt(), // 2
                                                                   () => new MetadataFloat(), // 3
                                                                   () => new MetadataString(), // 4
                                                                   () => new MetadataSlot() // 5
                                                               };

    public override string ToString()
    {
        if (entries.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        foreach (var entry in entries.Values)
        {
            sb.Append(entry.ToString());
            sb.Append(", ");
        }

        sb.Length -= 2;

        return sb.ToString();
    }
}