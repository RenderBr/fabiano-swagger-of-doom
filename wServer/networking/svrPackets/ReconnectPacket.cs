namespace wServer.networking.svrPackets
{
    public class ReconnectPacket : ServerPacket
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public string StatusMessage { get; set; } = "Loading...";
        public int Port { get; set; }
        public int GameId { get; set; }
        public int KeyTime { get; set; }
        public bool IsFromArena { get; set; }
        public byte[] Key { get; set; }

        public override PacketID ID
        {
            get { return PacketID.RECONNECT; }
        }

        public override Packet CreateInstance()
        {
            return new ReconnectPacket();
        }

        protected override void Read(Client psr, NReader rdr)
        {
            Name = rdr.ReadUTF();
            Host = rdr.ReadUTF();
            StatusMessage = rdr.ReadUTF();
            Port = rdr.ReadInt32();
            GameId = rdr.ReadInt32();
            KeyTime = rdr.ReadInt32();
            IsFromArena = rdr.ReadBoolean();
            Key = new byte[rdr.ReadInt16()];
            Key = rdr.ReadBytes(Key.Length);
        }

        protected override void Write(Client psr, NWriter wtr)
        {
            wtr.WriteUTF(Name);
            wtr.WriteUTF(Host);
            wtr.WriteUTF(StatusMessage);
            wtr.Write(Port);
            wtr.Write(GameId);
            wtr.Write(KeyTime);
            wtr.Write(IsFromArena);
            wtr.Write((short) Key.Length);
            wtr.Write(Key);
        }
    }
}