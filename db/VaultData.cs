using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

[Serializable, XmlRoot, Keyless]
public class VaultData
{
    [XmlElement("Chest")]
    public List<VaultChest> Chests { get; set; }
}