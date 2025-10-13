// Decompiled with JetBrains decompiler
// Type: RotMG.Common.SeededRandom
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

#nullable disable
namespace RotMG.Common
{
  public class SeededRandom
  {
    private uint state;

    public SeededRandom(uint seed) => this.state = seed;

    private uint NextState()
    {
      lock (this)
      {
        uint num1 = 16807U * (this.state >> 16);
        uint num2 = (uint) (16807 * ((int) this.state & (int) ushort.MaxValue)) + (uint) (((int) num1 & (int) short.MaxValue) << 16) + (num1 >> 15);
        if (((int) num2 & int.MinValue) != 0)
          num2 = (uint) ((int) num2 + 1 & int.MaxValue);
        this.state = num2;
        return this.state;
      }
    }

    public uint NextUInt32(uint loBound, uint hiBound)
    {
      return (int) hiBound == (int) loBound ? loBound : loBound + this.NextState() % (hiBound - loBound);
    }
  }
}
