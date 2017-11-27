namespace Serviceable.Objects.Remote.Composition.Host.Commands
{
    using System;

    public sealed class RunAndBlock : ICommand<ApplicationHost, ApplicationHost>
    {
        public ApplicationHost Execute(ApplicationHost context)
        {
            context.CancellationTokenSource.Token.Register(CancellationRequested, context);
            context.EventWaitHandle.WaitOne();
            return context;
        }

        private static void CancellationRequested(object context)
        {
            (context as ApplicationHost).EventWaitHandle.Set();
        }
    }
}