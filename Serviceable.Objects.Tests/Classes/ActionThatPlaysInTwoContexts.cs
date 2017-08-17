namespace Serviceable.Objects.Tests.Classes
{
    public sealed class ActionThatPlaysInTwoContexts : 
        ICommand<ContextForTest, ContextForTest>,
        ICommand<ContextForTest2, ContextForTest2>
    {
        public readonly string ValueToChangeTo;

        public ActionThatPlaysInTwoContexts(string valueToChangeTo)
        {
            ValueToChangeTo = valueToChangeTo;
        }

        public ContextForTest Execute(ContextForTest context)
        {
            context.ContextVariable = ValueToChangeTo;
            return context;
        }

        public ContextForTest2 Execute(ContextForTest2 context)
        {
            context.ContextVariable = ValueToChangeTo;
            return context;
        }
    }
}
