namespace Serviceable.Objects.Instrumentation.Server.Commands
{
    using CommonParameters;
    using Remote;

    public sealed class SetupCallData : ReproducibleCommandWithData<InstrumentationServer, InstrumentationServer, CommonInstrumentationParameters>
    {
        public SetupCallData(CommonInstrumentationParameters data) : base(data)
        {
        }

        public override InstrumentationServer Execute(InstrumentationServer context)
        {
            context.CommonInstrumentationParameters = Data;
            return context;
        }
    }
}