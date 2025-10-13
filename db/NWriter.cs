#region

using System;
using System.Buffers.Binary;
using System.IO;
using System.Net;
using System.Text;

#endregion

public class NWriter : BinaryWriter
{
    public NWriter(Stream s)
        : base(s, Encoding.UTF8)
    {
    }
    
    public override void Write(short value)
    {
        Span<byte> arr = stackalloc byte[2];
        BinaryPrimitives.WriteInt16BigEndian(arr, value);
        base.Write(arr);
    }
    
    public override void Write(int value)
    {
        Span<byte> arr = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(arr, value);
        base.Write(arr);
    }
    
    public override void Write(long value)
    {
        Span<byte> arr = stackalloc byte[8];
        BinaryPrimitives.WriteInt64BigEndian(arr, value);
        base.Write(arr);
    }
    
    public override void Write(ushort value)
    {
        Span<byte> arr = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(arr, value);
        base.Write(arr);
    }
    
    public override void Write(uint value)
    {
        Span<byte> arr = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(arr, value);
        base.Write(arr);
    }
    
    public override void Write(ulong value)
    {
        Span<byte> arr = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(arr, value);
        base.Write(arr);
    }
    
    public override void Write(float value)
    {
        byte[] arr = BitConverter.GetBytes(value);
        Array.Reverse(arr);
        base.Write(arr);
    }
    
    public override void Write(double value)
    {
        byte[] arr = BitConverter.GetBytes(value);
        Array.Reverse(arr);
        base.Write(arr);
    }
    
    public override void Write(bool value)
    {
        base.Write(value ? (byte) 1 : (byte) 0);
    }

    public void WriteUTF(string str)
    {
        if (str == null)
            Write((short)0);
        else
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            Write((short)bytes.Length);
            Write(bytes);
        }
    }

    public void Write32UTF(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        Write(bytes.Length);
        Write(bytes);
    }
}