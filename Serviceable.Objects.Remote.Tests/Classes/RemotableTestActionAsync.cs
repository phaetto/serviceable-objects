namespace Serviceable.Objects.Remote.Tests.Classes
{
    using System.Threading.Tasks;
    using Serviceable.Objects.Tests.Classes;

    public class RemotableTestActionAsync : RemotableActionWithData<ReproducibleTestData, Task<ReproducibleTestData>, ContextForTest>
    {
        public RemotableTestActionAsync(ReproducibleTestData testData)
            : base(testData)
        {
            Data.DomainName = "Starting value";
        }

        public override Task<ReproducibleTestData> Execute(ContextForTest context)
        {
            context.ContextVariable = Data.ChangeToValue;
            return Task.FromResult(Data);
        }
    }
}
