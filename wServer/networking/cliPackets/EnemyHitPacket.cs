using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace wServer.networking.cliPackets
{
    public class EnemyHitPacket : ClientPacket
    {
        public int Time { get; set; }
        public byte BulletId { get; set; }
        public int TargetId { get; set; }
        public bool Killed { get; set; }

        public override PacketID ID
        {
            get { return PacketID.ENEMYHIT; }
        }

        public override Packet CreateInstance()
        {
            return new EnemyHitPacket();
        }

        protected override void Read(Client psr, NReader rdr)
        {
            Time = rdr.ReadInt32();
            BulletId = rdr.ReadByte();
            TargetId = rdr.ReadInt32();
            Killed = rdr.ReadBoolean();
            
            Program.Services.GetRequiredService<ILogger<EnemyHitPacket>>()
                .LogInformation("EnemyHit: {Time} {BulletID} {TargetID} {Killed}", Time, BulletId, TargetId, Killed);
        }

        protected override void Write(Client psr, NWriter wtr)
        {
            wtr.Write(Time);
            wtr.Write(BulletId);
            wtr.Write(TargetId);
            wtr.Write(Killed);
        }
    }
}