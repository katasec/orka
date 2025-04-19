using Orka.Cli.Config;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

var text = File.ReadAllText("C:\\Users\\ameer.deen\\.orka\\providers.yaml");
var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
var providers = deserializer.Deserialize<ProviderRoot>(text);
Console.WriteLine($"Loaded provider: {providers.Providers[0].Name}");

var text2 = File.ReadAllText("workflow.yaml");
var deserializer2 = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
var workflows = deserializer2.Deserialize<WorkflowRoot>(text2);
Console.WriteLine($"Loaded workflow: {workflows.Workflows.Name}");

var outputs = new Dictionary<string, string>();

foreach (var step in workflows.Workflows.Steps)
{
    Console.WriteLine($"\n🟢 Running Step: {step.Id}");

    // Resolve any placeholder references in input
    var resolvedInput = step.Input.ToDictionary(
        kvp => kvp.Key,
        kvp => ResolvePlaceholders(kvp.Value, outputs)
    );

    // Execute mock logic (echo, uppercase, reverse)
    string output = Execute(step.Provider, resolvedInput);

    outputs[step.Id] = output;

    Console.WriteLine($"✅ [{step.Id}] Output: {output}");
}

Environment.Exit(0);


// ===== Helpers =====

string ResolvePlaceholders(string input, Dictionary<string, string> context)
{
    return Regex.Replace(input, @"\$\{(?<stepId>[\w]+)\.output\}", match =>
    {
        var key = match.Groups["stepId"].Value;
        return context.TryGetValue(key, out var value) ? value : $"<missing:{key}>";
    });
}

string Execute(string provider, Dictionary<string, string> input)
{
    return provider switch
    {
        "uppercase" => input["user_input"].ToUpper(),
        "reverse" => new string(input["user_input"].Reverse().ToArray()),
        _ => input["user_input"] // includes "echo"
    };
}
