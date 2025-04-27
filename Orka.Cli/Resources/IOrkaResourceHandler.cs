using Orka.Cli.Config;

namespace Orka.Cli.Resources;


public interface IOrkaResourceHandler
{
    Task ExecuteAsync(OrkaResource step);
}
