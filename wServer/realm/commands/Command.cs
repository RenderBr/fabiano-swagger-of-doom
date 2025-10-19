#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RageRealm.Shared.Models;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm.commands
{
    public abstract class Command
    {
        protected readonly IServiceProvider Services;
        private readonly ILogger _logger;
        public string CommandName { get; private set; }
        public int PermissionLevel { get; private set; }

        protected Command(string name, int permLevel = 0, IServiceProvider services = null, ILogger logger = null)
        {
            Services = services;
            _logger = logger;
            CommandName = name;
            PermissionLevel = permLevel;
        }

        protected T GetRequiredService<T>() => Services.GetRequiredService<T>();
        protected abstract Task Process(Player player, RealmTime time, string[] args);

        private static int GetPermissionLevel(Player player)
        {
            if (player.Client.Account.Rank == 3)
                return 1;
            return 0;
        }


        public bool HasPermission(Player player)
        {
            if (GetPermissionLevel(player) < PermissionLevel)
                return false;
            return true;
        }

        public bool Execute(Player player, RealmTime time, string args)
        {
            if (!HasPermission(player))
            {
                player.SendError("No permission!");
                return false;
            }

            try
            {
                string[] a = args.Split(' ');
                return Process(player, time, a).IsCompletedSuccessfully;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error when executing the command");
                player.SendError("Error when executing the command.");
                return false;
            }
        }
    }

    public class CommandManager
    {
        private readonly IServiceProvider _services;
        private readonly Dictionary<string, Command> cmds;

        public CommandManager(IServiceProvider services, ILogger<CommandManager> logger)
        {
            logger.LogInformation("Initializing Command Manager...");
            _services = services;
            cmds = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var type in typeof(Command).Assembly.GetTypes()
                         .Where(t => !t.IsAbstract && typeof(Command).IsAssignableFrom(t)))
            {
                logger.LogInformation("Creating command type {TypeName}", type.FullName);
                try
                {
                    var command = (Command)ActivatorUtilities.CreateInstance(_services, type);
                    cmds.Add(command.CommandName.ToLowerInvariant(), command);
                }catch(Exception ex)
                {
                    logger.LogError(ex, "Failed to create command type {TypeName}", type.FullName);
                }
            }
            
            logger.LogInformation("Command Manager initialized with {Count} commands.", cmds.Count);
        }

        public IDictionary<string, Command> Commands
        {
            get { return cmds; }
        }

        public bool Execute(Player player, RealmTime time, string text)
        {
            int index = text.IndexOf(' ');
            string cmd = text.Substring(1, index == -1 ? text.Length - 1 : index - 1);
            string args = index == -1 ? "" : text.Substring(index + 1);

            Command command;
            if (!cmds.TryGetValue(cmd, out command))
            {
                player.SendError("Unknown command!");
                return false;
            }

            return command.Execute(player, time, args);
        }
    }
}