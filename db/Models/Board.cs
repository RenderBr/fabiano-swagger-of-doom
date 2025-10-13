using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("boards")]
public class Board
{
    [Key]
    [Column("guildId")]
    public int GuildId { get; set; }

    [Required]
    [Column("text")]
    [MaxLength(1024)]
    public string Text { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation property
    [ForeignKey("GuildId")]
    public virtual GuildEntity Guild { get; set; }
}
