// Decompiled with JetBrains decompiler
// Type: RotMG.Common.BMap.SerializationContext
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using System;
using System.Collections.Generic;

#nullable disable
namespace RotMG.Common.BMap
{
  internal class SerializationContext
  {
    public readonly Dictionary<uint, uint> TileIdMap = new Dictionary<uint, uint>();
    public readonly Dictionary<uint, uint> ObjectIdMap = new Dictionary<uint, uint>();
    public readonly Dictionary<string, uint> RegionMap = new Dictionary<string, uint>();

    public uint GetTileKey(uint tileId)
    {
      return this.TileIdMap.GetValueOrCreate<uint, uint>(tileId, (Func<uint, uint>) (_ => (uint) this.TileIdMap.Count));
    }

    public uint GetObjectKey(uint objId)
    {
      return this.ObjectIdMap.GetValueOrCreate<uint, uint>(objId, (Func<uint, uint>) (_ => (uint) this.ObjectIdMap.Count));
    }

    public uint GetRegionKey(string region)
    {
      return region == null ? 0U : this.RegionMap.GetValueOrCreate<string, uint>(region, (Func<string, uint>) (_ => (uint) (this.RegionMap.Count + 1)));
    }

    public void WriteTo(VarLenBinaryWriter writer)
    {
      writer.WriteVarLen32((uint) this.TileIdMap.Count);
      foreach (KeyValuePair<uint, uint> tileId in this.TileIdMap)
      {
        writer.WriteVarLen32(tileId.Value);
        writer.WriteVarLen32(tileId.Key);
      }
      writer.WriteVarLen32((uint) this.ObjectIdMap.Count);
      foreach (KeyValuePair<uint, uint> objectId in this.ObjectIdMap)
      {
        writer.WriteVarLen32(objectId.Value);
        writer.WriteVarLen32(objectId.Key);
      }
      writer.WriteVarLen32((uint) this.RegionMap.Count);
      foreach (KeyValuePair<string, uint> region in this.RegionMap)
      {
        writer.WriteVarLen32(region.Value);
        writer.Write(region.Key);
      }
    }
  }
}
