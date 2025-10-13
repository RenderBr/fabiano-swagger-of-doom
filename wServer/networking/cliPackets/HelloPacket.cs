using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace wServer.networking.cliPackets
{
    public class HelloPacket : ClientPacket
    {
        public string BuildVersion { get; set; }
        public int GameId { get; set; }
        public string GUID { get; set; }
        public string Password { get; set; }
        public string Secret { get; set; }
        public int KeyTime { get; set; }
        public byte[] Key { get; set; }
        public byte[] MapInfo { get; set; }
        public string EntryTag { get; set; }
        public string GameNet { get; set; }
        public string GameNetUserId { get; set; }
        public string PlayPlatform { get; set; }
        public string PlatformToken { get; set; }
        public string UserToken { get; set; }
        public string PreviousConnectionGuid { get; set; }

        public int Random1 { get; set; }
        public int Random2 { get; set; }

        public override PacketID ID => PacketID.HELLO;

        public override Packet CreateInstance() => new HelloPacket();

        protected override void Read(Client psr, NReader rdr)
        {
            // 1. Version
            BuildVersion = rdr.ReadUTF();

            // 2. Game ID
            GameId = rdr.ReadInt32();

            // 3. GUID
            var guildBase64 = rdr.ReadUTF();
            var decryptedGuild = RSA.Instance.Decrypt(guildBase64);

            GUID = decryptedGuild;

            // 4. Random int
            Random1 = rdr.ReadInt32();

            // 5. Password
            var passwordBase64 = rdr.ReadUTF();
            var decryptedPassword = RSA.Instance.Decrypt(passwordBase64);

            Password = decryptedPassword;

            // 6. Random int
            Random2 = rdr.ReadInt32();

            // 7. Secret
            Secret = rdr.ReadUTF();

            // 8. Key time + key
            KeyTime = rdr.ReadInt32();
            ushort keyLen = rdr.ReadUInt16();
            Key = rdr.ReadBytes(keyLen);

            // 9. Map info (int length + raw bytes with writeUTFBytes, not writeUTF)
            int mapLen = rdr.ReadInt32();
            MapInfo = rdr.ReadBytes(mapLen);

            // 10. Remaining strings
            EntryTag = rdr.ReadUTF();
            GameNet = rdr.ReadUTF();
            GameNetUserId = rdr.ReadUTF();
            PlayPlatform = rdr.ReadUTF();
            PlatformToken = rdr.ReadUTF();
            UserToken = rdr.ReadUTF();

            // 11. Hardcoded string from client (you might want to validate this)
            string hardcodedToken = rdr.ReadUTF();

            // 12. Previous connection GUID
            PreviousConnectionGuid = rdr.ReadUTF();
        }

        protected override void Write(Client psr, NWriter wtr)
        {
            // 1. Build version string
            wtr.WriteUTF(BuildVersion);

            // 2. Game ID
            wtr.Write(GameId);

            // 3. Credentials (plain text, not RSA encrypted)
            wtr.WriteUTF(GUID);
            wtr.WriteUTF(Password);
            wtr.WriteUTF(Secret);

            // 4. Key time + key
            wtr.Write(KeyTime);
            wtr.Write((ushort)Key.Length);
            wtr.Write(Key);

            // 5. Map info
            wtr.Write(MapInfo.Length);
            wtr.Write(MapInfo);

            // 6. Remaining strings
            wtr.WriteUTF(EntryTag);
            wtr.WriteUTF(GameNet);
            wtr.WriteUTF(GameNetUserId);
            wtr.WriteUTF(PlayPlatform);
            wtr.WriteUTF(PlatformToken);
            wtr.WriteUTF(UserToken);
            wtr.WriteUTF(PreviousConnectionGuid);
        }
    }
}