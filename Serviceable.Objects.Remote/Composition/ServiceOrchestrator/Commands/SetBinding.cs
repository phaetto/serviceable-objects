﻿namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System.Linq;
    using Data;
    using Objects.Composition.ServiceOrchestrator;

    public sealed class SetBinding : ReproducibleCommandWithData<IServiceOrchestrator, IServiceOrchestrator, BindingData>
    {
        public SetBinding(BindingData data) : base(data)
        {
        }

        public override IServiceOrchestrator Execute(IServiceOrchestrator context)
        {
            context.InBindingsPerService[Data.ServiceName] = Data.InBindings?.ToList() ?? context.InBindingsPerService[Data.ServiceName];
            context.ExternalBindingsPerService[Data.ServiceName] = Data.ExternalBindings?.ToList() ?? context.ExternalBindingsPerService[Data.ServiceName];
            return context;
        }
    }
}