using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace db.Models;

[Table("characters")]
public class Character
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("accId")]
    public long AccountId { get; set; }

    [Required]
    [Column("charId")]
    public int CharacterId { get; set; }

    [Required]
    [Column("charType")]
    public int CharacterType { get; set; } = 782;

    [Required]
    [Column("level")]
    public byte Level { get; set; } = 1;

    [Required]
    [Column("exp")]
    public int Experience { get; set; } = 0;

    [Required]
    [Column("fame")]
    public int Fame { get; set; } = 0;

    [Required] [Column("items")] public int[] Items { get; set; } = [];
    
    [Required]
    [Column("hpPotions")]
    public int HpPotions { get; set; } = 0;

    [Required]
    [Column("mpPotions")]
    public int MpPotions { get; set; } = 0;

    [Required]
    [Column("hp")]
    public int Hp { get; set; } = 1;

    [Required]
    [Column("mp")]
    public int Mp { get; set; } = 1;

    [NotMapped]
    public CharacterStats Stats { get; set; } = new CharacterStats();
    
    [Required]
    [Column("stats")]
    public string StatsJson
    {
        get => System.Text.Json.JsonSerializer.Serialize(Stats);
        set => Stats = System.Text.Json.JsonSerializer.Deserialize<CharacterStats>(value) ?? new CharacterStats();
    }

    [Required]
    [Column("dead")]
    public bool Dead { get; set; } = false;

    [Required]
    [Column("tex1")]
    public int Tex1 { get; set; } = 0;

    [Required]
    [Column("tex2")]
    public int Tex2 { get; set; } = 0;

    [Required]
    [Column("pet")]
    public int PetItemType { get; set; } = 0;

    [Column("petId")]
    public int PetId { get; set; }
    
    [ForeignKey(nameof(PetId))]
    public Pet Pet { get; set; }

    [Required]
    [Column("hasBackpack")]
    public bool HasBackpack { get; set; } = false;

    [Required]
    [Column("skin")]
    public int Skin { get; set; } = 0;

    [Required]
    [Column("xpBoosterTime")]
    public int XpBoosterTime { get; set; } = 0;

    [Required]
    [Column("ldTimer")]
    public int LdTimer { get; set; } = 0;

    [Required]
    [Column("ltTimer")]
    public int LtTimer { get; set; } = 0;

    [Column("fameStats")]
    public string FameStats { get; set; }

    [Required]
    [Column("createTime")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    [Column("deathTime")]
    public DateTime? DeathTime { get; set; }

    [Required]
    [Column("totalFame")]
    public int TotalFame { get; set; } = 0;

    [Required]
    [Column("lastSeen")]
    public DateTime LastSeen { get; set; } = DateTime.Now;

    [Required]
    [Column("lastLocation")]
    [MaxLength(128)]
    public string LastLocation { get; set; } = "";

    // Navigation property
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }
}

[Keyless]
public class CharacterStats
{
    public int MaxHitPoints { get; set; } = 100;
    public int MaxMagicPoints { get; set; } = 100;
    public int Attack { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int Speed { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Vitality { get; set; } = 10;
    public int Wisdom { get; set; } = 10;
}
