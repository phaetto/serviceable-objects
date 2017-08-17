namespace Serviceable.Objects
{
    using System;

    public sealed class EventResult
    {
        public string NodeId { get; set; }

        public Type ContextType { get; set; }

        public object ResultObject { get; set; }
    }
}
