namespace Serviceable.Objects.Tests
{
    using System.Collections.Generic;
    using Classes;
    using Xunit;

    public class ContextTest
    {
        [Fact]
        public void Context_WhenDoingAction_ThenContextIsBeenUpdated()
        {
            var result =
                new ContextForTest().Execute(new ActionForTest("value 1"))
                                    .Execute(new ActionForTest("value 2"))
                                    .Execute(new ActionForTest("value 3"));

            Assert.Equal("value 3", result.ContextVariable);

            result =
                result.Execute(new ActionForTest("value 4"))
                      .Execute(new ActionForTest("value 5"))
                      .Execute(new ActionForTest("value 6"));

            Assert.Equal("value 6", result.ContextVariable);
        }

        [Fact]
        public void Execute_WhenActionWithMultipleContextsIsExecuted_ThenContextIsBeenUpdated()
        {
            var result = new ContextForTest().Execute<ContextForTest>(new ActionThatPlaysInTwoContexts("value"));

            Assert.NotNull(result);
            Assert.Equal("value", result.ContextVariable);
        }

        [Fact]
        public void Execute_WhenAContextMightReturnNull_ThenTheActionsAfterNullAreNotExecuted()
        {
            var result = new ContextForTest();

            result.Execute(new ActionForTest("value 1"))
                ?.Execute(new ActionForTest("value 2"))
                ?.Execute(new ActionForTest("value 3"))
                ?.Execute(new ActionForTestReturnsNull())
                ?.Execute(new ActionForTest("value 4"))
                ?.Execute(new ActionForTest("value 5"))
                ?.Execute(new ActionForTest("value 6"));

            Assert.Equal("value 3", result.ContextVariable);
        }

        [Fact]
        public void Execute_WhenDoingActionWithEvents_ThenCommandEventsAreReceived()
        {
            var contextForTest = new ContextForTest();
            var eventList = new List<ActionForTestEventProducerCalledEvent>();

            contextForTest.CommandEventPublished += eventPublished =>
            {
                eventList.Add((ActionForTestEventProducerCalledEvent)eventPublished);
                return null;
            };

            var result =
                contextForTest.Execute(new ActionForTestEventProducer("value 1"))
                    .Execute(new ActionForTestEventProducer("value 2"))
                    .Execute(new ActionForTestEventProducer("value 3"));

            Assert.Equal(3, eventList.Count);

            Assert.Null(eventList[0].LastValue);
            Assert.Equal("value 1", eventList[0].ChangedTo);

            Assert.Equal("value 1", eventList[1].LastValue);
            Assert.Equal("value 2", eventList[1].ChangedTo);

            Assert.Equal("value 2", eventList[2].LastValue);
            Assert.Equal("value 3", eventList[2].ChangedTo);

            Assert.Equal("value 3", result.ContextVariable);
        }
    }
}
