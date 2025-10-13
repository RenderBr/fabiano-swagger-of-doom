// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketSHOOTACK
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketSHOOTACK : Packet
  {
    public override PacketId Id => PacketId.SHOOTACK;

    public int Time { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketSHOOTACK>((Action<InitDataTypeIO<PacketSHOOTACK>>) (init => init.Field<int>((Expression<Func<PacketSHOOTACK, int>>) (p => p.Time), FieldType.Int32).End()));
    }
  }
}
