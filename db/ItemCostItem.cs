using System;
using System.Xml.Serialization;

[Serializable]
public class ItemCostItem
{
    //<ItemCost type="854" purchasable="1" expires="0">900</ItemCost>
    [XmlAttribute("type")]
    public string Type { get; set; }

    [XmlAttribute("purchasable")]
    public int Puchasable { get; set; }

    [XmlAttribute("expires")]
    public int Expires { get; set; }

    [XmlText]
    public int Price { get; set; }
}