using YamlDotNet.Serialization;

namespace Orka.Cli.Config;

public class ProviderConfig
{
    [YamlMember(Alias = "endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    [YamlMember(Alias = "api_key")]
    public string ApiKey { get; set; } = string.Empty;

    [YamlMember(Alias = "chat_deployment")]
    public string ChatDeployment { get; set; } = string.Empty;

    [YamlMember(Alias = "embedding_deployment")]
    public string EmbeddingDeployment { get; set; } = string.Empty;
}