// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketOTHERHIT
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketOTHERHIT : Packet
  {
    public override PacketId Id => PacketId.OTHERHIT;

    public int Time { get; set; }

    public byte BulletId { get; set; }

    public int ObjectId { get; set; }

    public int TargetId { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketOTHERHIT>((Action<InitDataTypeIO<PacketOTHERHIT>>) (init => init.Field<int>((Expression<Func<PacketOTHERHIT, int>>) (p => p.Time), FieldType.Int32).Field<byte>((Expression<Func<PacketOTHERHIT, byte>>) (p => p.BulletId), FieldType.Byte).Field<int>((Expression<Func<PacketOTHERHIT, int>>) (p => p.ObjectId), FieldType.Int32).Field<int>((Expression<Func<PacketOTHERHIT, int>>) (p => p.TargetId), FieldType.Int32).End()));
    }
  }
}
