namespace Serviceable.Objects.Remote.Tests.Classes
{
    using System.Threading.Tasks;
    using Objects.Tests.Classes;

    public class ReproducibleTestCommandAsync : ReproducibleCommandWithData<ContextForTest, Task<ContextForTest>, ReproducibleTestData>
    {
        public ReproducibleTestCommandAsync(ReproducibleTestData testData)
            : base(testData)
        {
            Data.DomainName = "Starting text";
        }

        public override Task<ContextForTest> Execute(ContextForTest context)
        {
            context.ContextVariable = Data.ChangeToValue;

            return Task.FromResult(context);
        }
    }
}
