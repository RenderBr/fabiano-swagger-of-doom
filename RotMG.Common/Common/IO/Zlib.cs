// Decompiled with JetBrains decompiler
// Type: RotMG.Common.IO.Zlib
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;
using System.IO;
using System.IO.Compression;

#nullable disable
namespace RotMG.Common.IO
{
  public static class Zlib
  {
    private static uint ADLER32(byte[] data)
    {
      uint num1 = 1;
      uint num2 = 0;
      for (int index = 0; index < data.Length; ++index)
      {
        num1 = (num1 + (uint) data[index]) % 65521U;
        num2 = (num2 + num1) % 65521U;
      }
      return num2 << 16 | num1;
    }

    public static byte[] Decompress(byte[] buffer)
    {
      byte num1 = buffer.Length >= 6 ? buffer[0] : throw new ArgumentException("Invalid ZLIB buffer.");
      byte num2 = buffer[1];
      byte num3 = (byte) ((uint) num1 & 15U);
      byte num4 = (byte) ((uint) num1 >> 4);
      if (num3 != (byte) 8)
        throw new NotSupportedException("Invalid compression method.");
      if (num4 != (byte) 7)
        throw new NotSupportedException("Unsupported window size.");
      if (((int) num2 & 32) != 0)
        throw new NotSupportedException("Preset dictionary not supported.");
      if ((((int) num1 << 8) + (int) num2) % 31 != 0)
        throw new InvalidDataException("Invalid header checksum");
      MemoryStream memoryStream = new MemoryStream(buffer, 2, buffer.Length - 6);
      MemoryStream destination = new MemoryStream();
      using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream, CompressionMode.Decompress))
        deflateStream.CopyTo((Stream) destination);
      byte[] array = destination.ToArray();
      int num5 = buffer.Length - 4;
      byte[] numArray1 = buffer;
      int index1 = num5;
      int num6 = index1 + 1;
      int num7 = (int) numArray1[index1] << 24;
      byte[] numArray2 = buffer;
      int index2 = num6;
      int num8 = index2 + 1;
      int num9 = (int) numArray2[index2] << 16;
      int num10 = num7 | num9;
      byte[] numArray3 = buffer;
      int index3 = num8;
      int num11 = index3 + 1;
      int num12 = (int) numArray3[index3] << 8;
      int num13 = num10 | num12;
      byte[] numArray4 = buffer;
      int index4 = num11;
      int num14 = index4 + 1;
      int num15 = (int) numArray4[index4];
      if ((num13 | num15) != (int) Zlib.ADLER32(array))
        throw new InvalidDataException("Invalid data checksum");
      return array;
    }
  }
}
