using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable, XmlRoot]
public class Stats
{
    [XmlElement("ClassStats")]
    public List<ClassStats> ClassStates { get; set; } = new List<ClassStats>();
    public int BestCharFame { get; set; }
    public int TotalFame { get; set; }
    public int Fame { get; set; }
}