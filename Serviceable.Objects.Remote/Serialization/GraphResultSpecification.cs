namespace Serviceable.Objects.Remote.Serialization
{
    public sealed class GraphResultSpecification
    {
        public bool IsFaulted { get; set; }
        public bool IsIdle { get; set; }
        public bool IsPaused { get; set; }
    }
}
