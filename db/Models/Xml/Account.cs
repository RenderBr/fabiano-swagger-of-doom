using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace db.Models.Xml;

[Serializable]
[XmlRoot("Account")]
public class AccountXml
{
    public string AccountId { get; set; }
    public string Name { get; set; }
    public string NameChosen { get; set; }
    public string Converted { get; set; }
    public string Admin { get; set; }
    public string Banned { get; set; }
    public string VerifiedEmail { get; set; }
    public int Credits { get; set; } = 0;
    public int NextCharSlotPrice { get; set; } = 600;
    public VaultData Vault { get; set; }
    public Guild Guild { get; set; }
    public string Gifts { get; set; }
    public int PetYardType { get; set; } = 1;
    public Stats Stats { get; set; }
    public int IsAgeVerified { get; set; }

    public AccountXml() { }
    
    public AccountXml(Account account)
    {
        this.AccountId = account.Id.ToString();
        this.NameChosen = account.NameChosen ? "True" : string.Empty;
        this.Name = account.Name;
        this.Admin = account.IsAdmin ? "True" : string.Empty;
        this.Banned = account.Banned ? "True" : string.Empty;
        this.VerifiedEmail = account.Verified ? "True" : string.Empty;
        this.Converted = string.Empty;
        this.Vault = account.Vault;
        this.Guild = account.GuildObj;
        this.Gifts = Utils.GetCommaSepString(Array.Empty<int>());
        this.PetYardType = account.PetYardType;
        this.Stats = new Stats
        {
            ClassStates = [],
            Fame = account.Fame,
            TotalFame = account.TotalFame,
            BestCharFame = account.Stats?.BestCharFame ?? 0,
        };
        this.Credits = account.Credits;
        this.IsAgeVerified = account.IsAgeVerified ? 1 : 0;

    }
    
    private XmlSerializerNamespaces _namespaces = new([
        new XmlQualifiedName(string.Empty, "rotmg")
    ]);
    
    [XmlNamespaceDeclarations]
    public XmlSerializerNamespaces Namespaces
    {
        get { return _namespaces; }
    }
    
}