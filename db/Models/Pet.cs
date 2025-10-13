using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace db.Models;

[Table("pets")]
public class Pet
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("accId")]
    public long AccountId { get; set; }

    [Required]
    [Column("petId")]
    public int PetId { get; set; }

    [Required]
    [Column("objType")]
    public int ObjectType { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(64)]
    public string Name { get; set; }

    [Required]
    [Column("rarity")]
    public byte Rarity { get; set; }

    [Required]
    [Column("maxAbilityPower")]
    public byte MaxAbilityPower { get; set; }

    [Required]
    [Column("skin")]
    public int Skin { get; set; }

    [Required]
    [Column("ability1_type")]
    public int Ability1Type { get; set; }

    [Required]
    [Column("ability1_power")]
    public int Ability1Power { get; set; }

    [Required]
    [Column("ability1_points")]
    public int Ability1Points { get; set; }

    [Required]
    [Column("ability2_type")]
    public int Ability2Type { get; set; }

    [Required]
    [Column("ability2_power")]
    public int Ability2Power { get; set; }

    [Required]
    [Column("ability2_points")]
    public int Ability2Points { get; set; }

    [Required]
    [Column("ability3_type")]
    public int Ability3Type { get; set; }

    [Required]
    [Column("ability3_power")]
    public int Ability3Power { get; set; }

    [Required]
    [Column("ability3_points")]
    public int Ability3Points { get; set; }

    // Navigation property
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }

    // Legacy compatibility properties
    [NotMapped]
    public int PetType 
    { 
        get => ObjectType; 
        set => ObjectType = value; 
    }

    [NotMapped]
    public int InstanceId 
    { 
        get => PetId; 
        set => PetId = value; 
    }

    [NotMapped]
    public int MaxLevel { get; set; } = 30; // Default max level

    [NotMapped]
    public int Family { get; set; } = 0; // Pet family

    [NotMapped]
    public int FirstAbility 
    { 
        get => Ability1Type; 
        set => Ability1Type = value; 
    }

    [NotMapped]
    public int SecondAbility 
    { 
        get => Ability2Type; 
        set => Ability2Type = value; 
    }

    [NotMapped]
    public int ThirdAbility 
    { 
        get => Ability3Type; 
        set => Ability3Type = value; 
    }
}
