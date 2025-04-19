using YamlDotNet.Serialization;
namespace Orka.Cli.Config;


public class WorkflowRoot
{
    [YamlMember(Alias = "workflows")]
    public Workflow Workflows { get; set; } = new();
}



public class Workflow
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "steps")]
    public List<WorkflowStep> Steps { get; set; } = new();
}

public class WorkflowStep
{
    [YamlMember(Alias = "id")]
    public string Id { get; set; } = string.Empty;

    [YamlMember(Alias = "provider")]
    public string Provider { get; set; } = string.Empty;

    [YamlMember(Alias = "input")]
    public Dictionary<string, string> Input { get; set; } = new();
}

