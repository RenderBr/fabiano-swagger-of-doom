using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer.Models.Properties;
using wServer.realm;
using wServer.realm.entities;

namespace wServer.Factories;

public class PortalFactory : IPortalFactory
{
    private readonly ILogger<PortalFactory> _logger;

    public PortalFactory(ILogger<PortalFactory> logger)
    {
        _logger = logger;
    }

    public Task<Portal> CreatePortal(CreatePortalProperties properties)
    {
        if (properties == null)
            throw new ArgumentNullException(nameof(properties));

        var manager = properties.WorldInstance?.Manager;
        if (manager == null)
            throw new InvalidOperationException("CreatePortal requires properties.WorldInstance with a valid Manager.");

        return Task.FromResult(new Portal(manager, properties));
    }
}