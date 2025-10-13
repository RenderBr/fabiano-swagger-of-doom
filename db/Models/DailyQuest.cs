using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("dailyquests")]
public class DailyQuest
{
    [Key]
    [Column("accId")]
    public long AccountId { get; set; }

    [Required]
    [Column("goals")]
    public string Goals { get; set; } = "[]";

    [Required]
    [Column("tier")]
    public byte Tier { get; set; } = 1;

    [Required]
    [Column("time")]
    public DateTime Time { get; set; } = DateTime.Now;

    // Navigation property
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }
}
