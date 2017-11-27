namespace Serviceable.Objects.Remote.Composition.Graph
{
    using Serialization;

    public sealed class GraphNode : SerializableSpecification
    {
        public override int DataStructureVersionNumber => 1;

        public string Id { get; set; }

        public string TypeFullName { get; set; }
    }
}
