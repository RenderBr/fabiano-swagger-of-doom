using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace db.Models;

[Table("stats")]
public class Stat
{
    [Key]
    [Column("accId")]
    public long AccountId { get; set; }

    [Required]
    [Column("fame")]
    public int Fame { get; set; } = 0;

    [Required]
    [Column("totalFame")]
    public int TotalFame { get; set; } = 0;

    [Required]
    [Column("credits")]
    public int Credits { get; set; } = 0;

    [Required]
    [Column("totalCredits")]
    public int TotalCredits { get; set; } = 0;

    [Required]
    [Column("fortuneTokens")]
    public int FortuneTokens { get; set; } = 0;

    [Required]
    [Column("totalFortuneTokens")]
    public int TotalFortuneTokens { get; set; } = 0;

    [Required]
    [Column("bestCharFame")]
    public int BestCharFame { get; set; } = 0;

    // Navigation property
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }

    // Class stats navigation
    [NotMapped]
    public virtual ICollection<ClassStat> ClassStats { get; set; } = new List<ClassStat>();

    // Legacy compatibility
    [NotMapped]
    public List<ClassStats> ClassStates => ClassStats?.Select(cs => new ClassStats
    {
        ObjectType = cs.ObjectType.ToString(), // Assuming ObjectType is int, need to convert to string
        BestLevel = cs.BestLevel,
        BestFame = cs.BestFame
    }).ToList() ?? new List<ClassStats>();
}
