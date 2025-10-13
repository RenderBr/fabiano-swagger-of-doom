// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Data.InventorySlot
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
  public struct InventorySlot
  {
    public int ObjectId;
    public byte SlotId;
    public short ItemType;

    public static void InitIO()
    {
      DataTypeIO.Init<InventorySlot>((Action<InitDataTypeIO<InventorySlot>>) (init => init.Field<int>((Expression<Func<InventorySlot, int>>) (p => p.ObjectId), FieldType.Int32).Field<byte>((Expression<Func<InventorySlot, byte>>) (p => p.SlotId), FieldType.Byte).Field<short>((Expression<Func<InventorySlot, short>>) (p => p.ItemType), FieldType.Int16).End()));
    }
  }
}
