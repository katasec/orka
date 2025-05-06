namespace Orka.Abstractions;


public interface IOrkaResourceHandler
{
    Task ExecuteAsync(OrkaResource step, object typedInput);
}
