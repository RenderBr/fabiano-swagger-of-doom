using System;
using System.Xml.Serialization;
using db.Models;
using Microsoft.EntityFrameworkCore;

[Serializable, XmlRoot("Char"), Keyless]
public class Char
{
    [XmlAttribute("id")]
    public int CharacterId { get; set; }

    public int ObjectType { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public int CurrentFame { get; set; }
    public int HealthStackCount { get; set; }
    public int MagicStackCount { get; set; }

    [XmlElement("Equipment")]
    public string _Equipment { get; set; }

    [XmlIgnore]
    public int[] Equipment
    {
        get { return Utils.FromCommaSepString32(_Equipment); }
        set { _Equipment = Utils.GetCommaSepString(value); }
    }

    [XmlIgnore]
    public int[] Backpack { get; set; }

    public int HasBackpack { get; set; }
    public int MaxHitPoints { get; set; }
    public int HitPoints { get; set; }
    public int MaxMagicPoints { get; set; }
    public int MagicPoints { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public int Dexterity { get; set; }
    public int HpRegen { get; set; }
    public int MpRegen { get; set; }
    public int Tex1 { get; set; }
    public int Tex2 { get; set; }
    public bool XpBoosted { get; set; }
    public int XpTimer { get; set; }
    public int LDTimer { get; set; }
    public int LTTimer { get; set; }
    public string PCStats { get; set; }

    [XmlElement("casToken")]
    public string CasToken { get; set; }

    [XmlElement("Texture")]
    public int Skin { get; set; }

    [XmlIgnore]
    public FameStats FameStats { get; set; }

    public bool Dead { get; set; }

    public PetItem Pet { get; set; }

    public static Char FromCharacter(Character character)
    {
        return new Char
        {
            CharacterId = character.CharacterId,
            ObjectType = character.CharacterType,
            Level = character.Level,
            Exp = character.Experience,
            CurrentFame = character.Fame,
            HealthStackCount = character.HpPotions,
            MagicStackCount = character.MpPotions,
            Equipment = character.Items,
            HasBackpack = character.HasBackpack ? 1 : 0,
            MaxHitPoints = character.Stats.MaxHitPoints,
            HitPoints = character.Hp,
            MaxMagicPoints = character.Stats.MaxMagicPoints,
            MagicPoints = character.Mp,
            Attack = character.Stats.Attack,
            Defense = character.Stats.Defense,
            Speed = character.Stats.Speed,
            Dexterity = character.Stats.Dexterity,
            HpRegen = character.Stats.Vitality,
            MpRegen = character.Stats.Wisdom,
            Tex1 = character.Tex1,
            Tex2 = character.Tex2,
            XpBoosted = character.XpBoosterTime > 0,
            XpTimer = character.XpBoosterTime,
            LDTimer = character.LdTimer,
            LTTimer = character.LtTimer,
            PCStats = "{}", // Assuming default value
            CasToken = "", // Assuming default value
            Skin = character.Skin,
            FameStats = new FameStats(),
            Dead = character.Dead,
            Pet = null // Assuming no pet by default
        };
    }
}