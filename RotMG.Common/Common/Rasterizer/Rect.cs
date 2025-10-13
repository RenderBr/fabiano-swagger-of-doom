// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Rasterizer.Rect
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;

#nullable disable
namespace RotMG.Common.Rasterizer
{
  public struct Rect
  {
    public static readonly Rect Empty = new Rect();
    public readonly int MaxX;
    public readonly int MaxY;
    public readonly int X;
    public readonly int Y;

    public Rect(int x, int y, int maxX, int maxY)
    {
      this.X = x;
      this.Y = y;
      this.MaxX = maxX < x ? x : maxX;
      this.MaxY = maxY < y ? y : maxY;
    }

    public Rect(double x, double y, double maxX, double maxY)
      : this((int) Math.Round(x), (int) Math.Round(x), (int) Math.Round(maxX), (int) Math.Round(maxY))
    {
    }

    public bool Contains(Point pt) => this.Contains(pt.X, pt.Y);

    public bool Contains(double x, double y)
    {
      return x >= (double) this.X && x < (double) this.MaxX && y >= (double) this.Y && y < (double) this.MaxY;
    }

    public bool Contains(int x, int y)
    {
      return x >= this.X && x < this.MaxX && y >= this.Y && y < this.MaxY;
    }

    public bool IsEmpty => this.X == this.MaxX || this.Y == this.MaxY;

    public Rect Intersection(Rect rect)
    {
      return new Rect(Math.Max(this.X, rect.X), Math.Max(this.Y, rect.Y), Math.Min(this.MaxX, rect.MaxX), Math.Min(this.MaxY, rect.MaxY));
    }

    public override string ToString()
    {
      return string.Format("({0}, {1}, {2}, {3})", (object) this.X, (object) this.MaxX, (object) this.Y, (object) this.MaxY);
    }
  }
}
