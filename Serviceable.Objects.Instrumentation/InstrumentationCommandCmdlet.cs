using System.Management.Automation;
using Serviceable.Objects.IO.NamedPipes;
using Serviceable.Objects.Remote;

namespace Serviceable.Objects.Instrumentation
{
    public abstract class InstrumentationCommandCmdlet<T> : Cmdlet
        where T : IReproducible
    {
        public abstract T GenerateCommand();

        protected override void ProcessRecord()
        {
            var namedPipeClientContext = new NamedPipeClientContext();
            namedPipeClientContext.Connect(GenerateCommand());
        }
    }
}
