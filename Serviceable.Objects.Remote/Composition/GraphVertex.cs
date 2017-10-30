namespace Serviceable.Objects.Remote.Composition
{
    using Serviceable.Objects.Remote.Serialization;

    public sealed class GraphVertex : SerializableSpecification
    {
        public override int DataStructureVersionNumber => 1;

        public string FromId { get; set; }

        public string ToId { get; set; }
    }
}
