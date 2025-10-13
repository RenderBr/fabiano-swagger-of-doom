// Decompiled with JetBrains decompiler
// Type: RotMG.Common.FameStats
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using System;
using System.Collections.Generic;

#nullable disable
namespace RotMG.Common
{
  public class FameStats
  {
    private static readonly Dictionary<byte, Tuple<Func<FameStats, int>, Action<FameStats, int>>> statList = new Dictionary<byte, Tuple<Func<FameStats, int>, Action<FameStats, int>>>()
    {
      {
        (byte) 0,
        FameStats.Entry((Func<FameStats, int>) (s => s.Shots), (Action<FameStats, int>) ((s, val) => s.Shots = val))
      },
      {
        (byte) 1,
        FameStats.Entry((Func<FameStats, int>) (s => s.ShotsThatDamage), (Action<FameStats, int>) ((s, val) => s.ShotsThatDamage = val))
      },
      {
        (byte) 2,
        FameStats.Entry((Func<FameStats, int>) (s => s.SpecialAbilityUses), (Action<FameStats, int>) ((s, val) => s.SpecialAbilityUses = val))
      },
      {
        (byte) 3,
        FameStats.Entry((Func<FameStats, int>) (s => s.TilesUncovered), (Action<FameStats, int>) ((s, val) => s.TilesUncovered = val))
      },
      {
        (byte) 4,
        FameStats.Entry((Func<FameStats, int>) (s => s.Teleports), (Action<FameStats, int>) ((s, val) => s.Teleports = val))
      },
      {
        (byte) 5,
        FameStats.Entry((Func<FameStats, int>) (s => s.PotionsDrunk), (Action<FameStats, int>) ((s, val) => s.PotionsDrunk = val))
      },
      {
        (byte) 6,
        FameStats.Entry((Func<FameStats, int>) (s => s.MonsterKills), (Action<FameStats, int>) ((s, val) => s.MonsterKills = val))
      },
      {
        (byte) 7,
        FameStats.Entry((Func<FameStats, int>) (s => s.MonsterAssists), (Action<FameStats, int>) ((s, val) => s.MonsterAssists = val))
      },
      {
        (byte) 8,
        FameStats.Entry((Func<FameStats, int>) (s => s.GodKills), (Action<FameStats, int>) ((s, val) => s.GodKills = val))
      },
      {
        (byte) 9,
        FameStats.Entry((Func<FameStats, int>) (s => s.GodAssists), (Action<FameStats, int>) ((s, val) => s.GodAssists = val))
      },
      {
        (byte) 10,
        FameStats.Entry((Func<FameStats, int>) (s => s.CubeKills), (Action<FameStats, int>) ((s, val) => s.CubeKills = val))
      },
      {
        (byte) 11,
        FameStats.Entry((Func<FameStats, int>) (s => s.OryxKills), (Action<FameStats, int>) ((s, val) => s.OryxKills = val))
      },
      {
        (byte) 12,
        FameStats.Entry((Func<FameStats, int>) (s => s.QuestsCompleted), (Action<FameStats, int>) ((s, val) => s.QuestsCompleted = val))
      },
      {
        (byte) 13,
        FameStats.Entry((Func<FameStats, int>) (s => s.PirateCavesCompleted), (Action<FameStats, int>) ((s, val) => s.PirateCavesCompleted = val))
      },
      {
        (byte) 14,
        FameStats.Entry((Func<FameStats, int>) (s => s.UndeadLairsCompleted), (Action<FameStats, int>) ((s, val) => s.UndeadLairsCompleted = val))
      },
      {
        (byte) 15,
        FameStats.Entry((Func<FameStats, int>) (s => s.AbyssOfDemonsCompleted), (Action<FameStats, int>) ((s, val) => s.AbyssOfDemonsCompleted = val))
      },
      {
        (byte) 16,
        FameStats.Entry((Func<FameStats, int>) (s => s.SnakePitsCompleted), (Action<FameStats, int>) ((s, val) => s.SnakePitsCompleted = val))
      },
      {
        (byte) 17,
        FameStats.Entry((Func<FameStats, int>) (s => s.SpiderDensCompleted), (Action<FameStats, int>) ((s, val) => s.SpiderDensCompleted = val))
      },
      {
        (byte) 18,
        FameStats.Entry((Func<FameStats, int>) (s => s.SpriteWorldsCompleted), (Action<FameStats, int>) ((s, val) => s.SpriteWorldsCompleted = val))
      },
      {
        (byte) 19,
        FameStats.Entry((Func<FameStats, int>) (s => s.LevelUpAssists), (Action<FameStats, int>) ((s, val) => s.LevelUpAssists = val))
      },
      {
        (byte) 20,
        FameStats.Entry((Func<FameStats, int>) (s => s.MinutesActive), (Action<FameStats, int>) ((s, val) => s.MinutesActive = val))
      },
      {
        (byte) 21,
        FameStats.Entry((Func<FameStats, int>) (s => s.TombsCompleted), (Action<FameStats, int>) ((s, val) => s.TombsCompleted = val))
      },
      {
        (byte) 22,
        FameStats.Entry((Func<FameStats, int>) (s => s.TrenchesCompleted), (Action<FameStats, int>) ((s, val) => s.TrenchesCompleted = val))
      },
      {
        (byte) 23,
        FameStats.Entry((Func<FameStats, int>) (s => s.JunglesCompleted), (Action<FameStats, int>) ((s, val) => s.JunglesCompleted = val))
      },
      {
        (byte) 24,
        FameStats.Entry((Func<FameStats, int>) (s => s.ManorsCompleted), (Action<FameStats, int>) ((s, val) => s.ManorsCompleted = val))
      }
    };

