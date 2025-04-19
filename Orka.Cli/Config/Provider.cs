using YamlDotNet.Serialization;
namespace Orka.Cli.Config;


public class ProviderRoot
{
    [YamlMember(Alias = "providers")]
    public List<Provider> Providers { get; set; } = new();
}


public class Provider
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;
    [YamlMember(Alias = "type")]
    public string Type { get; set; } = string.Empty;

    [YamlMember(Alias = "config")]
    public ProviderConfig Config { get; set; } = new ProviderConfig();
}
