namespace Serviceable.Objects.Instrumentation
{
    using System.Management.Automation;
    using CommonParameters;
    using IO.NamedPipes.Client;
    using Remote;
    using Server.Commands;

    public abstract class InstrumentationCommandCmdlet<T> : Cmdlet, IDynamicParameters
        where T : IReproducible
    {
        private readonly CommonInstrumentationParameters commonInstrumentationParameters = new CommonInstrumentationParameters();

        public abstract T GenerateCommand();

        protected override void ProcessRecord()
        {
            if (!string.IsNullOrWhiteSpace(commonInstrumentationParameters.PipeName))
            {
                var namedPipeClientContext = new NamedPipeClientContext(commonInstrumentationParameters.PipeName, commonInstrumentationParameters.TimeoutInMilliseconds);
                WriteObject(namedPipeClientContext.Connect(GenerateCommand()));
            }
            else if (!string.IsNullOrWhiteSpace(commonInstrumentationParameters.ServiceContainerName))
            {
                // TODO: shared container maps for common configuration
                var namedPipe = string.Join(".", commonInstrumentationParameters.ServiceContainerName, "self", "testpipe");
                var namedPipeClientContext = new NamedPipeClientContext(namedPipe, commonInstrumentationParameters.TimeoutInMilliseconds);
                namedPipeClientContext.Connect(new SetupCallData(commonInstrumentationParameters));
                WriteObject(namedPipeClientContext.Connect(GenerateCommand()));
            }
        }

        public object GetDynamicParameters()
        {
            return commonInstrumentationParameters;
        }
    }
}
