namespace Serviceable.Objects.Remote.Tests
{
    using System.Collections.Generic;
    using Serviceable.Objects.Remote.Serialization;
    using Serviceable.Objects.Remote.Tests.Classes;
    using Xunit;

    public class SerializationTest
    {
        [Fact]
        public void Chain_WhenObjectIsSerializedToJsonAndDeserialized_ThenTheObjectIsTheSame()
        {
            var data = new ReproducibleTestData
                       {
                           ChangeToValue = "value",
                           DomainName = "domain"
                       };

            var serializableData = data.SerializeToJson();

            var deserializedData =
                DeserializableSpecification<ReproducibleTestData>.DeserializeFromJson(serializableData);

            Assert.Equal("value", deserializedData.ChangeToValue);
            Assert.Equal("domain", deserializedData.DomainName);
        }

        [Fact]
        public void Chain_WhenManyObjectsAreSerializedToJsonAndDeserialized_ThenObjectsAreTheSame()
        {
            var data = new[]
                       {
                           new ReproducibleTestData
                           {
                               ChangeToValue = "value 1",
                               DomainName = "domain 1",
                               StringArray = new[]
                                             {
                                                 "stuff 1",
                                                 "stuff 2",
                                             }
                           },
                           new ReproducibleTestData
                           {
                               ChangeToValue = "value 2",
                               DomainName = "domain 2"
                           },
                           new ReproducibleTestData
                           {
                               ChangeToValue = "value 3",
                               DomainName = "domain 3"
                           }
                       };

            var serializableData = SerializableSpecification.SerializeManyToJson(data);

            var deserializedData =
                DeserializableSpecification<ReproducibleTestData>.DeserializeManyFromJson(serializableData);

            Assert.Equal("value 1", deserializedData[0].ChangeToValue);
            Assert.Equal("domain 1", deserializedData[0].DomainName);
            Assert.Equal("value 2", deserializedData[1].ChangeToValue);
            Assert.Equal("domain 2", deserializedData[1].DomainName);
            Assert.Equal("value 3", deserializedData[2].ChangeToValue);
            Assert.Equal("domain 3", deserializedData[2].DomainName);
        }

        [Fact]
        public void DeserializableSpecification_WhenDeserializingComplexExceptionDataFromCommandSpec_ThenItSucceeds()
        {
            var jsonData =
                "{\"DataType\":\"System.Collections.Generic.KeyNotFoundException\",\"Data\":{\"ClassName\":\"System.Collections.Generic.KeyNotFoundException\",\"Message\":\"The given key was not present in the dictionary.\",\"InnerException\":null,\"HelpURL\":null,\"StackTraceString\":\"at System.Collections.Generic.Dictionary`2 < string, string>.get_Item(string) < 0x001b8 >\nat Services.Communication.DataStructures.NameValue.GetKeyValue.Execute(Services.Communication.DataStructures.NameValue.HashContext) < 0x0004f >\nat Chains.Context`1 < Services.Communication.DataStructures.NameValue.HashContext >.InvokeAct<Services.Communication.DataStructures.NameValue.KeyValueData>(Chains.ICommand`2 < Services.Communication.DataStructures.NameValue.HashContext, Services.Communication.DataStructures.NameValue.KeyValueData >) < 0x0005f >\nat Chains.Context`1 < Services.Communication.DataStructures.NameValue.HashContext >.Execute<Services.Communication.DataStructures.NameValue.KeyValueData>(Chains.ICommand`2 < Services.Communication.DataStructures.NameValue.HashContext, Services.Communication.DataStructures.NameValue.KeyValueData >) < 0x00063 >\nat(wrapper dynamic - method) object.CallSite.Target(System.Runtime.CompilerServices.Closure, System.Runtime.CompilerServices.CallSite, object, object) < 0x0010f >\nat Chains.Play.ExecuteActionAndGetResult.Execute(Chains.Play.DynamicExecutionContext) < 0x005a3 >\nat Chains.Context`1 < Chains.Play.DynamicExecutionContext >.InvokeAct<Chains.Play.ExecutionResultContext>(Chains.ICommand`2 < Chains.Play.DynamicExecutionContext, Chains.Play.ExecutionResultContext >) < 0x0005f >\nat Chains.Context`1 < Chains.Play.DynamicExecutionContext >.Execute<Chains.Play.ExecutionResultContext>(Chains.ICommand`2 < Chains.Play.DynamicExecutionContext, Chains.Play.ExecutionResultContext >) < 0x00063 >\nat Chains.Play.ExecuteActionFromSpecification.Execute(Chains.Play.DynamicExecutionContext) < 0x0005f >\nat Chains.Context`1 < Chains.Play.DynamicExecutionContext >.InvokeAct<Chains.Play.ExecutionResultContext>(Chains.ICommand`2 < Chains.Play.DynamicExecutionContext, Chains.Play.ExecutionResultContext >) < 0x0005f >\nat Chains.Context`1 < Chains.Play.DynamicExecutionContext >.Execute<Chains.Play.ExecutionResultContext>(Chains.ICommand`2 < Chains.Play.DynamicExecutionContext, Chains.Play.ExecutionResultContext >) < 0x00063 >\nat Services.Communication.Protocol.ProtocolServerLogic.ApplyDataOnExecutionChain(Chains.Play.DynamicExecutionContext, Chains.Play.ExecutableCommandSpecification[]) < 0x00083 >\nat Services.Communication.Protocol.ProtocolServerLogic.ApplyDataAndReturn(Chains.Play.DynamicExecutionContext, Chains.Play.ExecutableCommandSpecification[], bool) < 0x000e3 >\n\",\"RemoteStackTraceString\":null,\"RemoteStackIndex\":0,\"HResult\":-2146233087,\"Source\":\"mscorlib\",\"ExceptionMethod\":null,\"Data\":null},\"DataStructureVersionNumber\":1}";

            var deserializedData =
                DeserializableSpecification<ExecutableCommandSpecification>.DeserializeFromJson(jsonData);

            Assert.NotNull(deserializedData);
            Assert.IsType<KeyNotFoundException>(deserializedData.Data);
        }

