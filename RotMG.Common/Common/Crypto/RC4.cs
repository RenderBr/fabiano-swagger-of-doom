// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Crypto.RC4
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System.Globalization;

#nullable disable
namespace RotMG.Common.Crypto
{
  public class RC4
  {
    private byte I;
    private byte J;
    private byte[] state;

    public RC4(byte[] key) => this.Initialize(key);

    public RC4(string key)
    {
      byte[] key1 = new byte[key.Length >> 1];
      for (int startIndex = 0; startIndex < key.Length; startIndex += 2)
        key1[startIndex >> 1] = byte.Parse(key.Substring(startIndex, 2), NumberStyles.HexNumber);
      this.Initialize(key1);
    }

    private void Initialize(byte[] key)
    {
      this.state = new byte[256];
      for (int index = 0; index < 256; ++index)
        this.state[index] = (byte) index;
      byte index1 = 0;
      for (int index2 = 0; index2 < 256; ++index2)
      {
        index1 += (byte) ((uint) this.state[index2] + (uint) key[index2 % key.Length]);
        Utils.Swap<byte>(ref this.state[index2], ref this.state[(int) index1]);
      }
      this.I = (byte) 0;
      this.J = (byte) 0;
    }

    public void Process(byte[] buffer, int offset, int length)
    {
      int num1 = offset + length;
      for (int index = offset; index < num1; ++index)
      {
        ++this.I;
        this.J += this.state[(int) this.I];
        Utils.Swap<byte>(ref this.state[(int) this.I], ref this.state[(int) this.J]);
        byte num2 = this.state[(int) (byte) ((uint) this.state[(int) this.I] + (uint) this.state[(int) this.J])];
        buffer[index] ^= num2;
      }
    }
  }
}
