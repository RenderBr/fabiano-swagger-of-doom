using db.Models;

namespace RageRealm.Shared.Utilities;

public class AccountUtilities
{
    private static readonly string[] Names =
    {
        "Darq", "Deyst", "Drac", "Drol",
        "Eango", "Eashy", "Eati", "Eendi", "Ehoni",
        "Gharr", "Iatho", "Iawa", "Idrae", "Iri", "Issz", "Itani",
        "Laen", "Lauk", "Lorz",
        "Oalei", "Odaru", "Oeti", "Orothi", "Oshyu",
        "Queq", "Radph", "Rayr", "Ril", "Rilr", "Risrr",
        "Saylt", "Scheev", "Sek", "Serl", "Seus",
        "Tal", "Tiar", "Uoro", "Urake", "Utanu",
        "Vorck", "Vorv", "Yangu", "Yimi", "Zhiar"
    };

    public static Account CreateGuestAccount(string uuid)
    {
        return new Account
        {
            Id = 0,
            Name = Names[(uint)uuid.GetHashCode() % Names.Length],
            Banned = false,
            Rank = 0,
            Credits = 0,
            PetYardType = 1,
            Guild = new GuildEntity()
            {
                Name = "",
                Id = 0,
                Rank = 0
            },
            NameChosen = false,
            Verified = false,
            Stats = new Stat()
            {
                BestCharFame = 0,
                Fame = 0,
                TotalFame = 0
            },
            Vault = new VaultData
            {
                Chests = new List<VaultChest>()
            },
            Gifts = "",
            OwnedSkins = "",
            Guest = true
        };
    }
}