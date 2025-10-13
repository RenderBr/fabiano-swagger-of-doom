// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Rasterizer.BitmapRasterizer`1
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;

#nullable disable
namespace RotMG.Common.Rasterizer
{
  public class BitmapRasterizer<TPixel> where TPixel : struct
  {
    private const int SEG_COUNT = 10;
    private readonly TPixel[,] buffer;
    private readonly int height;
    private readonly int width;
    private readonly bool[][,] caps = new bool[5][,]
    {
      new bool[1, 1]{ { true } },
      new bool[2, 2]{ { true, true }, { true, true } },
      new bool[3, 3]
      {
        {
          false,
          true,
          false
        },
        {
          true,
          true,
          true
        },
        {
          false,
          true,
          false
        }
      },
      new bool[4, 4]
      {
        {
          false,
          true,
          true,
          false
        },
        {
          true,
          true,
          true,
          true
        },
        {
          true,
          true,
          true,
          true
        },
        {
          false,
          true,
          true,
          false
        }
      },
      new bool[5, 5]
      {
        {
          false,
          true,
          true,
          true,
          false
        },
        {
          true,
          true,
          true,
          true,
          true
        },
        {
          true,
          true,
          true,
          true,
          true
        },
        {
          true,
          true,
          true,
          true,
          true
        },
        {
          false,
          true,
          true,
          true,
          false
        }
      }
    };

    public BitmapRasterizer(int width, int height)
    {
      this.buffer = new TPixel[width, height];
      this.width = width;
      this.height = height;
    }

    public TPixel[,] Bitmap => this.buffer;

    public int Width => this.width;

    public int Height => this.height;

    public void Clear(TPixel bg)
    {
      for (int index1 = 0; index1 < this.height; ++index1)
      {
        for (int index2 = 0; index2 < this.width; ++index2)
          this.buffer[index2, index1] = bg;
      }
    }

    private void FillRectInternal(int minX, int minY, int maxX, int maxY, TPixel pix)
    {
      for (int index1 = minY; index1 < maxY; ++index1)
      {
        for (int index2 = minX; index2 < maxX; ++index2)
          this.buffer[index2, index1] = pix;
      }
    }

    private void FillRectInternal(
      int minX,
      int minY,
      int maxX,
      int maxY,
      Func<int, int, TPixel> texMapping)
    {
      for (int index1 = minY; index1 < maxY; ++index1)
      {
        for (int index2 = minX; index2 < maxX; ++index2)
          this.buffer[index2, index1] = texMapping(index2, index1);
      }
    }

    public void FillRect(int x, int y, int w, int h, TPixel pix)
    {
      this.FillRectInternal(x, y, x + w, y + h, pix);
    }

    public void FillRect(Rect rect, TPixel pix)
    {
      this.FillRectInternal(rect.X, rect.Y, rect.MaxX, rect.MaxY, pix);
    }

    private void ApplyCap(int x, int y, TPixel pix, int width)
    {
      if (width == 1)
        this.buffer[x, y] = pix;
      if (width <= 5)
      {
        bool[,] cap = this.caps[width - 1];
        x -= width >> 1;
        y -= width >> 1;
        for (int index1 = 0; index1 < width; ++index1)
        {
          for (int index2 = 0; index2 < width; ++index2)
          {
            if (cap[index2, index1])
              this.buffer[x + index2, y + index1] = pix;
          }
        }
      }
      else
      {
        int num = width >> 1;
        x -= num;
        y -= num;
        this.FillRectInternal(x, y, x + width, y + width, pix);
      }
    }

    private void ApplyCap(int x, int y, Func<int, int, TPixel> texMapping, int width)
    {
      if (width == 1)
        this.buffer[x, y] = texMapping(x, y);
      if (width <= 5)
      {
        bool[,] cap = this.caps[width - 1];
        x -= width >> 1;
        y -= width >> 1;
        for (int index1 = 0; index1 < width; ++index1)
        {
          for (int index2 = 0; index2 < width; ++index2)
          {
            if (cap[index2, index1])
              this.buffer[x + index2, y + index1] = texMapping(x + index2, y + index1);
          }
        }
      }
      else
      {
        int num = width >> 1;
        x -= num;
        y -= num;
        this.FillRectInternal(x, y, x + width, y + width, texMapping);
      }
    }

    public void DrawLine(Point a, Point b, TPixel pix, int width = 1)
    {
      this.DrawLine(a.X, a.Y, b.X, b.Y, pix, width);
    }

    public void DrawLine(int x0, int y0, int x1, int y1, TPixel pix, int width = 1)
    {
      int num1 = Math.Abs(x1 - x0);
      int num2 = Math.Abs(y1 - y0);
      int num3 = x0 < x1 ? 1 : -1;
      int num4 = y0 < y1 ? 1 : -1;
      int num5 = num1 - num2;
      while (x0 != x1 || y0 != y1)
      {
        this.ApplyCap(x0, y0, pix, width);
        if (2 * num5 > -num2)
        {
          num5 -= num2;
          x0 += num3;
        }
        else
        {
          num5 += num1;
          y0 += num4;
        }
      }
      this.ApplyCap(x0, y0, pix, width);
    }

    public void DrawLine(Point a, Point b, Func<int, int, TPixel> texMapping, int width = 1)
    {
      this.DrawLine(a.X, a.Y, b.X, b.Y, texMapping, width);
    }

