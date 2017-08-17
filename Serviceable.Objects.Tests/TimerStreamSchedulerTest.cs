namespace Serviceable.Objects.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Serviceable.Objects.Streams.Timer;
    using Serviceable.Objects.Tests.Classes;
    using Xunit;

    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    [SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
    public class TimerStreamSchedulerTest
    {
        [Fact]
        public void timerStreamScheduler_WhenSchedulingOneFutureCallback_ThenOneEventIsReturned()
        {
            var contextForTest = new ContextForTest();

            using (var timerStreamScheduler = new TimerStreamScheduler())
            {
                Assert.True(timerStreamScheduler.IsIdle);

                timerStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 100, TimerScheduledCallType.Once);

                var infiniteStream = contextForTest.Execute(timerStreamScheduler.SubscribeToInfiniteStream<ActionForTest>()).GetEnumerator();

                Assert.True(infiniteStream.MoveNext());

                Assert.Equal("timer hit", contextForTest.ContextVariable);
            }
        }

        [Fact]
        public void timerStreamScheduler_WhenRequestingAsInterface_ThenOneEventIsReturned()
        {
            var contextForTest = new ContextForTest();

            using (var timerStreamScheduler = new TimerStreamScheduler())
            {
                Assert.True(timerStreamScheduler.IsIdle);

                timerStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 100, TimerScheduledCallType.Once);

                var infiniteStream = contextForTest.Execute(timerStreamScheduler.SubscribeToInfiniteStream<ICommand<ContextForTest, ContextForTest>>()).GetEnumerator();

                Assert.True(infiniteStream.MoveNext());

                Assert.Equal("timer hit", contextForTest.ContextVariable);
            }
        }

        [Fact]
        public void timerStreamScheduler_WhenCancelling_ThenShouldReturnGracefully()
        {
            var contextForTest = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();

            using (var timerStreamScheduler = new TimerStreamScheduler())
            {
                var infiniteStream = contextForTest.Execute(timerStreamScheduler.SubscribeToInfiniteStream<ActionForTest>(cancellationTokenSource.Token)).GetEnumerator();

                Task.Delay(50).ContinueWith(x =>
                    {
                        cancellationTokenSource.Cancel();
                    });

                infiniteStream.MoveNext();
            }
        }

        [Fact]
        public void timerStreamScheduler_WhenSchedulingAndCancelling_ThenNoEventHasRun()
        {
            var contextForTest = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();
            using (var timerStreamScheduler = new TimerStreamScheduler())
            {
                timerStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 100, TimerScheduledCallType.Once);

                Task.Delay(10).ContinueWith(x =>
                    {
                        var infiniteStream =
                            contextForTest.Execute(timerStreamScheduler.SubscribeToInfiniteStream<ActionForTest>(cancellationTokenSource.Token))
                                .GetEnumerator();

                        Assert.False(infiniteStream.MoveNext());
                    });

                cancellationTokenSource.Cancel();

                Assert.NotEqual("timer hit", contextForTest.ContextVariable);
            }
        }

        [Fact]
        public async Task timerStreamScheduler_WhenSchedulingANumberOfEventsAndCancelling_ThenExactNumberOfEventsAreRunning()
        {
            var contextForTest = new ContextForTest();
            var contextForTest2 = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationTokenSource2 = new CancellationTokenSource();
            using (var timerStreamScheduler = new TimerStreamScheduler())
            {
#pragma warning disable 4014
                contextForTest.ForceExecuteAsync(timerStreamScheduler.SubscribeToInfiniteStream<ActionForTest>(cancellationTokenSource.Token));
                contextForTest2.ForceExecuteAsync(timerStreamScheduler.SubscribeToInfiniteStream<ActionForTest>(cancellationTokenSource2.Token));
#pragma warning restore 4014

                timerStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit 1"), 50, TimerScheduledCallType.Once);
                timerStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit 2"), 100, TimerScheduledCallType.Once);
                timerStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit 3"), 150, TimerScheduledCallType.Once);
                timerStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit 4"), 200, TimerScheduledCallType.Once);

                await Task.Delay(120).ContinueWith(x =>
                    {
                        // Cancel after the 2nd event
                        cancellationTokenSource.Cancel();
                    });

                Assert.Equal("timer hit 2", contextForTest.ContextVariable);

                await Task.Delay(220).ContinueWith(x =>
                {
                    // Cancel after everything has finished
                    cancellationTokenSource2.Cancel();
                });

                Assert.Equal("timer hit 4", contextForTest2.ContextVariable);
            }
        }

        [Fact]
        public async Task timerStreamScheduler_WhenSchedulingRecurrentEventsAndCancellingSubscription_ThenExactNumberOfEventsAreRunning()
        {
            var contextForTest = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();
            using (var timerStreamScheduler = new TimerStreamScheduler())
            {
                timerStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 10, TimerScheduledCallType.Recurrent);

                await Task.Factory.StartNew(() =>
                    {
                        var infiniteStream =
                            contextForTest.Execute(timerStreamScheduler.SubscribeToInfiniteStream<ActionForTest>(cancellationTokenSource.Token))
                                .GetEnumerator();

                        Assert.True(infiniteStream.MoveNext());

                        Assert.True(infiniteStream.MoveNext());

                        cancellationTokenSource.Cancel();
                    });

                Assert.Equal("timer hit", contextForTest.ContextVariable);
            }
        }

        [Fact]
        public async Task timerStreamScheduler_WhenSchedulingRecurrentEventsAndCancellingEvent_ThenEventIsCancelled()
        {
            var contextForTest = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();
            using (var timerStreamScheduler = new TimerStreamScheduler())
            {
                timerStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 10, TimerScheduledCallType.Recurrent, cancellationTokenSource.Token);

                await Task.Factory.StartNew(() =>
                {
                    var infiniteStream =
                        contextForTest.Execute(timerStreamScheduler.SubscribeToInfiniteStream<ActionForTest>())
                            .GetEnumerator();

                    Assert.True(infiniteStream.MoveNext());

                    Assert.True(infiniteStream.MoveNext());

                    cancellationTokenSource.Cancel();
                });

                Assert.Equal("timer hit", contextForTest.ContextVariable);
            }
        }

        [Fact]
        public async Task timerStreamScheduler_WhenSchedulingDifferentEvents_ThenEventsAreFilteredCorrectly()
        {
            var contextForTest = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();
            using (var timerStreamScheduler = new TimerStreamScheduler())
            {
                timerStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 50, TimerScheduledCallType.Once, cancellationTokenSource.Token);
                timerStreamScheduler.ScheduleActionCall(new ActionForTestAsync("timer hit (async)"), 10, TimerScheduledCallType.Recurrent);

#pragma warning disable 4014
                Task.Factory.StartNew(() =>
#pragma warning restore 4014
                    {
                        var infiniteStream =
                            contextForTest.Execute(timerStreamScheduler.SubscribeToInfiniteStream<ActionForTest>())
                                .GetEnumerator();

                        Assert.True(infiniteStream.MoveNext());

                        // This should block - and get false
                        Assert.False(infiniteStream.MoveNext());
                    });

                await Task.Delay(100).ContinueWith(x =>
                    {
                        // Cancel after everything has finished
                        cancellationTokenSource.Cancel();
                    });

                Assert.Equal("timer hit", contextForTest.ContextVariable);
            }
        }
    }
}
