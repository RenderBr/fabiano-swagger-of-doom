// Decompiled with JetBrains decompiler
// Type: RotMG.Common.BMap.TileMap
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using System;
using System.IO;
using System.IO.Compression;

#nullable disable
namespace RotMG.Common.BMap
{
  public class TileMap
  {
    public const byte VERSION = 1;

    public TileMap(uint w, uint h)
    {
      this.Tiles = new MapTile[(int) (IntPtr) w, (int) (IntPtr) h];
      this.Width = w;
      this.Height = h;
    }

    public uint Width { get; private set; }

    public uint Height { get; private set; }

    public MapTile[,] Tiles { get; private set; }

    public void Save(Stream stream)
    {
      using (DeflateStream destination = new DeflateStream(stream, CompressionMode.Compress, true))
      {
        SerializationContext ctx = new SerializationContext();
        MemoryStream memoryStream = new MemoryStream();
        VarLenBinaryWriter writer1 = new VarLenBinaryWriter((Stream) memoryStream);
        uint width = this.Width;
        uint height = this.Height;
        for (int index1 = 0; (long) index1 < (long) height; ++index1)
        {
          for (int index2 = 0; (long) index2 < (long) width; ++index2)
            this.Tiles[index2, index1].WriteTo(writer1, ctx);
        }
        VarLenBinaryWriter writer2 = new VarLenBinaryWriter((Stream) destination);
        writer2.Write((byte) 1);
        writer2.WriteVarLen32(this.Width);
        writer2.WriteVarLen32(this.Height);
        ctx.WriteTo(writer2);
        memoryStream.Position = 0L;
        memoryStream.CopyTo((Stream) destination);
      }
    }

    public static TileMap Load(Stream stream)
    {
      using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true))
      {
        VarLenBinaryReader reader = new VarLenBinaryReader((Stream) deflateStream);
        TileMap tileMap = reader.ReadByte() == (byte) 1 ? new TileMap(reader.ReadVarLen32(), reader.ReadVarLen32()) : throw new InvalidDataException("Invalid BMap version.");
        DeserializationContext ctx = new DeserializationContext();
        ctx.ReadFrom(reader);
        uint width = tileMap.Width;
        uint height = tileMap.Height;
        MapTile[,] tiles = tileMap.Tiles;
        for (int index1 = 0; (long) index1 < (long) height; ++index1)
        {
          for (int index2 = 0; (long) index2 < (long) width; ++index2)
            MapTile.ReadFrom(ref tiles[index2,index1], reader, ctx);
        }
        return tileMap;
      }
    }
  }
}
