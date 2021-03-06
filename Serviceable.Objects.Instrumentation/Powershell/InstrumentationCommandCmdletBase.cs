namespace Serviceable.Objects.Instrumentation.Powershell
{
    using System;
    using System.Management.Automation;
    using CommonParameters;
    using IO.NamedPipes.Client;
    using Remote;
    using Remote.Dependencies;
    using Server.Commands;
    using Remote.Proxying;

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
                ExecuteAndReturnReply(namedPipeClientContext, GenerateCommand());
                return;
            }

            var namedPipe = SetupServer.WellknownPipeFormat(
                commonInstrumentationParameters.ServiceOrchestrator,
                commonInstrumentationParameters.ServiceName);
            namedPipeClientContext =
                new NamedPipeClientContext(namedPipe, commonInstrumentationParameters.TimeoutInMilliseconds);
            namedPipeClientContext.Execute(new SetupCallData(commonInstrumentationParameters));
            ExecuteAndReturnReply(namedPipeClientContext, GenerateCommand());
        }

        public object GetDynamicParameters()
        {
            return commonInstrumentationParameters;
        }

        private void ExecuteAndReturnReply(NamedPipeClientContext namedPipeClientContext, TCommand command)
        {
            var result = namedPipeClientContext.Execute(namedPipeClientContext.GenerateProxyCommandForGenericExecution(command));
            if (result is Exception exception)
            {
                ThrowTerminatingError(new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.InvalidOperation, this));
            }
            else
            {
                WriteObject(result);
            }
        }
    }
}