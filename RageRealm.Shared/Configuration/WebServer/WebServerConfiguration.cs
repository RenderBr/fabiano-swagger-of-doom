namespace RageRealm.Shared.Configuration.WebServer;

public class WebServerConfiguration
{
    public LoggingConfiguration Logging { get; set; } = new LoggingConfiguration();
    public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration();
    public SmtpConfiguration Smtp { get; set; } = new SmtpConfiguration();
    public ServerItemConfiguration[] Servers { get; set; } = [];
    
    public int Port { get; set; } = 80;
    public bool TestingOnline { get; set; } = false;
    public bool VerifyEmail { get; set; } = false;
    public string ServerDomain { get; set; } = "localhost";
    public string SupportDomain { get; set; } = "localhost";
}