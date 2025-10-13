// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketCREATE
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketCREATE : Packet
  {
    public override PacketId Id => PacketId.CREATE;

    public ushort ClassType { get; set; }

    public ushort SkinType { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketCREATE>((Action<InitDataTypeIO<PacketCREATE>>) (init => init.Field<ushort>((Expression<Func<PacketCREATE, ushort>>) (p => p.ClassType), FieldType.Int16).Field<ushort>((Expression<Func<PacketCREATE, ushort>>) (p => p.SkinType), FieldType.Int16).End()));
    }
  }
}
