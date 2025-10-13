// Decompiled with JetBrains decompiler
// Type: RotMG.Common.IO.VarLenBinaryWriter
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System.IO;
using System.Text;

#nullable disable
namespace RotMG.Common.IO
{
  internal class VarLenBinaryWriter : BinaryWriter
  {
    public VarLenBinaryWriter(Stream stream)
      : base(stream)
    {
    }

    public VarLenBinaryWriter(Stream stream, Encoding encoding)
      : base(stream, encoding)
    {
    }

    public void WriteVarLen32(uint value)
    {
      uint num = value & (uint) sbyte.MaxValue;
      for (value >>= 7; value != 0U; value >>= 7)
      {
        this.Write((byte) (num | 128U));
        num = value & (uint) sbyte.MaxValue;
      }
      this.Write((byte) num);
    }
  }
}
