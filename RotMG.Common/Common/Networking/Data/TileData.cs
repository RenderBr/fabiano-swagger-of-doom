// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Data.TileData
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
  public struct TileData
  {
    public short X;
    public short Y;
    public ushort TileType;

    public static void InitIO()
    {
      DataTypeIO.Init<TileData>((Action<InitDataTypeIO<TileData>>) (init => init.Field<short>((Expression<Func<TileData, short>>) (p => p.X), FieldType.Int16).Field<short>((Expression<Func<TileData, short>>) (p => p.Y), FieldType.Int16).Field<ushort>((Expression<Func<TileData, ushort>>) (p => p.TileType), FieldType.Int16).End()));
    }
  }
}
