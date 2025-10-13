// Decompiled with JetBrains decompiler
// Type: RotMG.Common.DictionaryAccessor`2
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System.Collections.Generic;

#nullable disable
namespace RotMG.Common
{
  public struct DictionaryAccessor<TKey, TValue>
  {
    private readonly Dictionary<TKey, TValue> dictionary;

    public DictionaryAccessor(Dictionary<TKey, TValue> dictionary) => this.dictionary = dictionary;

    public TValue this[TKey key] => this.dictionary[key];

    public bool TryGetValue(TKey key, out TValue value)
    {
      return this.dictionary.TryGetValue(key, out value);
    }
  }
}
