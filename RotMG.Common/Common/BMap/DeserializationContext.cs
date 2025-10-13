// Decompiled with JetBrains decompiler
// Type: RotMG.Common.BMap.DeserializationContext
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using System.Collections.Generic;

#nullable disable
namespace RotMG.Common.BMap
{
  internal class DeserializationContext
  {
    public readonly Dictionary<uint, uint> TileIdMap = new Dictionary<uint, uint>();
    public readonly Dictionary<uint, uint> ObjectIdMap = new Dictionary<uint, uint>();
    public readonly Dictionary<uint, string> RegionMap = new Dictionary<uint, string>();

    public uint GetTileId(uint tileKey) => this.TileIdMap[tileKey];

    public uint GetObjectId(uint objKey) => this.ObjectIdMap[objKey];

    public string GetRegionId(uint regionKey)
    {
      return regionKey == 0U ? (string) null : this.RegionMap[regionKey];
    }

    public void ReadFrom(VarLenBinaryReader reader)
    {
      uint num1 = reader.ReadVarLen32();
      for (int index = 0; (long) index < (long) num1; ++index)
        this.TileIdMap.Add(reader.ReadVarLen32(), reader.ReadVarLen32());
      uint num2 = reader.ReadVarLen32();
      for (int index = 0; (long) index < (long) num2; ++index)
        this.ObjectIdMap.Add(reader.ReadVarLen32(), reader.ReadVarLen32());
      uint num3 = reader.ReadVarLen32();
      for (int index = 0; (long) index < (long) num3; ++index)
        this.RegionMap.Add(reader.ReadVarLen32(), reader.ReadString());
    }
  }
}
