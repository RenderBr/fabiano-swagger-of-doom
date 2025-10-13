using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("mysteryboxes")]
public class MysteryBox
{
    [Key] [Column("id")] public int Id { get; set; }

    [Required] [Column("title")] public string Title { get; set; }

    [Required] [Column("weight")] public int Weight { get; set; }

    [Column("description")] public string Description { get; set; }

    [Required] [Column("contents")] public string Contents { get; set; }

    [Column("image")] public string Image { get; set; }

    [Column("icon")] public string Icon { get; set; }

    [Required] [Column("priceAmount")] public int PriceAmount { get; set; }

    [Required] [Column("priceCurrency")] public int PriceCurrency { get; set; }

    [Required] [Column("salePrice")] public int SalePrice { get; set; }

    [Required] [Column("saleCurrency")] public int SaleCurrency { get; set; }

    [Required] [Column("startTime")] public DateTime StartTime { get; set; }

    [Required] [Column("boxEnd")] public DateTime BoxEnd { get; set; }
    
    [Required] [Column("saleEnd")] public DateTime SaleEnd { get; set; }
}