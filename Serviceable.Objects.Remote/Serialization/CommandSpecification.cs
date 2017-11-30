namespace Serviceable.Objects.Remote.Serialization
{
    public sealed class CommandSpecification
    {
        public string CommandType { get; set; }
        public string DataAsJson { get; set; }
        public string Session { get; set; }
        public string ApiKey { get; set; }

        // public override int DataStructureVersionNumber => 1; // TODO: (re)add versioning
    }
}
