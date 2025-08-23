using TrueCraft.Core.Networking;

namespace TrueCraft.Core;

public class MetadataInt : MetadataEntry
{
    public override byte Identifier => 2;
    public override string FriendlyName => "int";

    public int Value;

    public static implicit operator MetadataInt(int value) => new(value);

    public MetadataInt() { }

    public MetadataInt(int value)
    {
        Value = value;
    }

    public override void FromStream(IMinecraftStream stream) => Value = stream.ReadInt32();

    public override void WriteTo(IMinecraftStream stream, byte index)
    {
        stream.WriteUInt8(GetKey(index));
        stream.WriteInt32(Value);
    }
}