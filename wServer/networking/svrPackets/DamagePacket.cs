﻿#region

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#endregion

namespace wServer.networking.svrPackets
{
    public class DamagePacket : ServerPacket
    {
        public int TargetId { get; set; }
        public ConditionEffects Effects { get; set; }
        public ushort Damage { get; set; }
        public bool Killed { get; set; }
        public bool ArmorPierce { get; set; }
        public byte BulletId { get; set; }
        public int ObjectId { get; set; }

        public override PacketID ID
        {
            get { return PacketID.DAMAGE; }
        }

        public override Packet CreateInstance()
        {
            return new DamagePacket();
        }

        protected override void Read(Client psr, NReader rdr)
        {
            TargetId = rdr.ReadInt32();
            byte c = rdr.ReadByte();
            Effects = 0;
            for (int i = 0; i < c; i++)
                Effects |= (ConditionEffects) (1 << rdr.ReadByte());
            Damage = rdr.ReadUInt16();
            Killed = rdr.ReadBoolean();
            BulletId = rdr.ReadByte();
            ObjectId = rdr.ReadInt32();
        }

        protected override void Write(Client psr, NWriter wtr)
        {
            wtr.Write(TargetId);
            var eff = new List<byte>();
            for (byte i = 1; i < 255; i++)
                if ((Effects & (ConditionEffects) (1 << i)) != 0)
                    eff.Add(i);
            wtr.Write((byte) eff.Count);
            foreach (byte i in eff) wtr.Write(i);
            wtr.Write(Damage);
            wtr.Write(Killed);
            wtr.Write(ArmorPierce);
            wtr.Write(BulletId);
            wtr.Write(ObjectId);
            
            Program.Services.GetRequiredService<ILogger<DamagePacket>>()
                .LogInformation("Damage: {TargetID} {Effects} {Damage} {Killed} {ArmorPierce} {BulletID} {ObjectID}", TargetId, Effects, Damage, Killed, ArmorPierce, BulletId, ObjectId);
        }
    }
}