using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("giftcodes")]
public class GiftCode
{
    [Key]
    [Column("code")]
    public string Code { get; set; }

    [Column("content")]
    public string Content { get; set; }

    [Column("accId")]
    public ulong AccId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("redeemed_at")]
    public DateTime? RedeemedAt { get; set; }
}