namespace Serviceable.Objects
{
    using System;

    public sealed class EventResult
    {
        public Type ContextType { get; set; }

        public object ResultObject { get; set; }
    }
}
