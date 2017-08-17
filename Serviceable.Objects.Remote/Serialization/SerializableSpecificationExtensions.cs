namespace Serviceable.Objects.Remote.Serialization
{
    public static class SerializableSpecificationExtensions
    {
        public static string SerializeToJson(this SerializableSpecification[] specs)
        {
            return SerializableSpecification.SerializeManyToJson(specs);
        }
    }
}
