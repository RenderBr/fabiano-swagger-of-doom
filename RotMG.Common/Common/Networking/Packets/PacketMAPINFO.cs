// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketMAPINFO
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketMAPINFO : Packet
  {
    public override PacketId Id => PacketId.MAPINFO;

    public uint Width { get; set; }

    public uint Height { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public uint Seed { get; set; }

    public int Background { get; set; }

    public int Difficulty { get; set; }

    public bool AllowTeleport { get; set; }

    public bool ShowDisplays { get; set; }

    public string[] ClientXML { get; set; }

    public string[] ExtraXML { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketMAPINFO>((Action<InitDataTypeIO<PacketMAPINFO>>) (init => init.Field<uint>((Expression<Func<PacketMAPINFO, uint>>) (p => p.Width), FieldType.Int32).Field<uint>((Expression<Func<PacketMAPINFO, uint>>) (p => p.Height), FieldType.Int32).Field<string>((Expression<Func<PacketMAPINFO, string>>) (p => p.Name), FieldType.UTF).Field<string>((Expression<Func<PacketMAPINFO, string>>) (p => p.DisplayName), FieldType.UTF).Field<uint>((Expression<Func<PacketMAPINFO, uint>>) (p => p.Seed), FieldType.Int32).Field<int>((Expression<Func<PacketMAPINFO, int>>) (p => p.Background), FieldType.Int32).Field<int>((Expression<Func<PacketMAPINFO, int>>) (p => p.Difficulty), FieldType.Int32).Field<bool>((Expression<Func<PacketMAPINFO, bool>>) (p => p.AllowTeleport), FieldType.Bool).Field<bool>((Expression<Func<PacketMAPINFO, bool>>) (p => p.ShowDisplays), FieldType.Bool).Field<string[]>((Expression<Func<PacketMAPINFO, string[]>>) (p => p.ClientXML), FieldType.UTF32Array).Field<string[]>((Expression<Func<PacketMAPINFO, string[]>>) (p => p.ExtraXML), FieldType.UTF32Array).End()));
    }
  }
}
