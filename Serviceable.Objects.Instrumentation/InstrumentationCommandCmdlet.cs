using System.Management.Automation;
using Serviceable.Objects.Remote;

namespace Serviceable.Objects.Instrumentation
{
    using CommonParameters;
    using IO.NamedPipes.Client;

    public abstract class InstrumentationCommandCmdlet<T> : Cmdlet, IDynamicParameters
        where T : IReproducible
    {
        private readonly CommonInstrumentationParameters commonInstrumentationParameters = new CommonInstrumentationParameters();

        public abstract T GenerateCommand();

        protected override void ProcessRecord()
        {
            var namedPipeClientContext = new NamedPipeClientContext();
            namedPipeClientContext.Connect(GenerateCommand());
        }

        public object GetDynamicParameters()
        {
            return commonInstrumentationParameters;
        }
    }
}
