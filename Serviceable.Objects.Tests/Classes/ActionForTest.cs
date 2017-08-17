namespace Serviceable.Objects.Tests.Classes
{
    public class ActionForTest: ICommand<ContextForTest, ContextForTest>
    {
        public readonly string ValueToChangeTo;

        public ActionForTest(string valueToChangeTo)
        {
            ValueToChangeTo = valueToChangeTo;
        }

        public ContextForTest Execute(ContextForTest context)
        {
            context.ContextVariable = ValueToChangeTo;
            return context;
        }
    }
}
