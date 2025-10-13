using System;
using System.Xml.Serialization;

[Serializable, XmlRoot]
public class GuildStruct
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    public string Name { get; set; }
    public int Level { get; set; }
    public string[] Members { get; set; }
    public int GuildFame { get; set; }
    public int TotalGuildFame { get; set; }
}