namespace Serviceable.Objects.Remote.Serialization.Exceptions
{
    public class CommandSpecificationExceptionCarrier
    {
        public string Message { get; set; }
        public string RealExceptionType { get; set; }
        public string StackTrace { get; set; }
    }
}