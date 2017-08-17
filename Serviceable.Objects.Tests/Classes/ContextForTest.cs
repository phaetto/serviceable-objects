namespace Serviceable.Objects.Tests.Classes
{
    public class ContextForTest : Context<ContextForTest>
    {
        public string ContextVariable = null;

        public bool HasBeenChecked = false;
    }
}
