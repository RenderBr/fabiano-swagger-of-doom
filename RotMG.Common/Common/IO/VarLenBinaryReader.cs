// Decompiled with JetBrains decompiler
// Type: RotMG.Common.IO.VarLenBinaryReader
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System.IO;
using System.Text;

#nullable disable
namespace RotMG.Common.IO
{
  public class VarLenBinaryReader : BinaryReader
  {
    public VarLenBinaryReader(Stream stream)
      : base(stream)
    {
    }

    public VarLenBinaryReader(Stream stream, Encoding encoding)
      : base(stream, encoding)
    {
    }

    public uint ReadVarLen32()
    {
      byte num1 = this.ReadByte();
      uint num2 = (uint) num1 & (uint) sbyte.MaxValue;
      int num3 = 7;
      while (((int) num1 & 128) != 0)
      {
        num1 = this.ReadByte();
        num2 |= (uint) (((int) num1 & (int) sbyte.MaxValue) << num3);
        num3 += 7;
      }
      return num2;
    }
  }
}
