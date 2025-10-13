using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("guilds")]
public class GuildEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(64)]
    public string Name { get; set; }

    [Required]
    [Column("level")]
    public int Level { get; set; } = 0;

    [Required]
    [Column("members")]
    public string Members { get; set; } = "";

    [Required]
    [Column("guildFame")]
    public int GuildFame { get; set; } = 0;

    [Required]
    [Column("totalGuildFame")]
    public int TotalGuildFame { get; set; } = 0;

    // Compatibility properties for legacy code
    [NotMapped]
    public int Fame 
    { 
        get => GuildFame; 
        set => GuildFame = value; 
    }

    [NotMapped]
    public int TotalFame 
    { 
        get => TotalGuildFame; 
        set => TotalGuildFame = value; 
    }

    [NotMapped]
    public int Rank { get; set; } = 0; // Guild rank (for compatibility)

    // Navigation property
    public virtual ICollection<Account> Accounts { get; set; }
    public virtual Board Board { get; set; }
}
