using System;

namespace wServer.networking.svrPackets
{
    public abstract class ServerPacket : Packet
    {
        internal static void WriteCompressedInt(NWriter w, int value)
        {
            bool negative = value < 0;
            int v = Math.Abs(value);
            byte b = (byte)(v & 0x3F); // 6 bits
            if (negative)
                b |= 0x40;
            v >>= 6;
            if (v > 0)
                b |= 0x80;
            w.Write(b);
            int bits = 7;
            while (v > 0)
            {
                b = (byte)(v & 0x7F);
                v >>= 7;
                if (v > 0)
                    b |= 0x80;
                w.Write(b);
                bits += 7;
            }
        }
        
        public override void Crypt(Client client, byte[] dat, int offset, int len)
        {
            //client.SendKey.Crypt(dat, offset, len);
        }
    }
}