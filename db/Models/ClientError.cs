using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("clientError")]
public class ClientError
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("message")]
    public string Message { get; set; }
    
    [Required]
    [Column("uuid")]
    public string Uuid { get; set; }
}