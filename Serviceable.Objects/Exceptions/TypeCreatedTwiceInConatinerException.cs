namespace Serviceable.Objects.Exceptions
{
    using System;

    public class TypeCreatedTwiceInConatinerException : Exception
    {
        public TypeCreatedTwiceInConatinerException()
        {
        }

        public TypeCreatedTwiceInConatinerException(string message) : base(message)
        {
        }

        public TypeCreatedTwiceInConatinerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
