// Decompiled with JetBrains decompiler
// Type: RotMG.Common.RandomExtensions
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace RotMG.Common
{
  public static class RandomExtensions
  {
    public static void Shuffle<T>(this Random random, IList<T> items)
    {
      for (int index1 = items.Count - 1; index1 > 0; --index1)
      {
        int index2 = random.Next(index1 + 1);
        T obj = items[index1];
        items[index1] = items[index2];
        items[index2] = obj;
      }
    }
  }
}
