using wServer.realm;

namespace wServer.Events;

public class WorldEvents
{
    public record WorldCreatedEvent(World World);
    public record WorldClosedEvent(World World);
}