namespace Serviceable.Objects.Exceptions
{
    using System;

    public class TypeCreatedTwiceInContainerException : Exception
    {
        public TypeCreatedTwiceInContainerException()
        {
        }

        public TypeCreatedTwiceInContainerException(string message) : base(message)
        {
        }

        public TypeCreatedTwiceInContainerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
