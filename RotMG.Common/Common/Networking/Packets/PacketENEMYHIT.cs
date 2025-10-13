// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketENEMYHIT
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketENEMYHIT : Packet
  {
    public override PacketId Id => PacketId.ENEMYHIT;

    public int Time { get; set; }

    public byte BulletId { get; set; }

    public int TargetId { get; set; }

    public bool Killed { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketENEMYHIT>((Action<InitDataTypeIO<PacketENEMYHIT>>) (init => init.Field<int>((Expression<Func<PacketENEMYHIT, int>>) (p => p.Time), FieldType.Int32).Field<byte>((Expression<Func<PacketENEMYHIT, byte>>) (p => p.BulletId), FieldType.Byte).Field<int>((Expression<Func<PacketENEMYHIT, int>>) (p => p.TargetId), FieldType.Int32).Field<bool>((Expression<Func<PacketENEMYHIT, bool>>) (p => p.Killed), FieldType.Bool).End()));
    }
  }
}
