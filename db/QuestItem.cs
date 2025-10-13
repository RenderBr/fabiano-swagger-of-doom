using System;
using System.Xml.Serialization;

[Serializable]
public class QuestItem
{
    [XmlAttribute("tier")]
    public int Tier { get; set; }
    [XmlAttribute("goal")]
    public string Goal { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }

    [XmlIgnore]
    public int Id { get; set; }
    [XmlIgnore]
    public DateTime Time { get; set; }
}