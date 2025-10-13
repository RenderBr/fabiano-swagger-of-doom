// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketMULTISHOOT
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
  public class PacketMULTISHOOT : Packet
  {
    public override PacketId Id => PacketId.MULTISHOOT;

    public byte BulletId { get; set; }

    public int OwnerId { get; set; }

    public byte BulletType { get; set; }

    public Position Position { get; set; }

    public float Angle { get; set; }

    public short Damage { get; set; }

    public byte NumShots { get; set; }

    public float AngleIncrement { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketMULTISHOOT>((Action<InitDataTypeIO<PacketMULTISHOOT>>) (init => init.Field<byte>((Expression<Func<PacketMULTISHOOT, byte>>) (p => p.BulletId), FieldType.Byte).Field<int>((Expression<Func<PacketMULTISHOOT, int>>) (p => p.OwnerId), FieldType.Int32).Field<byte>((Expression<Func<PacketMULTISHOOT, byte>>) (p => p.BulletType), FieldType.Byte).Field<Position>((Expression<Func<PacketMULTISHOOT, Position>>) (p => p.Position), FieldType.DataType).Field<float>((Expression<Func<PacketMULTISHOOT, float>>) (p => p.Angle), FieldType.Float32).Field<short>((Expression<Func<PacketMULTISHOOT, short>>) (p => p.Damage), FieldType.Int16).Field<byte>((Expression<Func<PacketMULTISHOOT, byte>>) (p => p.NumShots), FieldType.Byte).Field<float>((Expression<Func<PacketMULTISHOOT, float>>) (p => p.AngleIncrement), FieldType.Float32).End()));
    }
  }
}
