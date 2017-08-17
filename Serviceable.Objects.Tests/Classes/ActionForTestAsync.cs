namespace Serviceable.Objects.Tests.Classes
{
    using System.Threading.Tasks;

    public class ActionForTestAsync: ICommand<ContextForTest, Task<ContextForTest>>
    {
        public readonly string ValueToChangeTo;

        public ActionForTestAsync(string valueToChangeTo)
        {
            ValueToChangeTo = valueToChangeTo;
        }

        public async Task<ContextForTest> Execute(ContextForTest context)
        {
            await Task.Delay(300);

            context.ContextVariable = ValueToChangeTo;

            return context;
        }
    }
}
