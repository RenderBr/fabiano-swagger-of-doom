namespace RageRealm.Shared.Models;

public struct RealmTime
{
    public int thisTickCounts { get; set; }
    public int thisTickTimes { get; set; }
    public long tickCount { get; set; }
    public long tickTimes { get; set; }
}