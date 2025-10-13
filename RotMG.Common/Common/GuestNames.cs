// Decompiled with JetBrains decompiler
// Type: RotMG.Common.GuestNames
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

#nullable disable
namespace RotMG.Common
{
  public static class GuestNames
  {
    private static readonly string[] guestNames = new string[45]
    {
      "Darq",
      "Deyst",
      "Drac",
      "Drol",
      "Eango",
      "Eashy",
      "Eati",
      "Eendi",
      "Ehoni",
      "Gharr",
      "Iatho",
      "Iawa",
      "Idrae",
      "Iri",
      "Issz",
      "Itani",
      "Laen",
      "Lauk",
      "Lorz",
      "Oalei",
      "Odaru",
      "Oeti",
      "Orothi",
      "Oshyu",
      "Queq",
      "Radph",
      "Rayr",
      "Ril",
      "Rilr",
      "Risrr",
      "Saylt",
      "Scheev",
      "Sek",
      "Serl",
      "Seus",
      "Tal",
      "Tiar",
      "Uoro",
      "Urake",
      "Utanu",
      "Vorck",
      "Vorv",
      "Yangu",
      "Yimi",
      "Zhiar"
    };

    public static string GetName(string uuid)
    {
      uint num = 21;
      foreach (char ch in uuid)
        num = num * 7U + (uint) ch;
      return GuestNames.guestNames[(long) num % (long) GuestNames.guestNames.Length];
    }
  }
}
