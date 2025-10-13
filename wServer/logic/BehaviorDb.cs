#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using db.data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using wServer.logic.loot;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic
{
    public partial class BehaviorDb
    {
        private static wRandom rand = new wRandom();
        private static readonly ILogger log = Program.Services?.GetRequiredService<ILogger<BehaviorDb>>();

        private static int initializing;
        internal static BehaviorDb InitDb;
        private static int randCount = 0;

        internal static wRandom Random
        {
            get
            {
                if (randCount > 10)
                {
                    rand = new wRandom();
                    randCount = 0;
                }
                randCount++;
                return rand;
            }
        }

        public BehaviorDb(RealmManager manager)
        {
            log?.LogInformation("Initializing Behavior Database...");

            Manager = manager;

            Definitions = new Dictionary<ushort, Tuple<State, Loot>>();

            if (Interlocked.Exchange(ref initializing, 1) == 1)
            {
                log?.LogError("Attempted to initialize multiple BehaviorDb at the same time.");
                throw new InvalidOperationException("Attempted to initialize multiple BehaviorDb at the same time.");
            }
            InitDb = this;

            FieldInfo[] fields = GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.FieldType == typeof (_))
                .ToArray();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                log?.LogInformation("Loading behavior for '{fieldName}'({current}/{total})...", 
                    field.Name, i + 1, fields.Length);
                ((_) field.GetValue(this))();
                field.SetValue(this, null);
            }

            InitDb = null;
            initializing = 0;

            log?.LogInformation("Behavior Database initialized...");
        }

        public RealmManager Manager { get; private set; }

        internal static XmlDataService InitGameDataService
        {
            get { return InitDb.Manager.GameDataService; }
        }

        public Dictionary<ushort, Tuple<State, Loot>> Definitions { get; private set; }

        public void ResolveBehavior(Entity entity)
        {
            Tuple<State, Loot> def;
            if (Definitions.TryGetValue((ushort) entity.ObjectType, out def))
                entity.SwitchTo(def.Item1);
        }

        public static ctor Behav()
        {
            return new ctor();
        }

        public delegate ctor _();

        public struct ctor
        {
            public ctor Init(string objType, State rootState, params ILootDef[] defs)
            {
                var d = new Dictionary<string, State>();
                rootState.Resolve(d);
                rootState.ResolveChildren(d);
                XmlDataService dat = InitDb.Manager.GameDataService;
                if (defs.Length > 0)
                {
                    var loot = new Loot(defs);
                    rootState.Death += (sender, e) => loot.Handle((Enemy)e.Host, e.Time);
                    InitDb.Definitions.Add((ushort) dat.IdToObjectType[objType], new Tuple<State, Loot>(rootState, loot));
                }
                else
                    InitDb.Definitions.Add((ushort) dat.IdToObjectType[objType], new Tuple<State, Loot>(rootState, null));
                return this;
            }
        }
    }
}