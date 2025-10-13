// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Data.Position
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Data
{
  [DataType]
  public struct Position
  {
    public float X;
    public float Y;

    public Position(float x, float y)
    {
      this.X = x;
      this.Y = y;
    }

    public bool CanInteract(float x, float y)
    {
      float num1 = this.X - x;
      float num2 = this.Y - y;
      return (double) num1 * (double) num1 + (double) num2 * (double) num2 < 1.0;
    }

    public static void InitIO()
    {
      DataTypeIO.Init<Position>((Action<InitDataTypeIO<Position>>) (init => init.Field<float>((Expression<Func<Position, float>>) (p => p.X), FieldType.Float32).Field<float>((Expression<Func<Position, float>>) (p => p.Y), FieldType.Float32).End()));
    }

    public override string ToString()
    {
      return string.Format("({0},{1})", (object) this.X, (object) this.Y);
    }
  }
}
