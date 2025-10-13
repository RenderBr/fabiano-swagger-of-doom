using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

[Keyless]
[XmlRoot]
public class VaultChest
{
    [XmlIgnore]
    public int ChestId { get; set; }

    [XmlText]
    public string _Items { get; set; }

    [XmlIgnore]
    public int[] Items
    {
        get { return Utils.FromCommaSepString32(_Items); }
        set { _Items = Utils.GetCommaSepString(value); }
    }
}