using System.Text;

namespace fNbt;

/// <summary> BinaryReader wrapper that takes care of reading primitives from an NBT stream,
/// while taking care of endianness, string encoding, and skipping. </summary>
internal sealed class NbtBinaryReader : BinaryReader
{
    private readonly byte[] _floatBuffer = new byte[sizeof(float)],
        _doubleBuffer = new byte[sizeof(double)];

    private byte[] _seekBuffer;
    private const int _seekBufferSize = 64 * 1024;
    private readonly bool _bigEndian;

    public NbtBinaryReader(Stream input, bool bigEndian)
        : base(input)
    {
        this._bigEndian = bigEndian;
    }

    public NbtTagType ReadTagType()
    {
        var type = (NbtTagType) ReadByte();

        if (type < NbtTagType.End || type > NbtTagType.IntArray)
        {
            throw new NbtFormatException("NBT tag type out of range: " + (int) type);
        }

        return type;
    }

    public override short ReadInt16()
    {
        if (BitConverter.IsLittleEndian == _bigEndian)
        {
            return NbtBinaryWriter.Swap(base.ReadInt16());
        }
        else
        {
            return base.ReadInt16();
        }
    }

    public override int ReadInt32()
    {
        if (BitConverter.IsLittleEndian == _bigEndian)
        {
            return NbtBinaryWriter.Swap(base.ReadInt32());
        }
        else
        {
            return base.ReadInt32();
        }
    }

    public override long ReadInt64()
    {
        if (BitConverter.IsLittleEndian == _bigEndian)
        {
            return NbtBinaryWriter.Swap(base.ReadInt64());
        }
        else
        {
            return base.ReadInt64();
        }
    }

    public override float ReadSingle()
    {
        if (BitConverter.IsLittleEndian == _bigEndian)
        {
            BaseStream.Read(_floatBuffer, 0, sizeof(float));
            Array.Reverse(_floatBuffer);

            return BitConverter.ToSingle(_floatBuffer, 0);
        }

        return base.ReadSingle();
    }

    public override double ReadDouble()
    {
        if (BitConverter.IsLittleEndian == _bigEndian)
        {
            BaseStream.Read(_doubleBuffer, 0, sizeof(double));
            Array.Reverse(_doubleBuffer);

            return BitConverter.ToDouble(_doubleBuffer, 0);
        }

        return base.ReadDouble();
    }

    public override string ReadString()
    {
        var length = ReadInt16();

        if (length < 0)
        {
            throw new NbtFormatException("Negative string length given!");
        }

        var stringData = ReadBytes(length);

        return Encoding.UTF8.GetString(stringData);
    }

    public void Skip(int bytesToSkip)
    {
        if (bytesToSkip < 0)
        {
            throw new ArgumentOutOfRangeException("bytesToSkip");
        }
        else if (BaseStream.CanSeek)
        {
            BaseStream.Position += bytesToSkip;
        }
        else if (bytesToSkip != 0)
        {
            if (_seekBuffer == null)
            {
                _seekBuffer = new byte[_seekBufferSize];
            }

            var bytesDone = 0;

            while (bytesDone < bytesToSkip)
            {
                var readThisTime = BaseStream.Read(_seekBuffer, bytesDone, bytesToSkip - bytesDone);

                if (readThisTime == 0)
                {
                    throw new EndOfStreamException();
                }

                bytesDone += readThisTime;
            }
        }
    }

    public void SkipString()
    {
        var length = ReadInt16();

        if (length < 0)
        {
            throw new NbtFormatException("Negative string length given!");
        }

        Skip(length);
    }

    public TagSelector Selector { get; set; }
}