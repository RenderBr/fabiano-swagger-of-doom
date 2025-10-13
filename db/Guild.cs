using System;
using System.Xml.Serialization;

[Serializable, XmlRoot]
public class Guild
{
    [XmlAttribute("id")]
    public long Id { get; set; }
    
    public int Rank { get; set; }
    public string Name { get; set; }

    public int Fame { get; set; }
}