namespace Serviceable.Objects.Remote.Tests.Classes
{
    using System.Threading.Tasks;
    using Objects.Tests.Classes;

    public class RemotableTestCommandAsync : RemotableCommandWithData<ReproducibleTestData, Task<ReproducibleTestData>, ContextForTest>
    {
        public RemotableTestCommandAsync(ReproducibleTestData testData)
            : base(testData)
        {
            Data.DomainName = "Starting value";
        }

        public override async Task<ReproducibleTestData> Execute(ContextForTest context)
        {
            context.ContextVariable = Data.ChangeToValue;

            await Task.Delay(1);

            return Data;
        }
    }
}
