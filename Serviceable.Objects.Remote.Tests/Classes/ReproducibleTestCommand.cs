namespace Serviceable.Objects.Remote.Tests.Classes
{
    using Objects.Tests.Classes;

    public class ReproducibleTestCommand : ReproducibleCommandWithData<ContextForTest, ContextForTest, ReproducibleTestData>
    {
        public ReproducibleTestCommand(ReproducibleTestData testData)
            : base(testData)
        {
            //ResultDataAsJson.DomainName = "Starting text";
        }

        public override ContextForTest Execute(ContextForTest context)
        {
            context.ContextVariable = Data.ChangeToValue;

            return context;
        }
    }
}
