namespace Serviceable.Objects.Remote.Serialization
{
    using System;

    [Serializable]
    public class DataSpecification : SerializableSpecification
    {
        public string DataType;
        public object Data;
        public override int DataStructureVersionNumber => 1;
    }
}
