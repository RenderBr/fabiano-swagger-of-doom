// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Rasterizer.Point
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;

#nullable disable
namespace RotMG.Common.Rasterizer
{
  public struct Point
  {
    public static readonly Point Zero = new Point();
    public readonly int X;
    public readonly int Y;

    public Point(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }

    public Point(double x, double y)
      : this((int) Math.Round(x), (int) Math.Round(y))
    {
    }

    public override string ToString()
    {
      return string.Format("({0}, {1})", (object) this.X, (object) this.Y);
    }
  }
}
