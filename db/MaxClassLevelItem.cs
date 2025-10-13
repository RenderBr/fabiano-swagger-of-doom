using System;
using System.Xml.Serialization;

[Serializable]
public class MaxClassLevelItem
{
    [XmlAttribute("classType")]
    public string ClassType { get; set; }

    [XmlAttribute("maxLevel")]
    public string MaxLevel { get; set; }
}