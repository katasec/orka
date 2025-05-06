using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Orka.Abstractions;
using Orka.Abstractions.Schema;
using Orka.Cli.Resources;

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

            var provider = resource.Type.ToString().Trim('\'', '"');

            var orkResource = new OrkaResource
            {
                Name = resource.Name.IdentifierName,
                Provider = provider
            };

            var handler = new ResourceLoader().GetResourceHandler(provider);
            var inputType = (handler as IOrkaResourceSchemaProvider)?.GetInputType();

            var schema = inputType is not null
                ? SchemaHelper.GenerateSchemaFromType(inputType)
                : new Dictionary<string, OrkaFieldType>();

            if (resource.Value is ObjectSyntax body &&
                body.TryGetPropertyByName("properties")?.Value is ObjectSyntax props &&
                props.TryGetPropertyByName("input")?.Value is ObjectSyntax input)
            {
                foreach (var (key, fieldType) in schema)
                {
                    var prop = input.TryGetPropertyByName(key);
                    if (prop == null || prop.Value == null)
                    {
                        Console.WriteLine($"[ERROR] Missing required input: '{key}' for provider '{provider}'");
                        Environment.Exit(1);
                    }

                    orkResource.Inputs[key] = ParseSyntaxValue(fieldType, prop.Value);
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

    private static object ParseSyntaxValue(OrkaFieldType type, SyntaxBase syntax)
    {
        return type switch
        {
            OrkaFieldType.String when syntax is StringSyntax ss =>
                ss.TryGetLiteralValue() ?? ss.ToString().Trim('"'),

            OrkaFieldType.Int when syntax is IntegerLiteralSyntax intSyntax =>
                int.Parse(intSyntax.ToString()),

            OrkaFieldType.ArrayOfStrings when syntax is ArraySyntax array =>
                array.Items
                     .Select(i => i.Value as StringSyntax)
                     .Where(s => s is not null)
                     .Select(s =>
                     {
                         var literal = s!.TryGetLiteralValue();
                         return literal ?? s.ToString().Trim('"');
                     })
                     .ToList(),

            _ => syntax.ToString()
        };
    }

}
