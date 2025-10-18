using wServer.realm;

namespace wServer.Models.Properties;

public class CreatePortalProperties
{
    public ushort ObjType { get; set; }
    public int? Life { get; set; }
    public int Size { get; set; }
    public World WorldInstance { get; set; }
    public string Name { get; set; }
}