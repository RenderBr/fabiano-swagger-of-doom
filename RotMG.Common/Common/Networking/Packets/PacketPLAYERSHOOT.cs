// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketPLAYERSHOOT
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
  public class PacketPLAYERSHOOT : Packet
  {
    public override PacketId Id => PacketId.PLAYERSHOOT;

    public int Time { get; set; }

    public byte BulletId { get; set; }

    public ushort ContainerType { get; set; }

    public Position Position { get; set; }

    public float Angle { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketPLAYERSHOOT>((Action<InitDataTypeIO<PacketPLAYERSHOOT>>) (init => init.Field<int>((Expression<Func<PacketPLAYERSHOOT, int>>) (p => p.Time), FieldType.Int32).Field<byte>((Expression<Func<PacketPLAYERSHOOT, byte>>) (p => p.BulletId), FieldType.Byte).Field<ushort>((Expression<Func<PacketPLAYERSHOOT, ushort>>) (p => p.ContainerType), FieldType.Int16).Field<Position>((Expression<Func<PacketPLAYERSHOOT, Position>>) (p => p.Position), FieldType.DataType).Field<float>((Expression<Func<PacketPLAYERSHOOT, float>>) (p => p.Angle), FieldType.Float32).End()));
    }
  }
}
