// Decompiled with JetBrains decompiler
// Type: RotMG.Common.SpatialNodePool`1
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System.Collections.Concurrent;

#nullable disable
namespace RotMG.Common
{
  public class SpatialNodePool<T> where T : class
  {
    private readonly ConcurrentBag<SpatialNode<T>> pool = new ConcurrentBag<SpatialNode<T>>();

    internal SpatialNode<T> Allocate()
    {
      SpatialNode<T> result;
      if (!this.pool.TryTake(out result))
        result = new SpatialNode<T>();
      return result;
    }

    internal void Release(SpatialNode<T> node)
    {
      node.Item = default (T);
      node.Hash = 0U;
      node.Storage = (SpatialStorage<T>) null;
      node.Next = (SpatialNode<T>) null;
      node.Previous = (SpatialNode<T>) null;
      this.pool.Add(node);
    }
  }
}
