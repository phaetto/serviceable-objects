namespace Serviceable.Objects.IO.NamedPipes.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO.Pipes;
    using System.Linq;
    using Composition.Graph;
    using Newtonsoft.Json;
    using Remote;
    using Remote.Serialization;
    using Remote.Serialization.Streaming;

    public sealed class NamedPipeClientContext: Context<NamedPipeClientContext>, IGraphFlowExecutionSink
    {
        // TODO: make client a real proxy context
        private readonly string namedPipe;

        private readonly int timeoutInMilliseconds;
        private readonly StreamSession streamSession = new StreamSession();

        public NamedPipeClientContext(string namedPipe, int timeoutInMilliseconds)
        {
            this.namedPipe = namedPipe;
            this.timeoutInMilliseconds = timeoutInMilliseconds;
        }

        public object Send(IReproducible command)
        {
            using (var namedPipeClientStream = new NamedPipeClientStream(".", namedPipe, PipeDirection.InOut))
            {
                namedPipeClientStream.Connect(timeoutInMilliseconds);

                var specification = command.GetInstanceSpec();
                streamSession.Write(namedPipeClientStream, JsonConvert.SerializeObject(specification));

                namedPipeClientStream.WaitForPipeDrain();

                if (command is IRemotable) // TODO: abstract this to a common command executioner?
                {
                    do
                    {
                        streamSession.Read(namedPipeClientStream);
                    } while (streamSession.IsCommandBufferWaitingForCompletion);

                    if (streamSession.CommandsTextReadyToBeParsedQueue.TryDequeue(out var replyString))
                    {
                        if (!string.IsNullOrWhiteSpace(replyString))
                        {
                            var commandSpecificationService = new CommandSpecificationService();
                            var commandResultSpecification = JsonConvert.DeserializeObject<CommandResultSpecification[]>(replyString);
                            return commandSpecificationService.CreateResultDataFromCommandSpecification(commandResultSpecification.First());
                        }
                    }
                }

                return null;
            }
        }

        // TODO: proxy might not need this
        public dynamic CustomCommandExecute(GraphContext graphContext, string executingNodeId, dynamic commandApplied,
            Stack<EventResult> eventResults)
        {
            if (commandApplied is IReproducible reproducible)
            {
                return this.Send(reproducible);
            }

            throw new NotSupportedException($"Command {commandApplied.GetType().AssemblyQualifiedName} was not IReproducible");
        }
    }
}
