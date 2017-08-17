namespace Serviceable.Objects.Remote.Tests.Classes
{
    using System.Threading.Tasks;
    using Serviceable.Objects.Tests.Classes;

    public class ReproducibleTestActionAsync : ReproducibleActionWithData<ContextForTest, Task<ContextForTest>, ReproducibleTestData>
    {
        public ReproducibleTestActionAsync(ReproducibleTestData testData)
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
