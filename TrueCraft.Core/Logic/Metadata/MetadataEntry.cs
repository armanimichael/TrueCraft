using TrueCraft.Core.Networking;

namespace TrueCraft.Core;

public abstract class MetadataEntry
{
    public abstract byte Identifier { get; }
    public abstract string FriendlyName { get; }

    public abstract void FromStream(IMinecraftStream stream);
    public abstract void WriteTo(IMinecraftStream stream, byte index);

    internal byte Index { get; set; }

    public static implicit operator MetadataEntry(byte value) => new MetadataByte(value);

    public static implicit operator MetadataEntry(short value) => new MetadataShort(value);

    public static implicit operator MetadataEntry(int value) => new MetadataInt(value);

    public static implicit operator MetadataEntry(float value) => new MetadataFloat(value);

    public static implicit operator MetadataEntry(string value) => new MetadataString(value);

    public static implicit operator MetadataEntry(ItemStack value) => new MetadataSlot(value);

    protected byte GetKey(byte index)
    {
        Index = index; // Cheat to get this for ToString

        return (byte) ((Identifier << 5) | (index & 0x1F));
    }

    public override string ToString()
    {
        var type = GetType();
        var fields = type.GetFields();
        var result = FriendlyName + "[" + Index + "]: ";

        if (fields.Length != 0)
        {
            result += fields[0].GetValue(this)?.ToString() ?? string.Empty;
        }

        return result;
    }
}