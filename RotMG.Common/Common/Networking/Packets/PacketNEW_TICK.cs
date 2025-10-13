// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketNEW_TICK
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
  public class PacketNEW_TICK : Packet
  {
    public override PacketId Id => PacketId.NEW_TICK;

    public int TickId { get; set; }

    public int TickTime { get; set; }

    public ObjectStats[] Statuses { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketNEW_TICK>((Action<InitDataTypeIO<PacketNEW_TICK>>) (init => init.Field<int>((Expression<Func<PacketNEW_TICK, int>>) (p => p.TickId), FieldType.Int32).Field<int>((Expression<Func<PacketNEW_TICK, int>>) (p => p.TickTime), FieldType.Int32).Field<ObjectStats[]>((Expression<Func<PacketNEW_TICK, ObjectStats[]>>) (p => p.Statuses), FieldType.DataArray).End()));
    }
  }
}
