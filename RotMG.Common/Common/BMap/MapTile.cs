// Decompiled with JetBrains decompiler
// Type: RotMG.Common.BMap.MapTile
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using System;

#nullable disable
namespace RotMG.Common.BMap
{
  public struct MapTile
  {
    public uint TileType;
    public MapBiome Biome;
    public float Elevation;
    public float Moisture;
    public MapObject[] Objects;
    public string Region;

    internal void WriteTo(VarLenBinaryWriter writer, SerializationContext ctx)
    {
      writer.WriteVarLen32(ctx.GetTileKey(this.TileType));
      writer.WriteVarLen32((uint) this.Biome);
      writer.Write(this.Elevation);
      writer.Write(this.Moisture);
      writer.WriteVarLen32((uint) this.Objects.Length);
      foreach (MapObject mapObject in this.Objects)
        mapObject.WriteTo(writer, ctx);
      writer.WriteVarLen32(ctx.GetRegionKey(this.Region));
    }

    internal static void ReadFrom(
      ref MapTile tile,
      VarLenBinaryReader reader,
      DeserializationContext ctx)
    {
      tile.TileType = ctx.GetTileId(reader.ReadVarLen32());
      tile.Biome = (MapBiome) reader.ReadVarLen32();
      tile.Elevation = reader.ReadSingle();
      tile.Moisture = reader.ReadSingle();
      uint length = reader.ReadVarLen32();
      tile.Objects = length == 0U ? Empty<MapObject>.Array : new MapObject[(IntPtr) length];
      for (int index = 0; index < tile.Objects.Length; ++index)
        tile.Objects[index] = new MapObject().ReadFrom(reader, ctx);
      tile.Region = ctx.GetRegionId(reader.ReadVarLen32());
    }
  }
}
