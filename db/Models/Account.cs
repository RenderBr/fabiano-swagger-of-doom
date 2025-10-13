using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;
using System.Xml.Serialization;

namespace db.Models;

[Table("accounts")]
public class Account
{
    [Key] [Column("id")] public long Id { get; set; }

    [Required]
    [Column("uuid")]
    [MaxLength(128)]
    public string Uuid { get; set; }

    [Required]
    [Column("password")]
    [MaxLength(256)]
    public string Password { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(64)]
    public string Name { get; set; } = "DEFAULT";

    [XmlIgnore]
    [Required]
    [Column("rank")]
    public byte Rank { get; set; } = 0;

    [NotMapped] [XmlElement("Converted")] public string Converted { get; set; }
    [Required] [Column("namechosen")] public bool NameChosen { get; set; } = false;

    [XmlElement("VerifiedEmail")]
    [Required]
    [Column("verified")]
    public bool Verified { get; set; } = true;

    [XmlIgnore]
    [Required]
    [Column("guild")]
    public int GuildId { get; set; } = 0;

    [XmlIgnore]
    [Required]
    [Column("guildRank")]
    public byte GuildRank { get; set; } = 0;

    [XmlIgnore]
    [Required]
    [Column("guildFame")]
    public int GuildFame { get; set; } = 0;

    [Required]
    [Column("lastip")]
    [MaxLength(45)]
    public string LastIp { get; set; } = "";

    [XmlIgnore]
    [Required]
    [Column("vaultCount")]
    public byte VaultCount { get; set; } = 1;

    [XmlIgnore]
    [Required]
    [Column("maxCharSlot")]
    public byte MaxCharSlot { get; set; } = 2;

    [Required] [Column("regTime")] public DateTime RegTime { get; set; } = DateTime.Now;

    [Required] [Column("guest")] public bool Guest { get; set; } = false;

    [Required] [Column("banned")] public bool Banned { get; set; } = false;

    [XmlIgnore]
    [Required]
    [Column("publicMuledump")]
    public bool PublicMuledump { get; set; } = true;

    [Required] [Column("muted")] public bool Muted { get; set; } = false;

    [Required] [Column("prodAcc")] public bool ProdAcc { get; set; } = false;

    [Column("locked")] public string Locked { get; set; }

    [Column("ignored")] public string Ignored { get; set; }

    [XmlIgnore] [Column("gifts")] public string Gifts { get; set; } = "";

    [Required] [Column("isAgeVerified")] public bool IsAgeVerified { get; set; } = false;

    [Required] [Column("petYardType")] public byte PetYardType { get; set; } = 1;

    [Column("ownedSkins")] public string OwnedSkins { get; set; }

    [Required]
    [Column("authToken")]
    [MaxLength(128)]
    public string AuthToken { get; set; } = "";

    [Required] [Column("acceptedNewTos")] public bool AcceptedNewTos { get; set; } = true;

    [Required] [Column("lastSeen")] public DateTime LastSeen { get; set; } = DateTime.Now;

    [Required] [Column("accountInUse")] public bool AccountInUse { get; set; } = false;

    [NotMapped, XmlIgnore]
    // Navigation properties
    public virtual Stat Stats { get; set; } = new();

    // Legacy compatibility properties
    [NotMapped] public string AccountId => Id.ToString();

    [NotMapped]
    public int Credits
    {
        get => Stats?.Credits ?? 0;
        set
        {
            if (Stats != null) Stats.Credits = value;
        }
    }

    [NotMapped]
    public int FortuneTokens
    {
        get => Stats?.FortuneTokens ?? 0;
        set
        {
            if (Stats != null) Stats.FortuneTokens = value;
        }
    }

    [NotMapped] public bool IsGuestAccount => Guest;

    [XmlElement("Admin")] [NotMapped] public bool IsAdmin => Rank >= 2;

    [NotMapped] public bool VerifiedEmail => Verified;

    [NotMapped] public string DailyQuest { get; set; } // Need to implement

    [NotMapped] public string NextGiftCode { get; set; } // Need to implement

    [NotMapped] public VaultData Vault { get; set; } // Need to implement

    [NotMapped] public string Backpacks { get; set; } // Legacy compatibility

    [NotMapped]
    public int Fame
    {
        get => Stats?.Fame ?? 0;
        set
        {
            if (Stats != null) Stats.Fame = value;
        }
    }

    [NotMapped]
    public int TotalFame
    {
        get => Stats?.TotalFame ?? 0;
        set
        {
            if (Stats != null) Stats.TotalFame = value;
        }
    }

    // Legacy compatibility - Guild as int (guild id)
    [XmlIgnore, ForeignKey(nameof(GuildId))]
    public GuildEntity Guild { get; set; }

    // For legacy compatibility - Guild as object (would need to be populated)
    [NotMapped, XmlElement("Guild")]
    public Guild GuildObj
    {
        get
        {
            if (GuildId == 0 || Guild == null) return null;
            return new Guild
            {
                Id = Guild.Id,
                Name = Guild.Name,
                Rank = GuildRank,
                Fame = GuildFame
            };
        }
        set { }
    }
}