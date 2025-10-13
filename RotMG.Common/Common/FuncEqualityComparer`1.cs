// Decompiled with JetBrains decompiler
// Type: RotMG.Common.FuncEqualityComparer`1
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace RotMG.Common
{
  public class FuncEqualityComparer<T> : IEqualityComparer<T>
  {
    private readonly Func<T, T, bool> comparison;
    private readonly Func<T, int> hashCode;

    public FuncEqualityComparer(Func<T, T, bool> comparison, Func<T, int> hashCode)
    {
      this.comparison = comparison;
      this.hashCode = hashCode;
    }

    public bool Equals(T x, T y) => this.comparison(x, y);

    public int GetHashCode(T obj) => this.hashCode(obj);
  }
}
