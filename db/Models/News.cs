using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("news")]
public class News
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("icon")]
    [MaxLength(64)]
    public string Icon { get; set; }

    [Required]
    [Column("title")]
    [MaxLength(256)]
    public string Title { get; set; }

    [Required]
    [Column("text")]
    [MaxLength(1024)]
    public string Text { get; set; }

    [Required]
    [Column("link")]
    [MaxLength(512)]
    public string Link { get; set; }

    [Required]
    [Column("date")]
    public DateTime Date { get; set; } = DateTime.Now;
}
