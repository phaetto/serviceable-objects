namespace Serviceable.Objects.Streams.Timer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public sealed class TimerStreamScheduler : Publisher
    {
        private const int StartNeverTimerValue = Timeout.Infinite;

        private const int PeriodicCallbackDisabled = Timeout.Infinite;

        private const int TimerIdle = int.MinValue;

        private const int MinimumIntervalToStartApplyingActionInMilliseconds = 2;

        private readonly Timer timer;

        private readonly List<TimerScheduledCall> scheduledActions = new List<TimerScheduledCall>();

        private readonly object scheduledActionsSyncObject = new object();

        private int lastRunIntervalInMilliseconds = TimerIdle;

        private int timeInMillisecondsWhenStarted = Environment.TickCount;

        public bool IsIdle => lastRunIntervalInMilliseconds == TimerIdle;

        public TimerStreamScheduler()
        {
            this.timer = new Timer(
                TimerCallback,
                null,
                StartNeverTimerValue,
                PeriodicCallbackDisabled);
        }

        public void ScheduleActionCall(object action, int intervalInMilliseconds, TimerScheduledCallType timerScheduledCallType)
        {
            ScheduleActionCall(action, intervalInMilliseconds, timerScheduledCallType, CancellationToken.None);
        }

        public void ScheduleActionCall(object action, int intervalInMilliseconds, TimerScheduledCallType timerScheduledCallType, CancellationToken cancellationToken)
        {
            var elapsedMilliseconds = 0;

            if (lastRunIntervalInMilliseconds != TimerIdle)
            {
                elapsedMilliseconds = Environment.TickCount - timeInMillisecondsWhenStarted;
            }

            lock (scheduledActionsSyncObject)
            {
                foreach (var timerScheduledCall in scheduledActions)
                {
                    timerScheduledCall.NextScheduledTimeToRunInMilliseconds -= elapsedMilliseconds;

                    if (timerScheduledCall.NextScheduledTimeToRunInMilliseconds < 0)
                    {
                        timerScheduledCall.NextScheduledTimeToRunInMilliseconds = 0;
                    }
                }

                scheduledActions.Add(new TimerScheduledCall(intervalInMilliseconds)
                {
                    TimerScheduledCallType = timerScheduledCallType,
                    ActionToRepeat = action,
                    NextScheduledTimeToRunInMilliseconds = intervalInMilliseconds,
                    CancellationToken = cancellationToken,
                });
            }

            TimerStartLogic();
        }

        private int FindNextScheduleTime()
        {
            lock (scheduledActionsSyncObject)
            {
                var minimumScheduledTimeToRun = int.MaxValue;
                foreach (var timerScheduledCall in scheduledActions)
                {
                    if (minimumScheduledTimeToRun > timerScheduledCall.NextScheduledTimeToRunInMilliseconds
                        && !timerScheduledCall.CancellationToken.IsCancellationRequested)
                    {
                        minimumScheduledTimeToRun = timerScheduledCall.NextScheduledTimeToRunInMilliseconds;
                    }
                }

                if (minimumScheduledTimeToRun == int.MaxValue)
                {
                    return TimerIdle;
                }

                if (minimumScheduledTimeToRun < 0)
                {
                    return 0;
                }

                return minimumScheduledTimeToRun;
            }
        }

        private void TimerCallback(object state)
        {
            var schedulesActivated = new List<TimerScheduledCall>();
            var schedulesCancelled = new List<TimerScheduledCall>();

            lock (scheduledActionsSyncObject)
            {
                foreach (var timerScheduledCall in scheduledActions)
                {
                    timerScheduledCall.NextScheduledTimeToRunInMilliseconds -= lastRunIntervalInMilliseconds;

                    if (timerScheduledCall.CancellationToken.IsCancellationRequested)
                    {
                        schedulesCancelled.Add(timerScheduledCall);
                    }
                    else if (timerScheduledCall.NextScheduledTimeToRunInMilliseconds <=
                             MinimumIntervalToStartApplyingActionInMilliseconds)
                    {
                        schedulesActivated.Add(timerScheduledCall);
                    }
                }

                foreach (var timerScheduledCallCancelled in schedulesCancelled)
                {
                    scheduledActions.Remove(timerScheduledCallCancelled);
                }

                foreach (var timerScheduledCallActivated in schedulesActivated)
                {
                    if (timerScheduledCallActivated.CancellationToken.IsCancellationRequested)
                    {
                        continue;
                    }

                    Publish(timerScheduledCallActivated.ActionToRepeat);

                    if (timerScheduledCallActivated.TimerScheduledCallType == TimerScheduledCallType.Once)
                    {
                        scheduledActions.Remove(timerScheduledCallActivated);
                    }
                    else
                    {
                        timerScheduledCallActivated.NextScheduledTimeToRunInMilliseconds =
                            timerScheduledCallActivated.TimeToRunInMilliseconds;
                    }
                }
            }

            TimerStartLogic();
        }

        private void TimerStartLogic()
        {
            lastRunIntervalInMilliseconds = FindNextScheduleTime();

            if (lastRunIntervalInMilliseconds == 0)
            {
                TimerCallback(null);
                return;
            }

            if (lastRunIntervalInMilliseconds != TimerIdle)
            {
                timer.Change(lastRunIntervalInMilliseconds, PeriodicCallbackDisabled);
                timeInMillisecondsWhenStarted = Environment.TickCount;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            timer.Dispose();
        }
    }
}