    public void DrawLine(
      int x0,
      int y0,
      int x1,
      int y1,
      Func<int, int, TPixel> texMapping,
      int width = 1)
    {
      int num1 = Math.Abs(x1 - x0);
      int num2 = Math.Abs(y1 - y0);
      int num3 = x0 < x1 ? 1 : -1;
      int num4 = y0 < y1 ? 1 : -1;
      int num5 = num1 - num2;
      this.ApplyCap(x0, y0, texMapping, width);
      while (x0 != x1 || y0 != y1)
      {
        if (2 * num5 > -num2)
        {
          num5 -= num2;
          x0 += num3;
        }
        else
        {
          num5 += num1;
          y0 += num4;
        }
        this.ApplyCap(x0, y0, texMapping, width);
      }
    }

    private void ScanEdge(int x0, int y0, int x1, int y1, int?[] min, int?[] max)
    {
      int num1 = Math.Abs(x1 - x0);
      int num2 = Math.Abs(y1 - y0);
      int num3 = x0 < x1 ? 1 : -1;
      int num4 = y0 < y1 ? 1 : -1;
      int num5 = num1 - num2;
      if (min[y0].HasValue)
      {
        int? nullable = min[y0];
        int num6 = x0;
        if ((nullable.GetValueOrDefault() <= num6 ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
          goto label_3;
      }
      min[y0] = new int?(x0);
label_3:
      if (max[y0].HasValue)
      {
        int? nullable = max[y0];
        int num7 = x0;
        if ((nullable.GetValueOrDefault() >= num7 ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
          goto label_16;
      }
      max[y0] = new int?(x0);
label_16:
      while (x0 != x1 || y0 != y1)
      {
        int num8 = 2 * num5;
        if (num8 >= -num2)
        {
          num5 -= num2;
          x0 += num3;
        }
        if (num8 <= num1)
        {
          num5 += num1;
          y0 += num4;
        }
        if (min[y0].HasValue)
        {
          int? nullable = min[y0];
          int num9 = x0;
          if ((nullable.GetValueOrDefault() <= num9 ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
            goto label_13;
        }
        min[y0] = new int?(x0);
label_13:
        if (max[y0].HasValue)
        {
          int? nullable = max[y0];
          int num10 = x0;
          if ((nullable.GetValueOrDefault() >= num10 ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
            continue;
        }
        max[y0] = new int?(x0);
      }
    }

    public void FillTriangle(Point a, Point b, Point c, TPixel color)
    {
      int num1 = Math.Min(a.Y, Math.Min(b.Y, c.Y));
      int num2 = Math.Max(a.Y, Math.Max(b.Y, c.Y)) + 1;
      int length = num2 - num1;
      int?[] min = new int?[length];
      int?[] max = new int?[length];
      this.ScanEdge(a.X, a.Y - num1, b.X, b.Y - num1, min, max);
      this.ScanEdge(b.X, b.Y - num1, c.X, c.Y - num1, min, max);
      this.ScanEdge(c.X, c.Y - num1, a.X, a.Y - num1, min, max);
      for (int index1 = num1; index1 < num2; ++index1)
      {
        int num3 = min[index1 - num1].Value;
        int num4 = max[index1 - num1].Value;
        for (int index2 = num3; index2 <= num4; ++index2)
          this.buffer[index2, index1] = color;
      }
    }

    public void FillTriangle(Point a, Point b, Point c, Func<int, int, TPixel> texMapping)
    {
      int num1 = Math.Min(a.Y, Math.Min(b.Y, c.Y));
      int num2 = Math.Max(a.Y, Math.Max(b.Y, c.Y)) + 1;
      int length = num2 - num1;
      int?[] min = new int?[length];
      int?[] max = new int?[length];
      this.ScanEdge(a.X, a.Y - num1, b.X, b.Y - num1, min, max);
      this.ScanEdge(b.X, b.Y - num1, c.X, c.Y - num1, min, max);
      this.ScanEdge(c.X, c.Y - num1, a.X, a.Y - num1, min, max);
      for (int index1 = num1; index1 < num2; ++index1)
      {
        int num3 = min[index1 - num1].Value;
        int num4 = max[index1 - num1].Value;
        for (int index2 = num3; index2 <= num4; ++index2)
          this.buffer[index2, index1] = texMapping(index2, index1);
      }
    }

    public void DrawBezier(Point a, Point cp, Point b, TPixel pix, int width = 1)
    {
      this.DrawBezier(a.X, a.Y, cp.X, cp.Y, b.X, b.Y, pix, width);
    }

    public void DrawBezier(
      int x0,
      int y0,
      int x1,
      int y1,
      int x2,
      int y2,
      TPixel pix,
      int width = 1)
    {
      double x0_1 = (double) x0;
      double y0_1 = (double) y0;
      for (int index = 0; index < 10; ++index)
      {
        double num1 = (double) (index + 1) / 10.0;
        double num2 = 1.0 - num1;
        double x1_1 = num2 * num2 * (double) x0 + 2.0 * num2 * num1 * (double) x1 + num1 * num1 * (double) x2;
        double y1_1 = num2 * num2 * (double) y0 + 2.0 * num2 * num1 * (double) y1 + num1 * num1 * (double) y2;
        this.DrawLine((int) x0_1, (int) y0_1, (int) x1_1, (int) y1_1, pix, width);
        x0_1 = x1_1;
        y0_1 = y1_1;
      }
    }
  }
}
