// Decompiled with JetBrains decompiler
// Type: RotMG.Common.VisibilityMap
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System.Collections;

#nullable disable
namespace RotMG.Common
{
  public class VisibilityMap
  {
    private readonly BitArray bitmap;
    private readonly int w;
    private readonly int h;

    public VisibilityMap(int w, int h)
    {
      this.bitmap = new BitArray(w * h);
      this.w = w;
      this.h = h;
    }

    public bool IsSet(int x, int y) => this.bitmap[y * this.w + x];

    public void Set(int x, int y, bool value) => this.bitmap[y * this.w + x] = value;
  }
}
