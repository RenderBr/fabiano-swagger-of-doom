using System.Threading.Tasks;
using wServer.Models.Properties;
using wServer.realm.entities;

namespace wServer.Factories;

public interface IPortalFactory
{
    public Task<Portal> CreatePortal(CreatePortalProperties properties);
}