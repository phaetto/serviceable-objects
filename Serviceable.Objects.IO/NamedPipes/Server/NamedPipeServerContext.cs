namespace Serviceable.Objects.IO.NamedPipes.Server
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Threading.Tasks;
    using Commands;
    using Composition.Graph.Events;
    using Composition.Graph.Stages.Configuration;
    using Composition.Graph.Stages.Initialization;
    using Configuration;
    using Exceptions;
    using Remote.Composition.Configuration;
    using Remote.Serialization;
    using Remote.Serialization.Streaming;

    public sealed class NamedPipeServerContext : ConfigurableContext<NamedPipeServerConfiguration, NamedPipeServerContext>, IInitializeStageFactory
    {
        private readonly StreamSession streamSession = new StreamSession();
        internal Task ServerTask;

        public NamedPipeServerContext()
        {
        }

        public NamedPipeServerContext(NamedPipeServerConfiguration configuration) : base(configuration)
        {
        }

        public NamedPipeServerContext(IConfigurationSource configurationSource) : base(configurationSource)
        {
        }

        public dynamic GenerateInitializeCommand()
        {
            return new StartServer();
        }

        internal void RunServerAndBlock()
        {
            Check.ArgumentNull(Configuration, nameof(Configuration));

            using (var namedPipeServerStream = new NamedPipeServerStream(Configuration.PipeName, PipeDirection.InOut))
            {
                while (true)
                {
                    namedPipeServerStream.WaitForConnection();

                    try
                    {
                        while (true)
                        {
                            // Let the stream protocol get the pieces
                            streamSession.Read(namedPipeServerStream);

                            if (streamSession.CommandsTextReadyToBeParsedQueue.TryDequeue(out var commandString))
                            {
                                var result = PushDataAsCommand(commandString);

                                if (result != null)
                                {
                                    var dataSpecifications = result.Where(x => x != null).Select(x => new DataSpecification
                                    {
                                        Data = x,
                                        DataType = x.GetType().AssemblyQualifiedName
                                    });

                                    streamSession.Write(namedPipeServerStream, SerializableSpecification.SerializeManyToJson(dataSpecifications.ToArray()));
                                }
                            }

                            namedPipeServerStream.WaitForPipeDrain();
                        }
                    }
                    catch (IOException)
                    {
                        namedPipeServerStream.Disconnect();
                        // Reconnect
                    }
                }
            }
        }

        private IEnumerable<object> PushDataAsCommand(string data)
        {
            var spec = DeserializableSpecification<ExecutableCommandSpecification>.DeserializeFromJson(data);
            var command = spec.CreateFromSpec();

            var eventResults =
                PublishCommandEventAndGetResults(new GraphFlowEventPushControlApplyCommandInsteadOfEvent(command))
                    .Where(x => x.ResultObject != null).ToList();

            if (eventResults.Count > 0)
            {
                var results = eventResults.Select(x => x.ResultObject);
                return results;
            }

            return null;
        }
    }
}
