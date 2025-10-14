using System;
using System.Linq;
using System.Threading.Tasks;
using db.data;
using db.Models;
using db.Repositories;
using Microsoft.Extensions.Logging;

namespace db.Services;

public class CharacterCreationService
{
    private ILogger<CharacterCreationService> Logger { get; }
    private ICharacterRepository CharacterRepository { get; }
    private IAccountRepository AccountRepository { get; }
    private XmlDataService XmlDataService { get; }

    public CharacterCreationService(ILogger<CharacterCreationService> logger, ICharacterRepository characterRepository,
        IAccountRepository accountRepository, XmlDataService xmlDataService)
    {
        Logger = logger;
        CharacterRepository = characterRepository;
        AccountRepository = accountRepository;
        XmlDataService = xmlDataService;
    }

    public Character Create(ushort classType, int characterId, int skinId = 0)
    {
        var classData = XmlDataService.ObjectTypeToElement[classType];
        if (classData == null)
        {
            Logger.LogWarning("Attempted to create character with invalid class type {ClassType}", classType);
            return null;
        }

        var newCharacter = new Character
        {
            CharacterType = classType,
            CharacterId = characterId,
            Level = 1,
            Experience = 0,
            Fame = 0,
            HasBackpack = false,
            Items = classData.Element("Equipment")?
                .Value
                .Replace("0xa22", "-1") // if you still want this substitution
                .Split(',')
                .Select(s =>
                {
                    s = s.Trim();
                    if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        return Convert.ToInt32(s, 16);
                    return int.Parse(s);
                })
                .ToArray(),
            Hp = classData.Element("MaxHitPoints") != null ? int.Parse(classData.Element("MaxHitPoints")?.Value) : 100,
            Mp = classData.Element("MaxMagicPoints") != null
                ? int.Parse(classData.Element("MaxMagicPoints")?.Value)
                : 100,
            Stats = new CharacterStats
            {
                MaxHitPoints = classData.Element("MaxHitPoints") != null
                    ? int.Parse(classData.Element("MaxHitPoints")?.Value)
                    : 100,
                MaxMagicPoints = classData.Element("MaxMagicPoints") != null
                    ? int.Parse(classData.Element("MaxMagicPoints")?.Value)
                    : 100,
                Attack = int.Parse(classData.Element("Attack")?.Value ?? "10"),
                Defense = int.Parse(classData.Element("Defense")?.Value ?? "10"),
                Speed = int.Parse(classData.Element("Speed")?.Value ?? "10"),
                Dexterity = int.Parse(classData.Element("Dexterity")?.Value ?? "10"),
                Vitality = int.Parse(classData.Element("HpRegen")?.Value ?? "10"),
                Wisdom = int.Parse(classData.Element("MpRegen")?.Value ?? "10")
            },
            Tex1 = 0,
            Tex2 = 0,
            Dead = false,
            Pet = null,
            Skin = skinId
        };

        return newCharacter;
    }
}