// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketDAMAGE
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
  public class PacketDAMAGE : Packet
  {
    public override PacketId Id => PacketId.DAMAGE;

    public int TargetId { get; set; }

    public ConditionEffect Effects { get; set; }

    public ushort Amount { get; set; }

    public bool Killed { get; set; }

    public byte BulletId { get; set; }

    public int OwnerId { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketDAMAGE>((Action<InitDataTypeIO<PacketDAMAGE>>) (init => init.Field<int>((Expression<Func<PacketDAMAGE, int>>) (p => p.TargetId), FieldType.Int32).Field<ConditionEffect>((Expression<Func<PacketDAMAGE, ConditionEffect>>) (p => p.Effects), FieldType.DataType).Field<ushort>((Expression<Func<PacketDAMAGE, ushort>>) (p => p.Amount), FieldType.Int16).Field<bool>((Expression<Func<PacketDAMAGE, bool>>) (p => p.Killed), FieldType.Bool).Field<byte>((Expression<Func<PacketDAMAGE, byte>>) (p => p.BulletId), FieldType.Byte).Field<int>((Expression<Func<PacketDAMAGE, int>>) (p => p.OwnerId), FieldType.Int32).End()));
    }
  }
}
