using Orka.Abstractions;
using Orka.Cli.Config;

namespace Orka.Cli.Resources;

public class NoOpResourceHandler : IOrkaResourceHandler
{
    public async Task ExecuteAsync(OrkaResource step)
    {
        Console.WriteLine($"[NO-OP] No plugin available for provider '{step.Provider}'. Skipping execution.");
        await Task.CompletedTask;
    }
}
