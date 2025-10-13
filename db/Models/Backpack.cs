using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("backpacks")]
public class Backpack
{
    [Required]
    [Column("accId")]
    public long AccountId { get; set; }

    [Required]
    [Column("charId")]
    public int CharacterId { get; set; }

    [Required]
    [Column("items")]
    public string Items { get; set; } = "[]";

    // Navigation property
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }
}
