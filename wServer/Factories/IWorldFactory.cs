using System;
using System.Threading.Tasks;
using wServer.realm;

namespace wServer.Factories;

public interface IWorldFactory
{
    public Task<World> CreateWorldAsync(Type worldType);
}