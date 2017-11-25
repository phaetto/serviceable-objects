using System.Linq;
using Newtonsoft.Json;
using Serviceable.Objects.Composition.Events;
using Serviceable.Objects.Remote.Serialization;
using System.IO.Pipes;
using System.IO;
using System.Threading.Tasks;
using Serviceable.Objects.Remote.Serialization.Streaming;

namespace Serviceable.Objects.IO.NamedPipes
{
    public sealed class NamedPipeServerContext : Context<NamedPipeServerContext>
    {
        private readonly string namedPipe; // TODO: formulate how we get the input redirection
        private readonly StreamSession streamSession = new StreamSession();
        private readonly Task serverTask;

        public NamedPipeServerContext()
        {
            serverTask = Task.Run(() => RunServerAndBlock());
        }

        // TODO: how to run this on initialization on graph, and still be decoupled?
        private void RunServerAndBlock()
        {
            using (var namedPipeServerStream = new NamedPipeServerStream("testpipe", PipeDirection.InOut))
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
