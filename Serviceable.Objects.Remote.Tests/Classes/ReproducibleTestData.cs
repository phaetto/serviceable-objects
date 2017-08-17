namespace Serviceable.Objects.Remote.Tests.Classes
{
    using System;
    using Serviceable.Objects.Remote.Serialization;

    [Serializable]
    public class ReproducibleTestData : SerializableSpecification
    {
        public string DomainName;
        public string ChangeToValue;
        public string[] StringArray;

        public override int DataStructureVersionNumber => 1;
    }
}
