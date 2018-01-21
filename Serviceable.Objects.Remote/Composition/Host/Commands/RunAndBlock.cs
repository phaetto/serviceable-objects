namespace Serviceable.Objects.Remote.Composition.Host.Commands
{
    using System;

    public sealed class RunAndBlock : ICommand<ApplicationHost, ApplicationHost>
    {
        public ApplicationHost Execute(ApplicationHost context)
        {
            try
            {
                context.GraphContext.ConfigureSetupAndInitialize();
                context.CancellationTokenSource.Token.Register(CancellationRequested, context);
                context.EventWaitHandle.WaitOne();
                return context;
            }
            finally
            {
                context.GraphContext.UninitializeDismantleAndDeconfigure();
            }
        }

        private static void CancellationRequested(object context)
        {
            ((ApplicationHost) context).EventWaitHandle.Set();
        }
    }
}