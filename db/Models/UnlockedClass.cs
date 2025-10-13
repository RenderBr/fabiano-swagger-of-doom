using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("unlockedclasses")]
public class UnlockedClass
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("accId")]
    public long AccId { get; set; }

    [Column("class")]
    public string Class { get; set; }

    [Column("available")]
    public string Available { get; set; }
}