// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketHELLO
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketHELLO : Packet
  {
    public override PacketId Id => PacketId.HELLO;

    public string BuildVersion { get; set; }

    public int GameId { get; set; }

    public string GUID { get; set; }

    public int Random1 { get; set; }

    public string Password { get; set; }

    public int Random2 { get; set; }

    public string Secret { get; set; }

    public int KeyTime { get; set; }

    public ArraySegment<byte> Key { get; set; }

    public string MapInfo { get; set; }

    public string EntryTag { get; set; }

    public string Constant1 { get; set; }

    public string Constant2 { get; set; }

    public string Constant3 { get; set; }

    public string PlayPlatform { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketHELLO>((Action<InitDataTypeIO<PacketHELLO>>) (init => init.Field<string>((Expression<Func<PacketHELLO, string>>) (p => p.BuildVersion), FieldType.UTF).Field<int>((Expression<Func<PacketHELLO, int>>) (p => p.GameId), FieldType.Int32).Field<string>((Expression<Func<PacketHELLO, string>>) (p => p.GUID), FieldType.UTF).Field<int>((Expression<Func<PacketHELLO, int>>) (p => p.Random1), FieldType.Int32).Field<string>((Expression<Func<PacketHELLO, string>>) (p => p.Password), FieldType.UTF).Field<int>((Expression<Func<PacketHELLO, int>>) (p => p.Random2), FieldType.Int32).Field<string>((Expression<Func<PacketHELLO, string>>) (p => p.Secret), FieldType.UTF).Field<int>((Expression<Func<PacketHELLO, int>>) (p => p.KeyTime), FieldType.Int32).Field<ArraySegment<byte>>((Expression<Func<PacketHELLO, ArraySegment<byte>>>) (p => p.Key), FieldType.Buffer16).Field<string>((Expression<Func<PacketHELLO, string>>) (p => p.MapInfo), FieldType.UTF32).Field<string>((Expression<Func<PacketHELLO, string>>) (p => p.EntryTag), FieldType.UTF).Field<string>((Expression<Func<PacketHELLO, string>>) (p => p.Constant1), FieldType.UTF).Field<string>((Expression<Func<PacketHELLO, string>>) (p => p.Constant2), FieldType.UTF).Field<string>((Expression<Func<PacketHELLO, string>>) (p => p.Constant3), FieldType.UTF).Field<string>((Expression<Func<PacketHELLO, string>>) (p => p.PlayPlatform), FieldType.UTF).End()));
    }
  }
}
