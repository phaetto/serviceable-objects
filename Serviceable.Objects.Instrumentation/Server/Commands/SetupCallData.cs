namespace Serviceable.Objects.Instrumentation.Server.Commands
{
    using CommonParameters;
    using Remote;

    public sealed class SetupCallData : ReproducibleCommandWithData<InstrumentationServerContext, InstrumentationServerContext, CommonInstrumentationParameters>
    {
        public SetupCallData(CommonInstrumentationParameters data) : base(data)
        {
        }

        public override InstrumentationServerContext Execute(InstrumentationServerContext context)
        {
            context.CommonInstrumentationParameters = Data;
            return context;
        }
    }
}