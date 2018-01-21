namespace Serviceable.Objects.Exceptions
{
    using System;

    public class RuntimeExecutionPausedException : Exception
    {
        public RuntimeExecutionPausedException()
        {
        }

        public RuntimeExecutionPausedException(string message) : base(message)
        {
        }

        public RuntimeExecutionPausedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}