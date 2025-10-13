#region

using System;
using System.Buffers.Binary;
using System.IO;
using System.Net;
using System.Text;

#endregion

public class NReader : BinaryReader
{
    public NReader(Stream s) : base(s, Encoding.UTF8)
    {
    }

    public override short ReadInt16()
    {
        return BinaryPrimitives.ReadInt16BigEndian(base.ReadBytes(2));
    }

    public override int ReadInt32()
    {
        return BinaryPrimitives.ReadInt32BigEndian(base.ReadBytes(4));
    }

    public override long ReadInt64()
    {
        return BinaryPrimitives.ReadInt64BigEndian(base.ReadBytes(8));
    }

    public override ushort ReadUInt16()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(base.ReadBytes(2));
    }

    public override uint ReadUInt32()
    {
        return BinaryPrimitives.ReadUInt32BigEndian(base.ReadBytes(4));
    }

    public override ulong ReadUInt64()
    {
        return BinaryPrimitives.ReadUInt64BigEndian(base.ReadBytes(8));
    }

    public override float ReadSingle()
    {
        byte[] arr = base.ReadBytes(4);
        Array.Reverse(arr);
        return BitConverter.ToSingle(arr, 0);
    }

    public override double ReadDouble()
    {
        byte[] arr = base.ReadBytes(8);
        Array.Reverse(arr);
        return BitConverter.ToDouble(arr, 0);
    }

    public string ReadNullTerminatedString()
    {
        StringBuilder ret = new StringBuilder();
        byte b = ReadByte();
        while (b != 0)
        {
            ret.Append((char)b);
            b = ReadByte();
        }

        return ret.ToString();
    }

    public string ReadUTF()
    {
        ushort len = ReadUInt16();           
        byte[] data = ReadBytes(len);        
        string s = Encoding.UTF8.GetString(data);

        return s;
    }


    public string Read32UTF()
    {
        return Encoding.UTF8.GetString(ReadBytes(ReadInt32()));
    }
}