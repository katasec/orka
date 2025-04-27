using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Orka.Cli.Config;

namespace Orka.Cli.Config;

public static class WorkflowLoader
{
    public static Workflow Load(string path)
    {
        var sourceText = File.ReadAllText(path);

        var parser = new Parser(sourceText);
        var programSyntax = parser.Program();

        var workflow = new Workflow
        {
            Name = Path.GetFileNameWithoutExtension(path),
            Steps = new List<OrkaResource>()
        };

        foreach (var declaration in programSyntax.Declarations)
        {
            if (declaration is not ResourceDeclarationSyntax resource)
                continue;

            var orkaResource = new OrkaResource
            {
                Name = resource.Name.IdentifierName,
                Provider = resource.Type.ToString().Trim().Trim('\'')
            };

            // Parse properties block
            if (resource.Value is ObjectSyntax body &&
                body.TryGetPropertyByName("properties")?.Value is ObjectSyntax props)
            {
                // Parse input
                if (props.TryGetPropertyByName("input")?.Value is ObjectSyntax input)
                {
                    foreach (var prop in input.Properties)
                    {
                        var key = prop.TryGetKeyText();
                        var val = prop.Value.ToString().Trim().Trim('\'');

                        if (key != null)
                        {
                            orkaResource.Inputs[key] = val;
                        }
                        else
                        {
                            Console.WriteLine("[WARN] Found null key inside input block.");
                        }
                    }
                }

            }

            // Parse dependsOn at top level
            if (resource.Value is ObjectSyntax body2 &&
                body2.TryGetPropertyByName("dependsOn")?.Value is ArraySyntax dependsArray)
            {
                foreach (var item in dependsArray.Items)
                {
                    orkaResource.DependsOn.Add(item.ToString().Trim().Trim('\''));
                }
            }

            workflow.Steps.Add(orkaResource);
        }

        return workflow;
    }
}
