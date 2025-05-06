using Orka.Abstractions;

using System.Reflection;
using System.Runtime.Loader;

namespace Orka.Cli.Resources;

public class ResourceLoader
{
    private static readonly string ResourcesPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".orka",
        "resources"
    );

    public IOrkaResourceHandler GetResourceHandler(string provider)
    {
        // Load all resource DLLs
        var files = Directory.GetFiles(ResourcesPath, "*.dll");
        if (files.Length == 0)
        {
            Console.WriteLine($"[WARN] No resource found in {ResourcesPath}");
            return new NoOpResourceHandler();
        }

        foreach (var dll in files)
        {
            var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);

            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"== ReflectionTypeLoadException when loading {dll} ==");
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine(loaderException?.ToString());
                }
                continue; // Skip this assembly and try the next one
            }

            foreach (var type in types)
            {
                if (!type.IsInterface)
                {
                    if (type.Name.StartsWith(provider, StringComparison.OrdinalIgnoreCase) || type.Name.IndexOf(provider, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Console.WriteLine($"Type Name: {type.Name}");
                        Console.WriteLine($"[INFO] Found resource handler for provider: {provider}");
                        return (IOrkaResourceHandler)Activator.CreateInstance(type)!;
                    }
                }
            }
        }

        Console.WriteLine($"[WARN] No resource handler found for provider: {provider}");
        return new NoOpResourceHandler();
    }
}
