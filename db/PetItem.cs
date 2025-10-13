using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable, XmlRoot]
public class PetItem
{
    [XmlAttribute("name")]
    public string SkinName { get; set; }

    [XmlAttribute("type")]
    public int Type { get; set; }

    [XmlAttribute("instanceId")]
    public int InstanceId { get; set; }

    [XmlAttribute("maxAbilityPower")]
    public int MaxAbilityPower { get; set; }

    [XmlAttribute("skin")]
    public int Skin { get; set; }

    [XmlAttribute("rarity")]
    public int Rarity { get; set; }

    [XmlArray("Abilities")]
    [XmlArrayItem("Ability")]
    public List<AbilityItem> Abilities { get; set; }

    // Compatibility properties for legacy code
    [XmlIgnore]
    public int ItemType 
    { 
        get => Type; 
        set => Type = value; 
    }

    [XmlIgnore]
    public int Id 
    { 
        get => InstanceId; 
        set => InstanceId = value; 
    }
}