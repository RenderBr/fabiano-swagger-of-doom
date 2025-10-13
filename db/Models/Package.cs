using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("packages")]
public class Package
{
    [Key] [Column("id")] public int Id { get; set; }

    [Required] [Column("name")] public string Name { get; set; }

    [Required] [Column("price")] public int Price { get; set; }

    [Required] [Column("quantity")] public int Quantity { get; set; }

    [Required] [Column("maxPurchase")] public int MaxPurchase { get; set; }

    [Required] [Column("weight")] public int Weight { get; set; }

    [Required] [Column("bgUrl")] public string BgUrl { get; set; }

    [Required] [Column("endDate")] public DateTime EndDate { get; set; }

    [Required] [Column("contents")] public string Contents { get; set; }
}