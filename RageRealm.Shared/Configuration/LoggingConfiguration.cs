namespace RageRealm.Shared.Configuration;

public class LoggingConfiguration
{
    public string LogLevel { get; set; } = "Information";
    public bool EnableConsole { get; set; } = true;
    public bool EnableDebug { get; set; } = false;
    public bool EnableFile { get; set; } = false;
    public string FilePath { get; set; } = "logs/server.log";
}