// Decompiled with JetBrains decompiler
// Type: RotMG.Common.SpatialNode`1
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;

#nullable disable
namespace RotMG.Common
{
  public class SpatialNode<T> where T : class
  {
    internal SpatialNode()
    {
    }

    public T Item { get; internal set; }

    public uint Hash { get; internal set; }

    public SpatialStorage<T> Storage { get; internal set; }

    public SpatialNode<T> Next { get; internal set; }

    public SpatialNode<T> Previous { get; internal set; }

    internal static uint HashCoordinates(float x, float y) => (uint) x << 16 | (uint) y;

    internal void AddToBucket(float x, float y)
    {
      this.Hash = SpatialNode<T>.HashCoordinates(x, y);
      SpatialNode<T> spatialNode;
      if (this.Storage.nodes.TryGetValue(this.Hash, out spatialNode))
      {
        SpatialNode<T> next = spatialNode.Next;
        spatialNode.Next = this;
        next.Previous = this;
        this.Next = next;
        this.Previous = spatialNode;
      }
      else
      {
        this.Previous = this.Next = this;
        this.Storage.nodes[this.Hash] = this;
      }
    }

    private void RemoveFromBucket()
    {
      if (this.Storage.nodes[this.Hash] == this)
      {
        if (this.Next != this)
        {
          this.Next.Previous = this.Previous;
          this.Previous.Next = this.Next;
          this.Storage.nodes[this.Hash] = this.Next;
        }
        else
          this.Storage.nodes.Remove(this.Hash);
      }
      else
      {
        this.Next.Previous = this.Previous;
        this.Previous.Next = this.Next;
      }
    }

    public void Remove()
    {
      if ((object) this.Item == null)
        throw new InvalidOperationException("Node is already released.");
      this.RemoveFromBucket();
      this.Storage.items.Remove(this.Item);
      this.Storage.pool.Release(this);
    }

    public void Move(float x, float y)
    {
      if ((object) this.Item == null)
        throw new InvalidOperationException("Node is already released.");
      if ((int) this.Hash == (int) SpatialNode<T>.HashCoordinates(x, y))
        return;
      this.RemoveFromBucket();
      this.AddToBucket(x, y);
    }
  }
}
