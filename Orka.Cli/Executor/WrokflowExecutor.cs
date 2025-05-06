using Orka.Abstractions;
using Orka.Cli.Config;
using Orka.Cli.Resources;
using System.Text.Json;

namespace Orka.Cli.Executor;

public class WorkflowExecutor
{
    private readonly ResourceLoader _resourceLoader;

    public WorkflowExecutor()
    {
        _resourceLoader = new ResourceLoader();
    }

    public async Task ExecuteWorkflowAsync(Workflow workflow)
    {
        foreach (var step in workflow.Steps)
        {
            Console.WriteLine($"\n== Executing: {step.Name} ({step.Provider})");

            try
            {
                var handler = _resourceLoader.GetResourceHandler(step.Provider);
                if (handler == null)
                {
                    Console.WriteLine($"[SKIP] No plugin found for provider: {step.Provider}");
                    continue;
                }

                // Resolve the input type dynamically
                object? typedInput = null;
                if (handler is IOrkaResourceSchemaProvider schemaProvider)
                {
                    var inputType = schemaProvider.GetInputType();
                    var inputJson = JsonSerializer.Serialize(step.Inputs);
                    typedInput = JsonSerializer.Deserialize(inputJson, inputType);
                }

                // Pass both step and the typed input into the handler
                if (typedInput != null)
                {
                    var method = handler.GetType().GetMethod("ExecuteAsync", new[] { typeof(OrkaResource), typedInput.GetType() });
                    if (method != null)
                    {
                        var task = (Task)method.Invoke(handler, new object[] { step, typedInput })!;
                        await task;
                        continue;
                    }
                }

                Console.WriteLine($"[ERROR] Plugin does not support typed execution for provider: {step.Provider}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Step '{step.Name}' failed: {ex.Message}");
            }
        }
    }
}
