// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketUPDATE
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
  public class PacketUPDATE : Packet
  {
    public override PacketId Id => PacketId.UPDATE;

    public TileData[] Tiles { get; set; }

    public ObjectDef[] NewObjects { get; set; }

    public int[] RemovedObjectIds { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketUPDATE>((Action<InitDataTypeIO<PacketUPDATE>>) (init => init.Field<TileData[]>((Expression<Func<PacketUPDATE, TileData[]>>) (p => p.Tiles), FieldType.DataArray).Field<ObjectDef[]>((Expression<Func<PacketUPDATE, ObjectDef[]>>) (p => p.NewObjects), FieldType.DataArray).Field<int[]>((Expression<Func<PacketUPDATE, int[]>>) (p => p.RemovedObjectIds), FieldType.IntArray).End()));
    }
  }
}
