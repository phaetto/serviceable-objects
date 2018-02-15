namespace Serviceable.Objects.Remote.Tests
{
    using System;
    using Classes;
    using Objects.Tests.Classes;
    using Serialization;
    using Serialization.Exceptions;
    using Xunit;

    public class SerializationTest
    {
        [Fact]
        public void CommandSpecification_WhenCommandIsSerializedToJsonAndDeserialized_ThenTheObjectIsTheSame()
        {
            var serializableSpecificationService = new CommandSpecificationService();
            var reproducibleCommand = new ReproducibleTestCommand(new ReproducibleTestData
            {
                ChangeToValue = "value",
                DomainName = "domain"
            });

            var serializableCommandSpecification =
                serializableSpecificationService.CreateSpecificationForCommand(reproducibleCommand);

            var deserializedCommand = serializableSpecificationService.CreateCommandFromSpecification<ReproducibleTestCommand>(serializableCommandSpecification);

            Assert.Equal("value", deserializedCommand.Data.ChangeToValue);
            Assert.Equal("domain", deserializedCommand.Data.DomainName);
        }

        [Fact]
        public void CommandSpecification_WhenRemotableDataIsSerializedToJsonAndDeserialized_ThenTheObjectIsTheSame()
        {
            var serializableSpecificationService = new CommandSpecificationService();
            var remotableTestCommand = new RemotableTestCommand(new ReproducibleTestData
            {
                ChangeToValue = "value",
                DomainName = "domain"
            });
            var contextForTest = new ContextForTest();

            var remotableResult = contextForTest.Execute(remotableTestCommand);

            var commandResultSpecification =
                serializableSpecificationService.CreateSpecificationForCommandResult(remotableTestCommand.GetType(), remotableResult);

            var data = serializableSpecificationService.CreateResultDataFromCommandSpecification<ReproducibleTestData>(commandResultSpecification);

            Assert.Equal("value", data.ChangeToValue);
            Assert.Equal("Starting value", data.DomainName);
        }

        [Fact]
        public void CommandSpecification_WhenRemotableDataWithErrorIsDeserialized_ThenTheObjectIsAnException()
        {
            var exception = new InvalidOperationException("This is so invalid.");
            var serializableSpecificationService = new CommandSpecificationService();
            var commandResultSpecification = new CommandResultSpecification
            {
                CommandType = typeof(RemotableTestCommand).AssemblyQualifiedName,
                ContainsError = true,
                Exception = new CommandSpecificationExceptionCarrier
                {
                    Message = exception.Message,
                    RealExceptionType = exception.GetType().FullName,
                    StackTrace = exception.StackTrace
                }
            };

            var data = serializableSpecificationService.CreateResultDataFromCommandSpecification(commandResultSpecification);

            Assert.IsNotType<ReproducibleTestData>(data);
            Assert.IsType<Exception>(data);
            var exceptionReturned = data as Exception;
            Assert.StartsWith("This is so invalid.", exceptionReturned.Message);
        }
    }
}
