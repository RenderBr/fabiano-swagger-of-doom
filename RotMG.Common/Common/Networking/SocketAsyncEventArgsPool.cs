// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.SocketAsyncEventArgsPool
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

#nullable disable
namespace RotMG.Common.Networking
{
  public class SocketAsyncEventArgsPool
  {
    private readonly ConcurrentBag<SocketAsyncEventArgs> pool = new ConcurrentBag<SocketAsyncEventArgs>();
    private Func<SocketAsyncEventArgs> allocateNewFunc;

    public SocketAsyncEventArgsPool(int initialCount, Func<SocketAsyncEventArgs> allocateNewFunc)
    {
      this.allocateNewFunc = allocateNewFunc;
      for (int index = 0; index < initialCount; ++index)
        this.pool.Add(allocateNewFunc());
    }

    public int Count => this.pool.Count;

    public SocketAsyncEventArgs Allocate()
    {
      SocketAsyncEventArgs result;
      if (!this.pool.TryTake(out result))
        result = this.allocateNewFunc();
      return result;
    }

    public void Release(SocketAsyncEventArgs args)
    {
      args.SetBuffer((byte[]) null, 0, 0);
      args.UserToken = (object) null;
      this.pool.Add(args);
    }
  }
}
