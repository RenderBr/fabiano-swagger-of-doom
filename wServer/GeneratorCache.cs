using System.Collections.Generic;
using System.Threading.Tasks;
using DungeonGenerator;
using DungeonGenerator.Templates;
using DungeonGenerator.Templates.Abyss;
using DungeonGenerator.Templates.Lab;
using DungeonGenerator.Templates.PirateCave;
using Microsoft.Extensions.Logging;

namespace wServer
{
    public class GeneratorCache(ILogger<GeneratorCache> logger)
    {
        private ILogger log = logger;
        private static Dictionary<string, List<string>> cachedMaps;

        public void Init()
        {
            cachedMaps = new Dictionary<string, List<string>>();
            createCache("Abyss of Demons", new AbyssTemplate());
            createCache("Mad Lab", new LabTemplate());
            createCache("Pirate Cave", new PirateCaveTemplate());
        }

        public string NextAbyss(uint seed) => nextMap(seed, "Abyss of Demons", new AbyssTemplate());
        public string NextLab(uint seed) => nextMap(seed, "Mad Lab", new LabTemplate());
        public string NextPirateCave(uint seed) => nextMap(seed, "Pirate Cave", new PirateCaveTemplate());

        private string nextMap(uint seed, string key, DungeonTemplate template)
        {
            var map = cachedMaps[key][0];
            cachedMaps[key].RemoveAt(0);
            log?.LogInformation("Generating new map for dungeon: {dungeonKey}", key);
            Task.Factory.StartNew(() => cachedMaps[key].Add(generateNext(seed, template)));
            return map;
        }

        private string generateNext(uint seed, DungeonTemplate template)
        {
            var gen = new DungeonGen((int)seed, template);
            gen.GenerateAsync();
            return gen.ExportToJson();
        }

        private void createCache(string key, DungeonTemplate template)
        {
            log?.LogInformation("Generating cache for dungeon: {dungeonKey}", key);
            cachedMaps.Add(key, new List<string>());
            for (var i = 0; i < 3; i++) //Keep at least 3 maps in cache
                cachedMaps[key].Add(generateNext(0, template));
        }
    }
}
