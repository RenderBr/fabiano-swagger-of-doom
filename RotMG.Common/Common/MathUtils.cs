// Decompiled with JetBrains decompiler
// Type: RotMG.Common.MathUtils
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;

#nullable disable
namespace RotMG.Common
{
  public static class MathUtils
  {
    public static float Lerp(float a, float b, float t)
    {
      return (float) ((1.0 - (double) t) * (double) a + (double) t * (double) b);
    }

    public static double Distance(double x1, double y1, double x2, double y2)
    {
      double num1 = x1 - x2;
      double num2 = y1 - y2;
      return Math.Sqrt(num1 * num1 + num2 * num2);
    }

    public static double DistanceSquared(double x1, double y1, double x2, double y2)
    {
      double num1 = x1 - x2;
      double num2 = y1 - y2;
      return num1 * num1 + num2 * num2;
    }
  }
}
