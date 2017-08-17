﻿namespace Serviceable.Objects.Remote.Tests.Classes
{
    using Serviceable.Objects.Tests.Classes;

    public class ReproducibleTestAction : ReproducibleActionWithData<ContextForTest, ContextForTest, ReproducibleTestData>
    {
        public ReproducibleTestAction(ReproducibleTestData testData)
            : base(testData)
        {
            Data.DomainName = "Starting text";
        }

        public override ContextForTest Execute(ContextForTest context)
        {
            context.ContextVariable = Data.ChangeToValue;

            return context;
        }
    }
}