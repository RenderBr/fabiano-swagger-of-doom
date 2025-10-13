// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.BufferPool
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System.Collections.Concurrent;
using System.Collections.Generic;

#nullable disable
namespace RotMG.Common.Networking
{
  public class BufferPool
  {
    private readonly int blockSize;
    private readonly Stack<byte[]> blocks = new Stack<byte[]>();
    private readonly int bufferLen;
    private readonly ConcurrentBag<BufferSegment> segments = new ConcurrentBag<BufferSegment>();

    public BufferPool(int bufferLen, int segmentCount)
    {
      this.bufferLen = bufferLen;
      this.blockSize = segmentCount * bufferLen;
    }

    public int BlockCount => this.segments.Count;

    public int SegmentCount => this.segments.Count;

    public int BufferLength => this.bufferLen;

    private BufferSegment NewBlock()
    {
      lock (this.blocks)
      {
        byte[] buffer = new byte[this.blockSize];
        this.blocks.Push(buffer);
        BufferSegment bufferSegment1 = new BufferSegment();
        for (int offset = 0; offset < this.blockSize; offset += this.bufferLen)
        {
          BufferSegment bufferSegment2 = new BufferSegment(buffer, offset);
          if (offset == 0)
            bufferSegment1 = bufferSegment2;
          else
            this.segments.Add(bufferSegment2);
        }
        return bufferSegment1;
      }
    }

    public BufferSegment Allocate()
    {
      BufferSegment result;
      if (!this.segments.TryTake(out result))
        result = this.NewBlock();
      return result;
    }

    public void Release(BufferSegment segment) => this.segments.Add(segment);

    public bool ContainsBlock(byte[] bufferBlock) => this.blocks.Contains(bufferBlock);
  }
}
