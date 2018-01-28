namespace Serviceable.Objects.Proxying
{
    public interface IProxyFactoryContext
    {
        object GenerateProxyCommandForGenericExecution(object commandToBeProxied);
    }
}