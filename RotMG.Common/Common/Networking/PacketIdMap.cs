// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.PacketIdMap
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

#nullable disable
namespace RotMG.Common.Networking
{
  public static class PacketIdMap
  {
    public static readonly PacketId?[] Map = new PacketId?[256];
    public static readonly byte?[] ReverseMap = new byte?[256];

    static PacketIdMap()
    {
      PacketIdMap.RegisterMap((byte) 0, PacketId.FAILURE);
      PacketIdMap.RegisterMap((byte) 91, PacketId.HELLO);
      PacketIdMap.RegisterMap((byte) 7, PacketId.MAPINFO);
      PacketIdMap.RegisterMap((byte) 41, PacketId.CREATE);
      PacketIdMap.RegisterMap((byte) 63, PacketId.LOAD);
      PacketIdMap.RegisterMap((byte) 84, PacketId.CREATE_SUCCESS);
      PacketIdMap.RegisterMap((byte) 67, PacketId.UPDATE);
      PacketIdMap.RegisterMap((byte) 34, PacketId.NEW_TICK);
      PacketIdMap.RegisterMap((byte) 92, PacketId.MOVE);
      PacketIdMap.RegisterMap((byte) 17, PacketId.UPDATEACK);
      PacketIdMap.RegisterMap((byte) 74, PacketId.INVDROP);
      PacketIdMap.RegisterMap((byte) 5, PacketId.INVSWAP);
      PacketIdMap.RegisterMap((byte) 89, PacketId.INVRESULT);
      PacketIdMap.RegisterMap((byte) 11, PacketId.PLAYERSHOOT);
      PacketIdMap.RegisterMap((byte) 94, PacketId.ALLYSHOOT);
      PacketIdMap.RegisterMap((byte) 37, PacketId.SHOOT);
      PacketIdMap.RegisterMap((byte) 93, PacketId.MULTISHOOT);
      PacketIdMap.RegisterMap((byte) 88, PacketId.SHOOTACK);
      PacketIdMap.RegisterMap((byte) 15, PacketId.ENEMYHIT);
      PacketIdMap.RegisterMap((byte) 69, PacketId.OTHERHIT);
      PacketIdMap.RegisterMap((byte) 86, PacketId.PLAYERHIT);
      PacketIdMap.RegisterMap((byte) 14, PacketId.SQUAREHIT);
      PacketIdMap.RegisterMap((byte) 19, PacketId.DAMAGE);
      PacketIdMap.RegisterMap((byte) 38, PacketId.TEXT);
      PacketIdMap.RegisterMap((byte) 36, PacketId.NOTIFICATION);
      PacketIdMap.RegisterMap((byte) 6, PacketId.QUESTOBJID);
    }

    private static void RegisterMap(byte rawId, PacketId id)
    {
      PacketIdMap.Map[(int) rawId] = new PacketId?(id);
      PacketIdMap.ReverseMap[(int) (byte) id] = new byte?(rawId);
    }
  }
}
