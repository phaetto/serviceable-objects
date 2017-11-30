namespace Serviceable.Objects.Remote
{
    using System;

    public interface IRemotable : IReproducible
    {
        Type ReturnType { get; }
    }
}
