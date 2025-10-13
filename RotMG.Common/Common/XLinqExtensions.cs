// Decompiled with JetBrains decompiler
// Type: RotMG.Common.XLinqExtensions
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System.Xml.Linq;

#nullable disable
namespace RotMG.Common
{
  public static class XLinqExtensions
  {
    public static bool Exists(this XElement elem, string elemName)
    {
      return elem.Element((XName) elemName) != null;
    }

    public static T ValueOrDefault<T>(this XElement elem, T defValue)
    {
      return elem == null ? defValue : elem.Value.Convert<T>();
    }

    public static T ValueOrDefault<T>(this XAttribute attr, T defValue)
    {
      return attr == null ? defValue : attr.Value.Convert<T>();
    }
  }
}
