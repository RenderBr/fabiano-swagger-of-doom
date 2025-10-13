// Decompiled with JetBrains decompiler
// Type: RotMG.Common.SpatialStorage`1
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace RotMG.Common
{
  public class SpatialStorage<T> : IEnumerable<T>, IEnumerable where T : class
  {
    internal SpatialNodePool<T> pool;
    internal HashSet<T> items = new HashSet<T>();
    internal Dictionary<uint, SpatialNode<T>> nodes = new Dictionary<uint, SpatialNode<T>>();

    public SpatialStorage(SpatialNodePool<T> pool) => this.pool = pool;

    public SpatialNode<T> New(T item, float x, float y)
    {
      if ((object) item == null)
        throw new ArgumentNullException(nameof (item));
      SpatialNode<T> spatialNode = this.pool.Allocate();
      spatialNode.Storage = this;
      spatialNode.Item = item;
      spatialNode.AddToBucket(x, y);
      this.items.Add(item);
      return spatialNode;
    }

    public SpatialNode<T> Query(float x, float y)
    {
      SpatialNode<T> spatialNode;
      return this.nodes.TryGetValue(SpatialNode<T>.HashCoordinates(x, y), out spatialNode) ? spatialNode : (SpatialNode<T>) null;
    }

    public void HitTest(float x, float y, Action<SpatialNode<T>> onHit)
    {
      SpatialNode<T> spatialNode1 = this.Query(x, y);
      if (spatialNode1 == null)
        return;
      SpatialNode<T> spatialNode2 = spatialNode1;
      do
      {
        onHit(spatialNode2);
        spatialNode2 = spatialNode2.Next;
      }
      while (spatialNode2 != spatialNode1);
    }

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.items.GetEnumerator();
  }
}
