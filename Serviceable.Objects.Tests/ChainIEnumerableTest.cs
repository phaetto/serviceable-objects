namespace Serviceable.Objects.Tests
{
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Serviceable.Objects.Tests.Classes;
    using Xunit;

    public class ChainIEnumerableTest
    {
        private const int TimeBetweenValueUpdates = 10;

        [Fact]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void Chain_WhenInfiniteListIsProvided_ThenSuccesfullyAppliesItems()
        {
            var blockingColection = new BlockingCollection<ActionForTest>(new ConcurrentQueue<ActionForTest>(), 10);
            var contextForTest = new ContextForTest();

            Task.Delay(10).ContinueWith(x =>
                {
                    blockingColection.Add(new ActionForTest("Value 1"));
                }).ContinueWith(x => Task.Delay(TimeBetweenValueUpdates).ContinueWith(y =>
                {
                    blockingColection.Add(new ActionForTest("Value 2"));
                })).Unwrap().ContinueWith(x => Task.Delay(TimeBetweenValueUpdates).ContinueWith(y =>
                {
                    blockingColection.Add(new ActionForTest("Value 3"));
                })).Unwrap().ContinueWith(x => Task.Delay(TimeBetweenValueUpdates).ContinueWith(y =>
                {
                    // Signals that there are no more items
                    blockingColection.CompleteAdding();
                })).Unwrap();

            var infiniteStream = contextForTest.Execute(blockingColection.GetConsumingEnumerable());

            // This will block until there are no more items in the enum
            foreach (var updatedContext in infiniteStream)
            {
                // Called each time an item gets in
            }

            Assert.Equal("Value 3", contextForTest.ContextVariable);
        }

        [Fact]
        public async Task Chain_WhenInfiniteListIsProvidedAsync_ThenSuccesfullyAppliesItems()
        {
            var blockingColection = new BlockingCollection<ActionForTest>(new ConcurrentQueue<ActionForTest>(), 10);

            var contextForTest = new ContextForTest();

            // This will produce an infinite stream and enumerate it
            var task = contextForTest.ForceExecuteAsync(blockingColection.GetConsumingEnumerable());

            blockingColection.Add(new ActionForTest("Value 1"));
            blockingColection.Add(new ActionForTest("Value 2"));
            blockingColection.Add(new ActionForTest("Value 3"));

            blockingColection.CompleteAdding();
            await task;

            Assert.Equal("Value 3", contextForTest.ContextVariable);
        }

        [Fact]
        public async Task Chain_WhenInfiniteListIsProvidedAsync_ThenSuccesfullyAppliesItemsInEventLikeMannerRightAfterTheStreamItem()
        {
            var blockingColection = new BlockingCollection<ActionForTest>(new ConcurrentQueue<ActionForTest>(), 10);

            var contextForTest = new ContextForTest();

            // This will produce an infinite stream and enumerate it
#pragma warning disable 4014
            contextForTest.ForceExecuteAsync(blockingColection.GetConsumingEnumerable());
#pragma warning restore 4014

            blockingColection.Add(new ActionForTest("Value 1"));

            await Task.Delay(TimeBetweenValueUpdates);
            Assert.Equal("Value 1", contextForTest.ContextVariable);

            blockingColection.Add(new ActionForTest("Value 2"));

            await Task.Delay(TimeBetweenValueUpdates);
            Assert.Equal("Value 2", contextForTest.ContextVariable);

            blockingColection.Add(new ActionForTest("Value 3"));

            await Task.Delay(TimeBetweenValueUpdates);
            Assert.Equal("Value 3", contextForTest.ContextVariable);
        }
    }
}
