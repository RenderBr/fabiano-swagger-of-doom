// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.IO.IOHelper
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using System;
using System.Text;

#nullable disable
namespace RotMG.Common.Networking.IO
{
  internal static class IOHelper
  {
    public static void NextUTF(ref int index, string value)
    {
      int byteCount = Encoding.UTF8.GetByteCount(value);
      index += byteCount + 2;
    }

    public static void NextUTF32(ref int index, string value)
    {
      int byteCount = Encoding.UTF8.GetByteCount(value);
      index += byteCount + 4;
    }

    public static void NextUTF32Array(ref int index, string[] array)
    {
      index += 2;
      for (int index1 = 0; index1 < array.Length; ++index1)
      {
        int byteCount = Encoding.UTF8.GetByteCount(array[index1]);
        index += byteCount + 4;
      }
    }

    public static void NextIntArray(ref int index, int[] array) => index += 2 + (array.Length << 2);

    public static void NextBuffer16(ref int index, ArraySegment<byte> segment)
    {
      index += 2 + segment.Count;
    }

    public static T[] ReadDataArray<T>(ref ByteBuffer buffer)
    {
      T[] objArray = new T[(int) buffer.ReadUInt16()];
      for (int index = 0; index < objArray.Length; ++index)
        objArray[index] = DataTypeIO.Load<T>(ref buffer);
      return objArray;
    }

    public static void WriteDataArray<T>(ref ByteBuffer buffer, T[] array)
    {
      if (array.Length > (int) ushort.MaxValue)
        throw ByteBuffer.Overflow();
      buffer.WriteUInt16((ushort) array.Length);
      for (int index = 0; index < array.Length; ++index)
        DataTypeIO.Save<T>(ref buffer, array[index]);
    }

    public static int SizeOfDataArray<T>(T[] array)
    {
      int num = 2;
      for (int index = 0; index < array.Length; ++index)
        num += DataTypeIO.SizeOf<T>(array[index]);
      return num;
    }
  }
}
