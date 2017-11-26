namespace TestHttpCompositionConsoleApp.Contexts.ConfigurationSource
{
    using System;
    using Newtonsoft.Json;
    using Serviceable.Objects.Composition;
    using Serviceable.Objects.Composition.Graph;
    using Serviceable.Objects.Composition.Graph.Stages.Configuration;
    using Serviceable.Objects.IO.NamedPipes;
    using Serviceable.Objects.IO.NamedPipes.State;

    public sealed class MemoryConfigurationSource : IConfigurationSource
    {
        public string GetConfigurationValueForKey(ContextGraph contextGraph, ContextGraphNode contextGraphNode, Type type)
        {
            switch (type)
            {
                case var t when t == typeof(NamedPipeServerContext):
                    return JsonConvert.SerializeObject(new NamedPipeServerState
                    {
                        PipeName  = "testpipe"
                    });
                default:
                    throw new InvalidOperationException($"Type {type.FullName} is not supprted.");
            }
        }
    }
}
