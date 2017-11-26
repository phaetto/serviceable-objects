namespace Serviceable.Objects.IO.NamedPipes
{
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Threading.Tasks;
    using Composition.Graph.Events;
    using Composition.Graph.Stages.Configuration;
    using Composition.Graph.Stages.Initialization;
    using Exceptions;
    using Newtonsoft.Json;
    using Remote.Composition.Configuration;
    using Remote.Serialization;
    using Remote.Serialization.Streaming;
    using State;

    public sealed class NamedPipeServerContext : ConfigurableContext<NamedPipeServerState, NamedPipeServerContext>, IInitialize
    {
        private readonly StreamSession streamSession = new StreamSession();
        private Task serverTask;

        public NamedPipeServerContext()
        {
        }

        public NamedPipeServerContext(NamedPipeServerState configuration) : base(configuration)
        {
        }

        public NamedPipeServerContext(IConfigurationSource configurationSource) : base(configurationSource)
        {
        }

        public void Initialize()
        {
            serverTask = Task.Run(() => RunServerAndBlock());
        }

        private void RunServerAndBlock()
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
