using wServer;
using wServer.networking;
using wServer.networking.svrPackets;

public class ShowEffectPacket : ServerPacket
{
    public EffectType EffectType { get; set; }
    public int TargetId { get; set; }
    public Position PosA { get; set; } = new();
    public Position PosB { get; set; } = new();
    public ARGB Color { get; set; } = new();
    public float Duration { get; set; } = 1f;

    public bool HasTarget { get; set; }
    public bool HasPosA { get; set; }
    public bool HasPosB { get; set; }
    public bool HasColor { get; set; }
    public bool HasDuration { get; set; }

    public override PacketID ID => PacketID.SHOWEFFECT;

    public override Packet CreateInstance() => new ShowEffectPacket();

    protected override void Write(Client psr, NWriter wtr)
    {
        byte flags = 0;
        if (HasColor) flags |= 1 << 0;
        if (HasPosA) flags |= (1 << 1) | (1 << 2);
        if (HasPosB) flags |= (1 << 3) | (1 << 4);
        if (HasDuration) flags |= 1 << 5;
        if (HasTarget) flags |= 1 << 6;

        wtr.Write((byte)EffectType);
        wtr.Write(flags);

        if (HasTarget)
            WriteCompressedInt(wtr, TargetId);

        if (HasPosA)
        {
            wtr.Write(PosA.X);
            wtr.Write(PosA.Y);
        }

        if (HasPosB)
        {
            wtr.Write(PosB.X);
            wtr.Write(PosB.Y);
        }

        if (HasColor)
            Color.Write(psr, wtr);

        if (HasDuration)
            wtr.Write(Duration);
    }

    protected override void Read(Client psr, NReader rdr)
    {
        // THIS PACKET IS NEVER READ BY THE SERVER
    }
}