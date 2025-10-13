// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Data.TimedPosition
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
  public struct TimedPosition
  {
    public int Time;
    public float X;
    public float Y;

    public TimedPosition(int time, float x, float y)
    {
      this.Time = time;
      this.X = x;
      this.Y = y;
    }

    public TimedPosition(int time, Position pos)
    {
      this.Time = time;
      this.X = pos.X;
      this.Y = pos.Y;
    }

    public static void InitIO()
    {
      DataTypeIO.Init<TimedPosition>((Action<InitDataTypeIO<TimedPosition>>) (init => init.Field<int>((Expression<Func<TimedPosition, int>>) (p => p.Time), FieldType.Int32).Field<float>((Expression<Func<TimedPosition, float>>) (p => p.X), FieldType.Float32).Field<float>((Expression<Func<TimedPosition, float>>) (p => p.Y), FieldType.Float32).End()));
    }

    public override string ToString()
    {
      return string.Format("({0}: {1},{2})", (object) this.Time, (object) this.X, (object) this.Y);
    }
  }
}
