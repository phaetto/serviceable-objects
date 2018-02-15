namespace Serviceable.Objects.IO.NamedPipes.Client.Commands
{
    using System.Collections.Generic;
    using System.IO.Pipes;
    using System.Linq;
    using Newtonsoft.Json;
    using Remote;
    using Remote.Serialization;

    public sealed class SendAndGetCommandExecutionResultSpecification : ICommand<NamedPipeClientContext, IEnumerable<CommandExecutionResultSpecification>>
    {
        private readonly IReproducible command;

        public SendAndGetCommandExecutionResultSpecification(IReproducible command)
        {
            this.command = command;
        }

        public IEnumerable<CommandExecutionResultSpecification> Execute(NamedPipeClientContext context)
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
                        var commandSpecificationService = new CommandSpecificationService();
                        var commandResultSpecification = JsonConvert.DeserializeObject<CommandResultSpecification[]>(replyString);
                        return commandResultSpecification
                            .Select(x => commandSpecificationService.CreateResultDataFromCommandSpecification(x))
                            .Cast<CommandExecutionResultSpecification>();
                    }
                }

                return new CommandExecutionResultSpecification[0];
            }
        }
    }
}