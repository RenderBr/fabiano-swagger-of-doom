using System;
using System.Xml.Serialization;

[Serializable]
public class ClassAvailabilityItem
{
    [XmlAttribute("id")]
    public string Class { get; set; }

    [XmlText]
    public string Restricted { get; set; }
}