        [Fact]
        public void DeserializableSpecification_WhenDeserializingComplexExceptionDataFromDataSpec_ThenItSucceeds()
        {
            var jsonData =
                "{\"DataType\":\"System.Collections.Generic.KeyNotFoundException\",\"Data\":{\"ClassName\":\"System.Collections.Generic.KeyNotFoundException\",\"Message\":\"The given key was not present in the dictionary.\",\"InnerException\":null,\"HelpURL\":null,\"StackTraceString\":\"at System.Collections.Generic.Dictionary`2 < string, string>.get_Item(string) < 0x001b8 >\nat Services.Communication.DataStructures.NameValue.GetKeyValue.Execute(Services.Communication.DataStructures.NameValue.HashContext) < 0x0004f >\nat Chains.Context`1 < Services.Communication.DataStructures.NameValue.HashContext >.InvokeAct<Services.Communication.DataStructures.NameValue.KeyValueData>(Chains.ICommand`2 < Services.Communication.DataStructures.NameValue.HashContext, Services.Communication.DataStructures.NameValue.KeyValueData >) < 0x0005f >\nat Chains.Context`1 < Services.Communication.DataStructures.NameValue.HashContext >.Execute<Services.Communication.DataStructures.NameValue.KeyValueData>(Chains.ICommand`2 < Services.Communication.DataStructures.NameValue.HashContext, Services.Communication.DataStructures.NameValue.KeyValueData >) < 0x00063 >\nat(wrapper dynamic - method) object.CallSite.Target(System.Runtime.CompilerServices.Closure, System.Runtime.CompilerServices.CallSite, object, object) < 0x0010f >\nat Chains.Play.ExecuteActionAndGetResult.Execute(Chains.Play.DynamicExecutionContext) < 0x005a3 >\nat Chains.Context`1 < Chains.Play.DynamicExecutionContext >.InvokeAct<Chains.Play.ExecutionResultContext>(Chains.ICommand`2 < Chains.Play.DynamicExecutionContext, Chains.Play.ExecutionResultContext >) < 0x0005f >\nat Chains.Context`1 < Chains.Play.DynamicExecutionContext >.Execute<Chains.Play.ExecutionResultContext>(Chains.ICommand`2 < Chains.Play.DynamicExecutionContext, Chains.Play.ExecutionResultContext >) < 0x00063 >\nat Chains.Play.ExecuteActionFromSpecification.Execute(Chains.Play.DynamicExecutionContext) < 0x0005f >\nat Chains.Context`1 < Chains.Play.DynamicExecutionContext >.InvokeAct<Chains.Play.ExecutionResultContext>(Chains.ICommand`2 < Chains.Play.DynamicExecutionContext, Chains.Play.ExecutionResultContext >) < 0x0005f >\nat Chains.Context`1 < Chains.Play.DynamicExecutionContext >.Execute<Chains.Play.ExecutionResultContext>(Chains.ICommand`2 < Chains.Play.DynamicExecutionContext, Chains.Play.ExecutionResultContext >) < 0x00063 >\nat Services.Communication.Protocol.ProtocolServerLogic.ApplyDataOnExecutionChain(Chains.Play.DynamicExecutionContext, Chains.Play.ExecutableCommandSpecification[]) < 0x00083 >\nat Services.Communication.Protocol.ProtocolServerLogic.ApplyDataAndReturn(Chains.Play.DynamicExecutionContext, Chains.Play.ExecutableCommandSpecification[], bool) < 0x000e3 >\n\",\"RemoteStackTraceString\":null,\"RemoteStackIndex\":0,\"HResult\":-2146233087,\"Source\":\"mscorlib\",\"ExceptionMethod\":null,\"Data\":null},\"DataStructureVersionNumber\":1}";

            var deserializedData =
                DeserializableSpecification<DataSpecification>.DeserializeFromJson(jsonData);

            Assert.NotNull(deserializedData);
            Assert.IsType<KeyNotFoundException>(deserializedData.Data);
        }
    }
}
