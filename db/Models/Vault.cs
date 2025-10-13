using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("vaults")]
public class Vault
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("accId")]
    public long AccountId { get; set; }

    [Required]
    [Column("chestId")]
    public int ChestId { get; set; }

    [Required]
    [Column("items")]
    public string Items { get; set; } = "[]";

    [Required]
    [Column("chestType")]
    public byte ChestType { get; set; } = 0;

    // Navigation property
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }
}
