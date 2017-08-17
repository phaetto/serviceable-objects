namespace Serviceable.Objects.Exceptions
{
    using System;

    public class CyclicDependencyException : Exception
    {
        public CyclicDependencyException()
        {
        }

        public CyclicDependencyException(string message) : base(message)
        {
        }

        public CyclicDependencyException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
