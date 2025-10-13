// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Packet
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.Networking.IO;

#nullable disable
namespace RotMG.Common.Networking
{
  [DataType]
  public abstract class Packet
  {
    public abstract PacketId Id { get; }
  }
}
