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
            NamedPipeClientContext namedPipeClientContext;

            if (!string.IsNullOrWhiteSpace(commonInstrumentationParameters.PipeName))
            {
                namedPipeClientContext = new NamedPipeClientContext(commonInstrumentationParameters.PipeName, commonInstrumentationParameters.TimeoutInMilliseconds);
                WriteObject(namedPipeClientContext.Connect(GenerateCommand()));
                return;
            }
            
            // TODO: shared container maps for common configuration

            if (!string.IsNullOrWhiteSpace(commonInstrumentationParameters.NodeId))
            {
                var namedPipe = string.Join(".", commonInstrumentationParameters.ServiceName,
                    commonInstrumentationParameters.NodeId);
                namedPipeClientContext =
                    new NamedPipeClientContext(namedPipe, commonInstrumentationParameters.TimeoutInMilliseconds);
                namedPipeClientContext.Connect(new SetupCallData(commonInstrumentationParameters));
                WriteObject(namedPipeClientContext.Connect(GenerateCommand()));
            }
            else
            {
                var namedPipe = $"serviceable.objects/instrumentation/{commonInstrumentationParameters.ServiceName}";
                namedPipeClientContext =
                    new NamedPipeClientContext(namedPipe, commonInstrumentationParameters.TimeoutInMilliseconds);
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
