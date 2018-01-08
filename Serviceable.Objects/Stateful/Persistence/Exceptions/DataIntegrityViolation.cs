namespace Serviceable.Objects.Stateful.Persistence.Exceptions
{
    using System;

    public class DataIntegrityViolationException : Exception
    {
        public DataIntegrityViolationException()
        {
        }

        public DataIntegrityViolationException(string message)
            : base(message)
        {
        }

        public DataIntegrityViolationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
