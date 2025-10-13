using System;
using System.Xml.Serialization;

[Serializable, XmlRoot("Server")]
public class ServerItem
{
    public string Name { get; set; }
    public string DNS { get; set; }
    public double Lat { get; set; }
    public double Long { get; set; }
    public double Usage { get; set; }
    public int RankRequired { get; set; }

    [XmlElement("AdminOnly")]
    private string _AdminOnly { get; set; }

    [XmlIgnore]
    public bool AdminOnly
    {
        get { return _AdminOnly != null; }
        set { _AdminOnly = value ? "True" : null; }
    }
}