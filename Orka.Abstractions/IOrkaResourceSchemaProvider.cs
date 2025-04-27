namespace Orka.Abstractions;

public interface IOrkaResourceSchemaProvider
{
    Dictionary<string, OrkaFieldType> GetInputSchema();
}
