// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketMOVE
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
  public class PacketMOVE : Packet
  {
    public override PacketId Id => PacketId.MOVE;

    public int TickId { get; set; }

    public int TickTime { get; set; }

    public Position NewPosition { get; set; }

    public TimedPosition[] PositionRecords { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketMOVE>((Action<InitDataTypeIO<PacketMOVE>>) (init => init.Field<int>((Expression<Func<PacketMOVE, int>>) (p => p.TickId), FieldType.Int32).Field<int>((Expression<Func<PacketMOVE, int>>) (p => p.TickTime), FieldType.Int32).Field<Position>((Expression<Func<PacketMOVE, Position>>) (p => p.NewPosition), FieldType.DataType).Field<TimedPosition[]>((Expression<Func<PacketMOVE, TimedPosition[]>>) (p => p.PositionRecords), FieldType.DataArray).End()));
    }
  }
}
