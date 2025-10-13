#region

using System;
using System.Collections.Generic;
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
        protected readonly ILogger<Command> _logger;

        public Command(string name, int permLevel = 0)
        {
            _logger = Program.Services?.GetRequiredService<ILogger<Command>>();
            CommandName = name;
            PermissionLevel = permLevel;
        }

        public string CommandName { get; private set; }
        public int PermissionLevel { get; private set; }

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
        private readonly ILogger<CommandManager> _logger;
        private readonly Dictionary<string, Command> cmds;
        private RealmManager manager;

        public CommandManager(RealmManager manager)
        {
            _logger = Program.Services?.GetRequiredService<ILogger<CommandManager>>();
            this.manager = manager;
            cmds = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);
            Type t = typeof (Command);
            foreach (Type i in t.Assembly.GetTypes())
                if (t.IsAssignableFrom(i) && i != t)
                {
                    Command instance = (Command) Activator.CreateInstance(i);
                    cmds.Add(instance.CommandName, instance);
                }
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
            _logger?.LogInformation("[Command] <{PlayerName}> {CommandText}", player.Name, text);
            return command.Execute(player, time, args);
        }
    }
}