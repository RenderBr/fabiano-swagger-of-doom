// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packets.PacketFAILURE
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;
using System;
using System.Linq.Expressions;

#nullable disable
namespace RotMG.Common.Networking.Packets
{
  public class PacketFAILURE : Packet
  {
    public const int Type1 = 4;
    public const int Type2 = 5;
    public const int Type3 = 6;
    public const int Type4 = 7;

    public override PacketId Id => PacketId.FAILURE;

    public int ErrorId { get; set; }

    public string ErrorDescription { get; set; }

    public static void InitIO()
    {
      DataTypeIO.Init<PacketFAILURE>((Action<InitDataTypeIO<PacketFAILURE>>) (init => init.Field<int>((Expression<Func<PacketFAILURE, int>>) (p => p.ErrorId), FieldType.Int32).Field<string>((Expression<Func<PacketFAILURE, string>>) (p => p.ErrorDescription), FieldType.UTF).End()));
    }
  }
}
