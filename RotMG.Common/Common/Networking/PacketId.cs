// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.PacketId
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

#nullable disable
namespace RotMG.Common.Networking
{
  public enum PacketId
  {
    INVALID,
    FAILURE,
    HELLO,
    MAPINFO,
    CREATE,
    LOAD,
    CREATE_SUCCESS,
    UPDATE,
    NEW_TICK,
    MOVE,
    UPDATEACK,
    INVDROP,
    INVSWAP,
    INVRESULT,
    PLAYERSHOOT,
    ALLYSHOOT,
    SHOOT,
    MULTISHOOT,
    SHOOTACK,
    ENEMYHIT,
    OTHERHIT,
    PLAYERHIT,
    SQUAREHIT,
    DAMAGE,
    TEXT,
    NOTIFICATION,
    QUESTOBJID,
  }
}
