namespace Serviceable.Objects.Instrumentation.Powershell
{
    using System.Management.Automation;
    using CommonParameters;
    using IO.NamedPipes.Client;
    using Remote;
    using Remote.Dependencies;
    using Server.Commands;

    public abstract class InstrumentationCommandCmdletBase<TCommand> : Cmdlet, IDynamicParameters
        where TCommand : IReproducible
    {
        private readonly CommonInstrumentationParameters commonInstrumentationParameters = new CommonInstrumentationParameters();

        public virtual TCommand GenerateCommand()
        {
            return (TCommand)Types.CreateObjectWithParameters(typeof(TCommand));
        }

        protected override void ProcessRecord()
        {
            NamedPipeClientContext namedPipeClientContext;

            if (!string.IsNullOrWhiteSpace(commonInstrumentationParameters.PipeName))
            {
                namedPipeClientContext = new NamedPipeClientContext(commonInstrumentationParameters.PipeName, commonInstrumentationParameters.TimeoutInMilliseconds);
                WriteObject(namedPipeClientContext.Send(GenerateCommand()));
                return;
            }

            var namedPipe = SetupServer.WellknownPipeFormat(
                commonInstrumentationParameters.ServiceOrchestrator,
                commonInstrumentationParameters.ServiceName);
            namedPipeClientContext =
                new NamedPipeClientContext(namedPipe, commonInstrumentationParameters.TimeoutInMilliseconds);
            namedPipeClientContext.Send(new SetupCallData(commonInstrumentationParameters));
            WriteObject(namedPipeClientContext.Send(GenerateCommand()));
        }

        public object GetDynamicParameters()
        {
            // TODO: Change to RuntimeDefinedParameterDictionary so we can provide dynamic responses
            return commonInstrumentationParameters;
        }
    }
}