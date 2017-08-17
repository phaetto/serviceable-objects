namespace Serviceable.Objects.Tests.Classes
{
    public class ActionForTest2: ICommand<ContextForTest2, ContextForTest2>
    {
        public readonly string ValueToChangeTo;

        public ActionForTest2(string valueToChangeTo)
        {
            ValueToChangeTo = valueToChangeTo;
        }

        public ContextForTest2 Execute(ContextForTest2 context)
        {
            context.ContextVariable = ValueToChangeTo;
            return context;
        }
    }
}
