using System.Management.Automation;
using Serviceable.Objects.Remote;

namespace Serviceable.Objects.Instrumentation
{
    public abstract class InstrumentationCommandCmdlet<T> : Cmdlet
        where T : IReproducible
    {
        public abstract T GenerateCommand();

        protected override void ProcessRecord()
        {
            // Open named-pipes
            // Send command over the wire
        }
    }
}
