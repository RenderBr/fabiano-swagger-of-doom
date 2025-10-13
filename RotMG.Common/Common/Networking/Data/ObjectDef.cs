// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Data.ObjectDef
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
  public struct ObjectDef
  {
    public ushort ObjectType;
    public ObjectStats Stats;

    public static void InitIO()
    {
      DataTypeIO.Init<ObjectDef>((Action<InitDataTypeIO<ObjectDef>>) (init => init.Field<ushort>((Expression<Func<ObjectDef, ushort>>) (p => p.ObjectType), FieldType.Int16).Field<ObjectStats>((Expression<Func<ObjectDef, ObjectStats>>) (p => p.Stats), FieldType.DataType).End()));
    }
  }
}
