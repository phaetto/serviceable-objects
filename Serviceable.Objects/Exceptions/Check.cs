namespace Serviceable.Objects.Exceptions
{
    using System;

    public static class Check
    {
        public static void ArgumentNull(object testObject, string name, string message = null)
        {
            if (!Equals(testObject, null))
            {
                return;
            }

            throw new ArgumentNullException(name, message ?? $"Parameter '{name}' is null");
        }

        public static void ArgumentNullOrEmpty(string testString, string name, string message = null)
        {
            if (!string.IsNullOrEmpty(testString))
            {
                return;
            }

            throw new ArgumentNullException(name, message ?? $"Parameter '{name}' is null or empty");
        }

        public static void Argument(bool condition, string name, string message)
        {
            if (!condition)
            {
                return;
            }

            throw new ArgumentException($"Argument '{name}' error: {message}", name);
        }

        public static void ArgumentOutOfRange(bool condition, object actualValue, string name, string message)
        {
            if (!condition)
            {
                return;
            }

            throw new ArgumentOutOfRangeException(name, actualValue, $"Argument out of range '{name}': {message}");
        }

        public static void ConditionNotSupported(bool condition, string message, Exception baseException = null)
        {
            if (!condition)
            {
                return;
            }

            throw new NotSupportedException(message, baseException);
        }
    }
}
