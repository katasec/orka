namespace Orka.Abstractions;

public class OrkaResource
{
    public string Name { get; set; } = "default";
    public string Provider { get; set; } = "";
    public Dictionary<string, object> Inputs { get; set; } = new();
    public List<string> DependsOn { get; set; } = new();
}