// Decompiled with JetBrains decompiler
// Type: RotMG.Common.DictionaryExtensions
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace RotMG.Common
{
  public static class DictionaryExtensions
  {
    public static TValue GetValueOrCreate<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      Func<TKey, TValue> creationFunc)
    {
      TValue valueOrCreate;
      if (!dictionary.TryGetValue(key, out valueOrCreate))
      {
        valueOrCreate = creationFunc(key);
        dictionary.Add(key, valueOrCreate);
      }
      return valueOrCreate;
    }

    public static TValue GetValueOrDefault<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue defaultValue)
    {
      return dictionary.GetValueOrDefault<TKey, TValue>(key, (Func<TKey, TValue>) (_ => defaultValue));
    }

    public static TValue GetValueOrDefault<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      Func<TKey, TValue> defaultFunc)
    {
      TValue obj;
      return !dictionary.TryGetValue(key, out obj) ? defaultFunc(key) : obj;
    }
  }
}
