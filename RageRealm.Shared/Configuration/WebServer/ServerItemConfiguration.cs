namespace RageRealm.Shared.Configuration.WebServer;

public class ServerItemConfiguration
{
    public int Number { get; set; } = 1;
    public string Name { get; set; } = "Server Item";
    public string Address { get; set; } = "localhost";
    public string Location { get; set; } = "US";
    public int Port { get; set; } = 2050;
    public bool AdminOnly { get; set; } = false;
}