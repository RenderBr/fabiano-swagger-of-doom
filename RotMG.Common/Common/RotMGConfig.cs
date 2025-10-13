// Decompiled with JetBrains decompiler
// Type: RotMG.Common.RotMGConfig
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using System.Configuration;

#nullable disable
namespace RotMG.Common
{
  public class RotMGConfig : ConfigurationSection
  {
    private static readonly RotMGConfig config = (RotMGConfig) ConfigurationManager.GetSection("RotMG");

    public static RotMGConfig Config => RotMGConfig.config;

    [ConfigurationProperty("resourcesLocation", IsRequired = true)]
    public string ResourcesLocation => (string) this["resourcesLocation"];

    [ConfigurationProperty("dbConnectionString", IsRequired = true)]
    public string DatabaseConnectionString => (string) this["dbConnectionString"];

    [ConfigurationProperty("redisConfig", IsRequired = true)]
    public string RedisConfig => (string) this["redisConfig"];
  }
}
