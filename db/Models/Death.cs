using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("death")]
public class Death
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("accId")]
    public long AccountId { get; set; }

    [Required]
    [Column("chrId")]
    public int CharacterId { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(64)]
    public string Name { get; set; } = "DEFAULT";

    [Required]
    [Column("charType")]
    public int CharacterType { get; set; } = 782;

    [Required]
    [Column("tex1")]
    public int Tex1 { get; set; } = 0;

    [Required]
    [Column("tex2")]
    public int Tex2 { get; set; } = 0;

    [Required]
    [Column("skin")]
    public int Skin { get; set; } = 0;

    [Required]
    [Column("items")]
    public string Items { get; set; } = "[]";

    [Required]
    [Column("fame")]
    public int Fame { get; set; } = 0;

    [Required]
    [Column("exp")]
    public int Experience { get; set; } = 0;

    [Column("fameStats")]
    public string FameStats { get; set; }

    [Required]
    [Column("totalFame")]
    public int TotalFame { get; set; } = 0;

    [Required]
    [Column("firstBorn")]
    public bool FirstBorn { get; set; } = false;

    [Required]
    [Column("killer")]
    [MaxLength(128)]
    public string Killer { get; set; }

    [Required]
    [Column("time")]
    public DateTime Time { get; set; } = DateTime.Now;

    // Navigation property
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }
}
