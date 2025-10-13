using System;
using System.Xml.Serialization;

[Serializable]
public class AbilityItem
{
    [XmlAttribute("type")]
    public int Type { get; set; }

    [XmlAttribute("power")]
    public int Power { get; set; }

    [XmlAttribute("points")]
    public int Points { get; set; }
}