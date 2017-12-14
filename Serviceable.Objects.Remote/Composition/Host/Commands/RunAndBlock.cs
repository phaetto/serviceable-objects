namespace Serviceable.Objects.Remote.Composition.Host.Commands
{
    public sealed class RunAndBlock : ICommand<ApplicationHost, ApplicationHost>
    {
        public ApplicationHost Execute(ApplicationHost context)
        {
            context.GraphContext.ConfigureSetupAndInitialize();
            context.CancellationTokenSource.Token.Register(CancellationRequested, context);
            context.EventWaitHandle.WaitOne();
            return context;
        }

        private static void CancellationRequested(object context)
        {
            ((ApplicationHost) context).EventWaitHandle.Set();
        }
    }
}