namespace Orka.Abstractions.Schema;

using Orka.Abstractions;
using System.Reflection;

public static class SchemaHelper
{
    public static Dictionary<string, OrkaFieldType> GenerateSchemaFromType(Type type)
    {
        var schema = new Dictionary<string, OrkaFieldType>();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var fieldType = prop.PropertyType;

            if (fieldType == typeof(string))
                schema[prop.Name] = OrkaFieldType.String;
            else if (fieldType == typeof(int))
                schema[prop.Name] = OrkaFieldType.Int;
            else if (typeof(IEnumerable<string>).IsAssignableFrom(fieldType))
                schema[prop.Name] = OrkaFieldType.ArrayOfStrings;
            else
                Console.WriteLine($"[WARN] Unsupported field type '{fieldType.Name}' for '{prop.Name}'");
        }

        return schema;
    }
}
