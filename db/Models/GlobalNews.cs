using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("globalnews")]
public class GlobalNews
{
    [Key]
    [Column("slot")]
    public ushort Slot { get; set; }

    [Required]
    [Column("linkType")]
    public byte LinkType { get; set; }

    [Required]
    [Column("title")]
    [MaxLength(128)]
    public string Title { get; set; }

    [Required]
    [Column("image")]
    public string Image { get; set; }

    [Required]
    [Column("priority")]
    public byte Priority { get; set; }
    
    [Required]
    [Column("linkDetail")]
    public string LinkDetail { get; set; }
    
    [Required]
    [Column("platform")]
    [MaxLength(128)]
    public string Platform { get; set; }
    
    [Required]
    [Column("startTime")]
    public DateTime Date { get; set; } = DateTime.Now;
    
    [Required]
    [Column("endTime")]
    public DateTime EndTime { get; set; } = DateTime.Now.AddDays(7);
}
