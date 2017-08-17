namespace Serviceable.Objects.Tests.Classes
{
    public class ActionForTestReturnsNull: ICommand<ContextForTest, ContextForTest>
    {
        public ContextForTest Execute(ContextForTest context)
        {
            return null;
        }
    }
}
