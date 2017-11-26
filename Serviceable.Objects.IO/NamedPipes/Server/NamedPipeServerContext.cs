namespace Serviceable.Objects.IO.NamedPipes.Server
{
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
    using Newtonsoft.Json;
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

                                streamSession.Write(namedPipeServerStream, result);
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

        private string PushDataAsCommand(string data)
        {
            var spec = DeserializableSpecification<ExecutableCommandSpecification>.DeserializeFromJson(data);
            var command = spec.CreateFromSpec();

            var eventResults =
                PublishCommandEventAndGetResults(new GraphFlowEventPushControlApplyCommandInsteadOfEvent(command))
                    .Where(x => x.ResultObject != null).ToList();

            if (eventResults.Count > 0)
            {
                var results = eventResults.Select(x => x.ResultObject);
                return JsonConvert.SerializeObject(results);
            }

            return null;
        }
    }
}
