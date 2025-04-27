using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Orka.Abstractions;
using Orka.Cli.Config;
using System.Text.Json;

namespace Orka.Cli.Config;

public static class WorkflowLoader
{
    public static Workflow Load(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, fileName);
        if (!File.Exists(path))
        {
            Console.WriteLine($"❌ Workflow file not found: {path}");
            Environment.Exit(1);
        }

        var parser = new Parser(File.ReadAllText(path));
        var program = parser.Program();

        var workflow = new Workflow();

        foreach (var declaration in program.Declarations)
        {
            if (declaration is not ResourceDeclarationSyntax resource) continue;

            var orkResource = new OrkaResource
            {
                Name = resource.Name.IdentifierName,
                Provider = resource.Type.ToString().Trim().Trim('\'', '"')
            };

            if (resource.Value is ObjectSyntax body &&
                body.TryGetPropertyByName("properties")?.Value is ObjectSyntax props)
            {
                if (props.TryGetPropertyByName("input")?.Value is ObjectSyntax input)
                {
                    foreach (var prop in input.Properties)
                    {
                        var key = prop.TryGetKeyText();
                        if (key is null)
                            continue;

                        if (prop.Value is ArraySyntax arraySyntax)
                        {
                            var elements = new List<string>();
                            foreach (var arrayItem in arraySyntax.Items)
                            {
                                if (arrayItem.Value is StringSyntax stringSyntax)
                                {
                                    elements.Add(stringSyntax.ToString().Trim('"'));
                                }
                            }
                            orkResource.Inputs[key] = JsonSerializer.Serialize(elements);
                        }
                        else
                        {
                            orkResource.Inputs[key] = prop.Value.ToString().Trim('\'', '"');
                        }
                    }
                }
            }

            if (resource.Value is ObjectSyntax dependsBody &&
                dependsBody.TryGetPropertyByName("dependsOn")?.Value is ArraySyntax dependsOn)
            {
                foreach (var item in dependsOn.Items)
                {
                    orkResource.DependsOn.Add(item.ToString());
                }
            }

            workflow.Steps.Add(orkResource);
        }

        return workflow;
    }
}
