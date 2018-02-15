namespace Serviceable.Objects.IO.NamedPipes.Client.Commands
{
    using System.Collections.Generic;
    using System.IO.Pipes;
    using Newtonsoft.Json;
    using Remote;
    using Remote.Serialization;

    public sealed class SendAndGetCommandResultSpecification : ICommand<NamedPipeClientContext, IEnumerable<CommandResultSpecification>>
    {
        private readonly IReproducible command;

        public SendAndGetCommandResultSpecification(IReproducible command)
        {
            this.command = command;
        }

        public IEnumerable<CommandResultSpecification> Execute(NamedPipeClientContext context)
        {
            using (var namedPipeClientStream = new NamedPipeClientStream(".", context.NamedPipe, PipeDirection.InOut))
            {
                namedPipeClientStream.Connect(context.TimeoutInMilliseconds);

                var specification = command.GetInstanceSpec();
                context.StreamSession.Write(namedPipeClientStream, JsonConvert.SerializeObject(specification));

                namedPipeClientStream.WaitForPipeDrain();

                do
                {
                    context.StreamSession.Read(namedPipeClientStream);
                } while (context.StreamSession.IsCommandBufferWaitingForCompletion);

                if (context.StreamSession.CommandsTextReadyToBeParsedQueue.TryDequeue(out var replyString))
                {
                    if (!string.IsNullOrWhiteSpace(replyString))
                    {
                        return JsonConvert.DeserializeObject<CommandResultSpecification[]>(replyString);
                    }
                }

                return new CommandResultSpecification[0];
            }
        }
    }
}