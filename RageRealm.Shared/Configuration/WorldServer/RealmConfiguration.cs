namespace RageRealm.Shared.Configuration.WorldServer;

public class RealmConfiguration
{
    public int MaxClients { get; set; } = 100;
    public int Tps { get; set; } = 20;
    public bool Whitelist { get; set; } = false;
    public bool VerifyEmail { get; set; } = false;
    public DateTime WhitelistTurnOff { get; set; } = DateTime.MaxValue;
    public bool BroadcastNews { get; set; } = false;

    public string ServerDomain { get; set; } = "localhost";
    public int ServerPort { get; set; } = 2050;
}