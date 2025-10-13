// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Data.ObjectStats
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
  public struct ObjectStats
  {
    public int Id;
    public Position Position;
    public StatEntry[] StatsEntries;

    public static void InitIO()
    {
      DataTypeIO.Init<ObjectStats>((Action<InitDataTypeIO<ObjectStats>>) (init => init.Field<int>((Expression<Func<ObjectStats, int>>) (p => p.Id), FieldType.Int32).Field<Position>((Expression<Func<ObjectStats, Position>>) (p => p.Position), FieldType.DataType).Field<StatEntry[]>((Expression<Func<ObjectStats, StatEntry[]>>) (p => p.StatsEntries), FieldType.DataArray).End()));
    }
  }
}
