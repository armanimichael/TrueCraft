using System;
using TrueCraft.Core.Networking;

namespace TrueCraft.Core;

public class MetadataString : MetadataEntry
{
    public override byte Identifier => 4;
    public override string FriendlyName => "string";

    public string Value;

    public static implicit operator MetadataString(string value) => new(value);

    public MetadataString()
    {
        Value = string.Empty;
    }

    public MetadataString(string value)
    {
        if (value.Length > 16)
        {
            throw new ArgumentOutOfRangeException("value", "Maximum string length is 16 characters");
        }

        while (value.Length < 16)
        {
            value = value + "\0";
        }

        Value = value;
    }

    public override void FromStream(IMinecraftStream stream) => Value = stream.ReadString();

    public override void WriteTo(IMinecraftStream stream, byte index)
    {
        stream.WriteUInt8(GetKey(index));
        stream.WriteString(Value);
    }
}