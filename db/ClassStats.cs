using System;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

[Serializable, XmlRoot, Keyless]
public class ClassStats
{
    [XmlAttribute("objectType")]
    public string ObjectType { get; set; }

    public int BestLevel { get; set; }
    public int BestFame { get; set; }
}