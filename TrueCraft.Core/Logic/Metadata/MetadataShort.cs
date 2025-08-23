using TrueCraft.Core.Networking;

namespace TrueCraft.Core;

public class MetadataShort : MetadataEntry
{
    public override byte Identifier => 1;
    public override string FriendlyName => "short";

    public short Value;

    public static implicit operator MetadataShort(short value) => new(value);

    public MetadataShort() { }

    public MetadataShort(short value)
    {
        Value = value;
    }

    public override void FromStream(IMinecraftStream stream) => Value = stream.ReadInt16();

    public override void WriteTo(IMinecraftStream stream, byte index)
    {
        stream.WriteUInt8(GetKey(index));
        stream.WriteInt16(Value);
    }
}