    public int Shots { get; set; }

    public int ShotsThatDamage { get; set; }

    public int SpecialAbilityUses { get; set; }

    public int TilesUncovered { get; set; }

    public int Teleports { get; set; }

    public int PotionsDrunk { get; set; }

    public int MonsterKills { get; set; }

    public int MonsterAssists { get; set; }

    public int GodKills { get; set; }

    public int GodAssists { get; set; }

    public int CubeKills { get; set; }

    public int OryxKills { get; set; }

    public int QuestsCompleted { get; set; }

    public int PirateCavesCompleted { get; set; }

    public int UndeadLairsCompleted { get; set; }

    public int AbyssOfDemonsCompleted { get; set; }

    public int SnakePitsCompleted { get; set; }

    public int SpiderDensCompleted { get; set; }

    public int SpriteWorldsCompleted { get; set; }

    public int LevelUpAssists { get; set; }

    public int MinutesActive { get; set; }

    public int TombsCompleted { get; set; }

    public int TrenchesCompleted { get; set; }

    public int JunglesCompleted { get; set; }

    public int ManorsCompleted { get; set; }

    private static Tuple<Func<FameStats, int>, Action<FameStats, int>> Entry(
      Func<FameStats, int> getter,
      Action<FameStats, int> setter)
    {
      return Tuple.Create<Func<FameStats, int>, Action<FameStats, int>>(getter, setter);
    }

    public static FameStats Read(byte[] statsData)
    {
      FameStats fameStats = new FameStats();
      ByteBuffer byteBuffer = new ByteBuffer(statsData, 0, statsData.Length);
      while (!byteBuffer.IsEnd)
      {
        byte key = byteBuffer.ReadByte();
        FameStats.statList[key].Item2(fameStats, (int) byteBuffer.ReadUInt32());
      }
      return fameStats;
    }

    public byte[] Write()
    {
      int length = 0;
      foreach (KeyValuePair<byte, Tuple<Func<FameStats, int>, Action<FameStats, int>>> stat in FameStats.statList)
      {
        if (stat.Value.Item1(this) != 0)
          length += 5;
      }
      byte[] buffer = new byte[length];
      ByteBuffer byteBuffer = new ByteBuffer(buffer, 0, buffer.Length);
      foreach (KeyValuePair<byte, Tuple<Func<FameStats, int>, Action<FameStats, int>>> stat in FameStats.statList)
      {
        int num = stat.Value.Item1(this);
        if (num != 0)
        {
          byteBuffer.WriteByte(stat.Key);
          byteBuffer.WriteUInt32((uint) num);
        }
      }
      return buffer;
    }
  }
}
