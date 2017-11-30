namespace Serviceable.Objects.Remote.Composition.Host
{
    using System.Threading;

    public sealed class ApplicationHost : Context<ApplicationHost>
    {
        public EventWaitHandle EventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
    }
}