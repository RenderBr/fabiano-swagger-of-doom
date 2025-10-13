using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("arenalb")]
public class ArenaLeaderboard
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("wave")]
    public int Wave { get; set; }

    [Required]
    [Column("accid")]
    public long AccountId { get; set; }

    [Required]
    [Column("charid")]
    public int CharacterId { get; set; }

    [Column("petid")]
    public int? PetId { get; set; }

    [Required]
    [Column("time")]
    [MaxLength(256)]
    public string Time { get; set; }

    [Required]
    [Column("date")]
    public DateTime Date { get; set; } = DateTime.Now;

    // Navigation property
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }
}
