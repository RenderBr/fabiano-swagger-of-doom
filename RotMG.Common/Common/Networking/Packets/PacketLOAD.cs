// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketLOAD
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketLOAD : Packet
  {
    public override PacketId Id => PacketId.LOAD;

    public int CharId { get; set; }

    public bool IsFromArena { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketLOAD>((Action<InitDataTypeIO<PacketLOAD>>) (init => init.Field<int>((Expression<Func<PacketLOAD, int>>) (p => p.CharId), FieldType.Int32).Field<bool>((Expression<Func<PacketLOAD, bool>>) (p => p.IsFromArena), FieldType.Bool).End()));
    }
  }
}
