﻿namespace Serviceable.Objects.Remote.Tests.Classes
{
    using Objects.Tests.Classes;

    public class ReproducibleTestAction2 : ReproducibleCommandWithData<ContextForTest2, ContextForTest2, ReproducibleTestData>
    {
        public ReproducibleTestAction2(ReproducibleTestData testData)
            : base(testData)
        {
            Data.DomainName = "Starting text";
        }

        public override ContextForTest2 Execute(ContextForTest2 context)
        {
            context.ContextVariable = Data.ChangeToValue;

            return context;
        }
    }
}
