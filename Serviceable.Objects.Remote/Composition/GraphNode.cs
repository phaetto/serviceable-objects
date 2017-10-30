namespace Serviceable.Objects.Remote.Composition
{
    using Serviceable.Objects.Remote.Serialization;

    public sealed class GraphNode : SerializableSpecification
    {
        public override int DataStructureVersionNumber => 1;

        public string Id { get; set; }

        public string TypeFullName { get; set; }
    }
}
