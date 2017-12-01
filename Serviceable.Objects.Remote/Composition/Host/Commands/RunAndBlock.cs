﻿namespace Serviceable.Objects.Remote.Composition.Host.Commands
{
    using System;

    public sealed class RunAndBlock : ICommand<ApplicationHost, ApplicationHost>
    {
        public ApplicationHost Execute(ApplicationHost context)
        {
            context.GraphContext.Configure();
            context.GraphContext.Setup();
            context.GraphContext.Initialize();

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