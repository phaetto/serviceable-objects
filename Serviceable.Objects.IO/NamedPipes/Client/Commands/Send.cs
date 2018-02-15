namespace Serviceable.Objects.IO.NamedPipes.Client.Commands
{
    using System.Linq;
    using Remote;
    using Remote.Serialization;

    public sealed class Send : ICommand<NamedPipeClientContext, object>
    {
        private readonly IReproducible command;

        public Send(IReproducible command)
        {
            this.command = command;
        }

        public object Execute(NamedPipeClientContext context)
        {
            var commandSpecificationService = new CommandSpecificationService();
            var commandExecutionResultSpecification = context.Execute(new SendAndGetCommandExecutionResultSpecification(command))
                .Take(1)
                .FirstOrDefault();

            return commandExecutionResultSpecification == null
                ? null
                : commandSpecificationService.CreateResultDataFromCommandSpecification(commandExecutionResultSpecification);
        }
    }
}