namespace Serviceable.Objects.Remote.Serialization
{
    using System.Collections.Generic;
    using Exceptions;

    public sealed class CommandResultSpecification
    {
        public string CommandType { get; set; }
        public object ResultDataObject { get; set; }
        public bool ContainsError { get; set; }

        public GraphResultSpecification GraphResultSpecification { get; set; }
        public CommandSpecificationExceptionCarrier Exception { get; set; }
        public List<EventResultSpecification> PublishedEvents { get; set; } = new List<EventResultSpecification>();

        // public override int DataStructureVersionNumber => 1; // TODO: (re)add versioning
    }
}
