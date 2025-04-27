using Orka.Cli.Config;

namespace Orka.Cli.Resources;

public class ResourceLoader
{
    public IOrkaResourceHandler GetResourceHandler(string provider)
    {
        if (provider == "exec")
        {
            return new DummyCommandResource(); // for now until we replace
        }

        // 🛡️ Instead of returning null, fallback to NoOpHandler
        return new NoOpResourceHandler();
    }
}
