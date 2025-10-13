// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketALLYSHOOT
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketALLYSHOOT : Packet
  {
    public override PacketId Id => PacketId.ALLYSHOOT;

    public byte BulletId { get; set; }

    public int OwnerId { get; set; }

    public ushort ContainerType { get; set; }

    public float Angle { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketALLYSHOOT>((Action<InitDataTypeIO<PacketALLYSHOOT>>) (init => init.Field<byte>((Expression<Func<PacketALLYSHOOT, byte>>) (p => p.BulletId), FieldType.Byte).Field<int>((Expression<Func<PacketALLYSHOOT, int>>) (p => p.OwnerId), FieldType.Int32).Field<ushort>((Expression<Func<PacketALLYSHOOT, ushort>>) (p => p.ContainerType), FieldType.Int16).Field<float>((Expression<Func<PacketALLYSHOOT, float>>) (p => p.Angle), FieldType.Float32).End()));
    }
  }
}
