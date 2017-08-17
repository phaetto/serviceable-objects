namespace Serviceable.Objects.Remote.Composition
{
    using Serviceable.Objects.Remote.Serialization;

    public sealed class GraphVertex : SerializableSpecification
    {
        public override int DataStructureVersionNumber => 1;

        public string ParentId { get; set; }

        public string Id { get; set; }

        public string TypeFullName { get; set; }

    }
}
