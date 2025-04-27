using Orka.Abstractions;
using Orka.Cli.Config;

namespace Orka.Cli.Resources;

public class DummyCommandResource : IOrkaResourceHandler
{
    public async Task ExecuteAsync(OrkaResource step)
    {
        if (step.Inputs.TryGetValue("command", out var command))
        {
            Console.WriteLine($"[Dummy] Executing command: {command}");
            await Task.Delay(500); // Simulate doing some work
        }
        else
        {
            Console.WriteLine("[Dummy] No command provided.");
        }
    }
}
