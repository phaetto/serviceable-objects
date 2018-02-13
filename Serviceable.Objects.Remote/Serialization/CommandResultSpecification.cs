namespace Serviceable.Objects.Remote.Serialization
{
    public sealed class CommandResultSpecification
    {
        public string CommandType { get; set; }
        public string ResultDataAsJson { get; set; }
        public bool ContainsError { get; set; }
        public bool ContainsSubdata { get; set; }

        // public override int DataStructureVersionNumber => 1; // TODO: (re)add versioning
    }
}
