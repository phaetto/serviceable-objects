namespace Serviceable.Objects.Streams.Timer
{
    using System.Threading;

    internal sealed class TimerScheduledCall
    {
        public readonly int TimeToRunInMilliseconds;

        public TimerScheduledCall(int timeToRunInMilliseconds)
        {
            TimeToRunInMilliseconds = timeToRunInMilliseconds;
        }

        public TimerScheduledCallType TimerScheduledCallType { get; set; }

        public object ActionToRepeat { get; set; }

        public int NextScheduledTimeToRunInMilliseconds { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
