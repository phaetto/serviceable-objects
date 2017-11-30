namespace Serviceable.Objects.Remote
{
    using System;

    public interface IReproducibleWithData
    {
        Type InitializationType { get; }

        object DataAsObject { get; }
    }
}