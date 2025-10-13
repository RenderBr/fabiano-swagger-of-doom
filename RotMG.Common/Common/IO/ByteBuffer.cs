// Decompiled with JetBrains decompiler
// Type: RotMG.Common.IO.ByteBuffer
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace RotMG.Common.IO
{
  public struct ByteBuffer
  {
    private readonly byte[] buffer;
    private readonly int dataEnd;
    private readonly int length;
    private int index;

    public ByteBuffer(byte[] buffer, int index, int length)
    {
      this.buffer = buffer;
      this.index = index;
      this.length = length;
      this.dataEnd = index + length;
    }

    public byte[] Buffer => this.buffer;

    public int Index => this.index;

    public int Length => this.length;

    public bool IsEnd => this.index == this.dataEnd;

    internal static IndexOutOfRangeException Overflow()
    {
      throw new IndexOutOfRangeException("Buffer overflow.");
    }

    public byte ReadByte()
    {
      return this.index + 1 <= this.dataEnd ? this.buffer[this.index++] : throw ByteBuffer.Overflow();
    }

    public bool ReadBool() => this.ReadByte() != (byte) 0;

    public ushort ReadUInt16()
    {
      if (this.index + 2 > this.dataEnd)
        throw ByteBuffer.Overflow();
      return (ushort) ((int) this.buffer[this.index++] << 8 | (int) this.buffer[this.index++]);
    }

    public uint ReadUInt32()
    {
      if (this.index + 4 > this.dataEnd)
        throw ByteBuffer.Overflow();
      return (uint) ((int) this.buffer[this.index++] << 24 | (int) this.buffer[this.index++] << 16 | (int) this.buffer[this.index++] << 8) | (uint) this.buffer[this.index++];
    }

    public ulong ReadUInt64()
    {
      if (this.index + 8 > this.dataEnd)
        throw ByteBuffer.Overflow();
      return (ulong) ((int) this.buffer[this.index++] << 24 | (int) this.buffer[this.index++] << 16 | (int) this.buffer[this.index++] << 8 | (int) this.buffer[this.index++] | (int) this.buffer[this.index++] << 24 | (int) this.buffer[this.index++] << 16 | (int) this.buffer[this.index++] << 8 | (int) this.buffer[this.index++]);
    }

    public float ReadFloat32()
    {
      return new ByteBuffer.ConversionUnion()
      {
        UInt32 = this.ReadUInt32()
      }.Float32;
    }

    public double ReadFloat64()
    {
      return new ByteBuffer.ConversionUnion()
      {
        UInt64 = ((ulong) this.ReadUInt32())
      }.Float64;
    }

    public string ReadUTF()
    {
      ushort count = this.ReadUInt16();
      if (this.index + (int) count > this.dataEnd)
        throw ByteBuffer.Overflow();
      string str = Encoding.UTF8.GetString(this.buffer, this.index, (int) count);
      this.index += (int) count;
      return str;
    }

    public string ReadUTF32()
    {
      uint count = this.ReadUInt32();
      if ((long) this.index + (long) count > (long) this.dataEnd)
        throw ByteBuffer.Overflow();
      string str = Encoding.UTF8.GetString(this.buffer, this.index, (int) count);
      this.index += (int) count;
      return str;
    }

    public string[] ReadUTF32Array()
    {
      string[] strArray = new string[(int) this.ReadUInt16()];
      for (int index = 0; index < strArray.Length; ++index)
        strArray[index] = this.ReadUTF32();
      return strArray;
    }

    public ArraySegment<byte> ReadBuffer16()
    {
      ushort count = this.ReadUInt16();
      if (this.index + (int) count > this.dataEnd)
        throw ByteBuffer.Overflow();
      ArraySegment<byte> arraySegment = new ArraySegment<byte>(this.buffer, this.index, (int) count);
      this.index += (int) count;
      return arraySegment;
    }

    public int[] ReadIntArray()
    {
      ushort length = this.ReadUInt16();
      int[] numArray = new int[(int) length];
      for (int index = 0; index < (int) length; ++index)
        numArray[index] = (int) this.ReadUInt32();
      return numArray;
    }

    public void WriteByte(byte value)
    {
      this.buffer[this.index++] = this.index + 1 <= this.dataEnd ? value : throw ByteBuffer.Overflow();
    }

    public void WriteBool(bool value) => this.WriteByte(value ? (byte) 1 : (byte) 0);

    public void WriteUInt16(ushort value)
    {
      if (this.index + 2 > this.dataEnd)
        throw ByteBuffer.Overflow();
      this.buffer[this.index++] = (byte) ((uint) value >> 8);
      this.buffer[this.index++] = (byte) value;
    }

    public void WriteUInt32(uint value)
    {
      if (this.index + 4 > this.dataEnd)
        throw ByteBuffer.Overflow();
      this.buffer[this.index++] = (byte) (value >> 24);
      this.buffer[this.index++] = (byte) (value >> 16);
      this.buffer[this.index++] = (byte) (value >> 8);
      this.buffer[this.index++] = (byte) value;
    }

    public void WriteUInt64(ulong value)
    {
      if (this.index + 8 > this.dataEnd)
        throw ByteBuffer.Overflow();
      this.buffer[this.index++] = (byte) (value >> 56);
      this.buffer[this.index++] = (byte) (value >> 48);
      this.buffer[this.index++] = (byte) (value >> 40);
      this.buffer[this.index++] = (byte) (value >> 32);
      this.buffer[this.index++] = (byte) (value >> 24);
      this.buffer[this.index++] = (byte) (value >> 16);
      this.buffer[this.index++] = (byte) (value >> 8);
      this.buffer[this.index++] = (byte) value;
    }

    public void WriteFloat32(float value)
    {
      this.WriteUInt32(new ByteBuffer.ConversionUnion()
      {
        Float32 = value
      }.UInt32);
    }

    public void WriteFloat64(float value)
    {
      this.WriteUInt64(new ByteBuffer.ConversionUnion()
      {
        Float64 = ((double) value)
      }.UInt64);
    }

    public void WriteUTF(string value)
    {
      int byteCount = Encoding.UTF8.GetByteCount(value);
      if (this.index + 2 + byteCount > this.dataEnd || byteCount > (int) ushort.MaxValue)
        throw ByteBuffer.Overflow();
      this.WriteUInt16((ushort) byteCount);
      Encoding.UTF8.GetBytes(value, 0, value.Length, this.buffer, this.index);
      this.index += byteCount;
    }

    public void WriteUTF32(string value)
    {
      int byteCount = Encoding.UTF8.GetByteCount(value);
      if (this.index + 4 + byteCount > this.dataEnd)
        throw ByteBuffer.Overflow();
      this.WriteUInt32((uint) (ushort) byteCount);
      Encoding.UTF8.GetBytes(value, 0, value.Length, this.buffer, this.index);
      this.index += byteCount;
    }

    public void WriteUTF32Array(string[] array)
    {
      if (array.Length > (int) ushort.MaxValue)
        throw ByteBuffer.Overflow();
      this.WriteUInt16((ushort) array.Length);
      for (int index = 0; index < array.Length; ++index)
        this.WriteUTF32(array[index]);
    }

    public void WriteBuffer16(ArraySegment<byte> segment)
    {
      if (segment.Count > (int) ushort.MaxValue)
        throw ByteBuffer.Overflow();
      this.WriteUInt16((ushort) segment.Count);
      System.Buffer.BlockCopy((Array) segment.Array, segment.Offset, (Array) this.buffer, this.index, segment.Count);
      this.index += segment.Count;
    }

    public void WriteIntArray(int[] array)
    {
      if (array.Length > (int) ushort.MaxValue)
        throw ByteBuffer.Overflow();
      this.WriteUInt16((ushort) array.Length);
      for (int index = 0; index < array.Length; ++index)
        this.WriteUInt32((uint) array[index]);
    }

    public byte[] ToByteBuffer()
    {
      byte[] dst = new byte[this.dataEnd - this.index];
      System.Buffer.BlockCopy((Array) this.buffer, this.index, (Array) dst, 0, dst.Length);
      return dst;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct ConversionUnion
    {
      [FieldOffset(0)]
      public uint UInt32;
      [FieldOffset(0)]
      public ulong UInt64;
      [FieldOffset(0)]
      public float Float32;
      [FieldOffset(0)]
      public double Float64;
    }
  }
}
