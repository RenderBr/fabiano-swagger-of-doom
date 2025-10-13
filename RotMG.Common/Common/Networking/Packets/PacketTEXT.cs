// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketTEXT
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketTEXT : Packet
  {
    public override PacketId Id => PacketId.TEXT;

    public string Name { get; set; }

    public int ObjectId { get; set; }

    public int Stars { get; set; }

    public byte BubbleTime { get; set; }

    public string Recipient { get; set; }

    public string Text { get; set; }

    public string CleanText { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketTEXT>((Action<InitDataTypeIO<PacketTEXT>>) (init => init.Field<string>((Expression<Func<PacketTEXT, string>>) (p => p.Name), FieldType.UTF).Field<int>((Expression<Func<PacketTEXT, int>>) (p => p.ObjectId), FieldType.Int32).Field<int>((Expression<Func<PacketTEXT, int>>) (p => p.Stars), FieldType.Int32).Field<byte>((Expression<Func<PacketTEXT, byte>>) (p => p.BubbleTime), FieldType.Byte).Field<string>((Expression<Func<PacketTEXT, string>>) (p => p.Recipient), FieldType.UTF).Field<string>((Expression<Func<PacketTEXT, string>>) (p => p.Text), FieldType.UTF).Field<string>((Expression<Func<PacketTEXT, string>>) (p => p.CleanText), FieldType.UTF).End()));
    }
  }
}
