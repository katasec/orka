using Orka.Cli.Config;
using Orka.Cli.Resources;

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
                // Get ResourceHandler for the current step
                var resourceHandler = _resourceLoader.GetResourceHandler(step.Provider);
                if (resourceHandler == null)
                {
                    Console.WriteLine($"[SKIP] No plugin found for provider: {step.Provider}");
                    continue;
                }

                // Execute the step using the loaded resource handler
                await resourceHandler.ExecuteAsync(step);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Step '{step.Name}' failed: {ex.Message}");
            }
        }
    }
}
