namespace Serviceable.Objects.Remote.Serialization
{
    using System.Collections.Generic;
    using Exceptions;

    public sealed class CommandExecutionResultSpecification
    {
        public string CommandType { get; set; }
        public string ResultDataAsJson { get; set; }
        public bool IsFaulted { get; set; }
        public bool IsIdle { get; set; }
        public bool IsPaused { get; set; }
        public CommandSpecificationExceptionCarrier CommandSpecificationExceptionCarrier { get; set; }
        public List<IEvent> PublishedEvents { get; set; } = new List<IEvent>();
    }
}
