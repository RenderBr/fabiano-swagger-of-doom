// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Utils
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;

#nullable disable
namespace RotMG.Common
{
  public static class Utils
  {
    public static void Swap<T>(ref T a, ref T b)
    {
      T obj = a;
      a = b;
      b = obj;
    }

    public static int ToUnixTimestamp(this DateTime dt)
    {
      return (int) (dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    public static bool IsValidEmail(string email)
    {
      try
      {
        return new MailAddress(email).Address == email;
      }
      catch
      {
        return false;
      }
    }

    public static T Convert<T>(this string value)
    {
      return (T) TypeDescriptor.GetConverter(typeof (T)).ConvertFrom((object) value);
    }

    public static T[] FromCSV<T>(this string csv)
    {
      return ((IEnumerable<string>) csv.Split(',')).Select<string, T>((Func<string, T>) (value => value.Trim().Convert<T>())).ToArray<T>();
    }
  }
}
