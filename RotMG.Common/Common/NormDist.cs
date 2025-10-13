// Decompiled with JetBrains decompiler
// Type: RotMG.Common.NormDist
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;

#nullable disable
namespace RotMG.Common
{
  public class NormDist
  {
    private readonly float stdev;
    private readonly float mean;
    private readonly float max;
    private readonly float min;
    private readonly Random random;
    private double? nextNum;

    public NormDist(float stdev, float mean, float min, float max, int? seed = null)
    {
      this.stdev = stdev;
      this.mean = mean;
      this.min = min;
      this.max = max;
      if (seed.HasValue)
        this.random = new Random(seed.Value);
      else
        this.random = new Random();
    }

    private double NextValueInternal()
    {
      if (this.nextNum.HasValue)
      {
        double num = this.nextNum.Value;
        this.nextNum = new double?();
        return num;
      }
      double num1;
      double num2;
      double d;
      do
      {
        num1 = this.random.NextDouble() * 2.0 - 1.0;
        num2 = this.random.NextDouble() * 2.0 - 1.0;
        d = num1 * num1 + num2 * num2;
      }
      while (d >= 1.0);
      this.nextNum = new double?(num1 * Math.Sqrt(-2.0 * Math.Log(d) / d));
      return num2 * Math.Sqrt(-2.0 * Math.Log(d) / d);
    }

    public double NextValue()
    {
      double num = this.NextValueInternal() * (double) this.stdev + (double) this.mean;
      if (num < (double) this.min)
        num = (double) this.min;
      if (num > (double) this.max)
        num = (double) this.max;
      return num;
    }
  }
}
