using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("classstats")]
public class ClassStat
{
    [Required]
    [Column("accId")]
    public long AccountId { get; set; }

    [Required]
    [Column("objType")]
    public int ObjectType { get; set; }

    [Required]
    [Column("bestLv")]
    public byte BestLevel { get; set; } = 1;

    [Required]
    [Column("bestFame")]
    public int BestFame { get; set; } = 0;

    // Navigation property
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }
}
