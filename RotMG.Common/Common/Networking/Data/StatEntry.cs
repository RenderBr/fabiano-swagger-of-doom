// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.Data.StatEntry
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using RotMG.Common.Networking.IO;
using RotMG.Data;

#nullable disable
namespace RotMG.Common.Networking.Data
{
  [DataType]
  public struct StatEntry
  {
    public StatData StatType;
    public int ValueInt;
    public string ValueString;

    public StatEntry(StatData stat, int value)
    {
      this.StatType = stat;
      this.ValueInt = value;
      this.ValueString = (string) null;
    }

    public StatEntry(StatData stat, string value)
    {
      this.StatType = stat;
      this.ValueInt = 0;
      this.ValueString = value;
    }

    public static bool IsString(StatData stat)
    {
      switch (stat)
      {
        case StatData.Name:
        case StatData.AccountId:
        case StatData.OwnerAccountId:
        case StatData.Guild:
        case StatData.PetName:
          return true;
        default:
          return false;
      }
    }

    public static StatEntry Load(ref ByteBuffer buffer)
    {
      StatEntry statEntry = new StatEntry();
      statEntry.StatType = (StatData) buffer.ReadByte();
      if (StatEntry.IsString(statEntry.StatType))
        statEntry.ValueString = buffer.ReadUTF();
      else
        statEntry.ValueInt = (int) buffer.ReadUInt32();
      return statEntry;
    }

    public static void Save(ref ByteBuffer buffer, StatEntry entry)
    {
      buffer.WriteByte((byte) entry.StatType);
      if (entry.ValueString != null)
        buffer.WriteUTF(entry.ValueString);
      else
        buffer.WriteUInt32((uint) entry.ValueInt);
    }

    public static int SizeOf(StatEntry entry)
    {
      if (entry.ValueString == null)
        return 5;
      int index = 1;
      IOHelper.NextUTF(ref index, entry.ValueString);
      return index;
    }
  }
}
