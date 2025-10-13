// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketINVSWAP
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.Data;
using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketINVSWAP : Packet
  {
    public override PacketId Id => PacketId.INVSWAP;

    public int Time { get; set; }

    public Position Position { get; set; }

    public InventorySlot SlotA { get; set; }

    public InventorySlot SlotB { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketINVSWAP>((Action<InitDataTypeIO<PacketINVSWAP>>) (init => init.Field<int>((Expression<Func<PacketINVSWAP, int>>) (p => p.Time), FieldType.Int32).Field<Position>((Expression<Func<PacketINVSWAP, Position>>) (p => p.Position), FieldType.DataType).Field<InventorySlot>((Expression<Func<PacketINVSWAP, InventorySlot>>) (p => p.SlotA), FieldType.DataType).Field<InventorySlot>((Expression<Func<PacketINVSWAP, InventorySlot>>) (p => p.SlotB), FieldType.DataType).End()));
    }
  }
}
