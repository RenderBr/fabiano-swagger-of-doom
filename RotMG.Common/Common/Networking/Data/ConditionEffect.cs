// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Data.ConditionEffect
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using RotMG.Common.Networking.IO;

#nullable disable
namespace RotMG.Common.Networking.Data
{
  [DataType]
  public struct ConditionEffect
  {
    public uint Effect;

    public ConditionEffect(uint condEff) => this.Effect = condEff;

    private static uint NumberOfSetBits(uint i)
    {
      i -= i >> 1 & 1431655765U;
      i = (uint) (((int) i & 858993459) + ((int) (i >> 2) & 858993459));
      return (uint) (((int) i + (int) (i >> 4) & 252645135) * 16843009 >>> 24);
    }

    public static ConditionEffect Load(ref ByteBuffer buffer)
    {
      uint condEff = 0;
      for (byte index = buffer.ReadByte(); index > (byte) 0; --index)
        condEff |= 1U << (int) buffer.ReadByte();
      return new ConditionEffect(condEff);
    }

    public static void Save(ref ByteBuffer buffer, ConditionEffect condEff)
    {
      if (condEff.Effect == 0U)
      {
        buffer.WriteByte((byte) 0);
      }
      else
      {
        buffer.WriteByte((byte) ConditionEffect.NumberOfSetBits(condEff.Effect));
        uint effect = condEff.Effect;
        byte num = 0;
        for (; effect != 0U; effect >>= 1)
        {
          if (((int) effect & 1) != 0)
            buffer.WriteByte(num);
          ++num;
        }
      }
    }

    public static int SizeOf(ConditionEffect condEff)
    {
      return condEff.Effect == 0U ? 1 : 1 + (int) ConditionEffect.NumberOfSetBits(condEff.Effect);
    }
  }
}
