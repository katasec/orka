namespace Orka.Cli.Config;

public class Provider
{


    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public Dictionary<string, string> Config { get; set; } = [];
    public static void PrintProviders(Dictionary<string, Provider> providers)
    {
        foreach (var p in providers)
        {
            Console.WriteLine($"- {p.Key} ({p.Value.Type})");
            foreach (var kv in p.Value.Config)
            {
                Console.WriteLine($"  - {kv.Key}: {kv.Value}");
            }
        }
    }
}
