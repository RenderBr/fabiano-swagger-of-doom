namespace RageRealm.Shared.Configuration.WorldServer;

public class WorldServerConfiguration
{
    public LoggingConfiguration Logging { get; set; } = new LoggingConfiguration();
    public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration();
    public RealmConfiguration Realm { get; set; } = new RealmConfiguration();
}