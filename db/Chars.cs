#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using db.Models;
using db.Models.Xml;

#endregion

[Serializable, XmlRoot("Chars")]
public class Chars
{
    private XmlSerializerNamespaces _namespaces;

    public Chars()
    {
        _namespaces = new XmlSerializerNamespaces(new[]
        {
            new XmlQualifiedName(string.Empty, "rotmg")
        });
    }

    [XmlElement("Char")]
    public List<Char> Characters { get; set; }

    [XmlAttribute("nextCharId")]
    public int NextCharId { get; set; }

    [XmlAttribute("maxNumChars")]
    public int MaxNumChars { get; set; }

    public AccountXml Account { get; set; }

    [XmlArray("News")]
    [XmlArrayItem("Item")]
    public List<NewsItem> News { get; set; }

    [XmlArray("Servers")]
    [XmlArrayItem("Server")]
    public List<ServerItem> Servers { get; set; }

    public string OwnedSkins { get; set; }
    [XmlElement("TOSPopup")]
    public string TOSPopup { get; set; }
    public string Lat { get; set; }
    public string Long { get; set; }

    [XmlArray("ClassAvailabilityList")]
    [XmlArrayItem("ClassAvailability")]
    public List<ClassAvailabilityItem> ClassAvailabilityList { get; set; }

    [XmlNamespaceDeclarations]
    public XmlSerializerNamespaces Namespaces
    {
        get { return _namespaces; }
    }

    [XmlArray("ItemCosts")]
    [XmlArrayItem("ItemCost")]
    public List<ItemCostItem> ItemCostsList
    {
        get
        {
            return new List<ItemCostItem>
            {
                new ItemCostItem {Type = "900", Puchasable = 0, Expires = 0, Price = 90000},
                new ItemCostItem {Type = "902", Puchasable = 0, Expires = 0, Price = 90000},
                new ItemCostItem {Type = "834", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "835", Puchasable = 1, Expires = 0, Price = 600},
                new ItemCostItem {Type = "836", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "837", Puchasable = 1, Expires = 0, Price = 600},
                new ItemCostItem {Type = "838", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "839", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "840", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "841", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "842", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "843", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "844", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "845", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "846", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "847", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "848", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "849", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "850", Puchasable = 0, Expires = 1, Price = 900},
                new ItemCostItem {Type = "851", Puchasable = 0, Expires = 1, Price = 900},
                new ItemCostItem {Type = "852", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "853", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "854", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "855", Puchasable = 1, Expires = 0, Price = 900},
                new ItemCostItem {Type = "856", Puchasable = 0, Expires = 0, Price = 90000},
                new ItemCostItem {Type = "883", Puchasable = 0, Expires = 0, Price = 90000}
            };
        }
    }

    [XmlArray("MaxClassLevelList")]
    [XmlArrayItem("MaxClassLevel")]
    public List<MaxClassLevelItem> MaxClassLevelList
    {
        get
        {
            return new List<MaxClassLevelItem>
            {
                new MaxClassLevelItem {ClassType = "768", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "800", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "802", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "803", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "804", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "805", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "806", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "775", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "782", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "797", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "784", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "801", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "798", MaxLevel = "20"},
                new MaxClassLevelItem {ClassType = "799", MaxLevel = "20"}
            };
        }
    }

    public string SalesForce { get; set; }
}