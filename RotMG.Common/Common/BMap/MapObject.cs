// Decompiled with JetBrains decompiler
// Type: RotMG.Common.BMap.MapObject
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using System;
using System.Collections.Generic;

#nullable disable
namespace RotMG.Common.BMap
{
  public class MapObject
  {
    public KeyValuePair<string, string>[] Attributes;
    public uint ObjectType;

    internal void WriteTo(VarLenBinaryWriter writer, SerializationContext ctx)
    {
      writer.WriteVarLen32(ctx.GetObjectKey(this.ObjectType));
      writer.WriteVarLen32((uint) this.Attributes.Length);
      foreach (KeyValuePair<string, string> attribute in this.Attributes)
      {
        writer.Write(attribute.Key);
        writer.Write(attribute.Value);
      }
    }

    internal MapObject ReadFrom(VarLenBinaryReader reader, DeserializationContext ctx)
    {
      this.ObjectType = ctx.GetObjectId(reader.ReadVarLen32());
      uint length = reader.ReadVarLen32();
      this.Attributes = length == 0U ? Empty<KeyValuePair<string, string>>.Array : new KeyValuePair<string, string>[(IntPtr) length];
      for (int index = 0; index < this.Attributes.Length; ++index)
        this.Attributes[index] = new KeyValuePair<string, string>(reader.ReadString(), reader.ReadString());
      return this;
    }
  }
}
