﻿namespace Serviceable.Objects.IO.NamedPipes.Server
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Threading.Tasks;
    using Commands;
    using Composition.Graph.Events;
    using Composition.Graph.Stages.Configuration;
    using Composition.Graph.Stages.Initialization;
    using Configuration;
    using Exceptions;
    using Newtonsoft.Json;
    using Remote;
    using Remote.Composition.Configuration;
    using Remote.Serialization;
    using Remote.Serialization.Streaming;

    public sealed class NamedPipeServerContext : ConfigurableContext<NamedPipeServerConfiguration, NamedPipeServerContext>, IInitializeStageFactory
    {
        private readonly StreamSession streamSession = new StreamSession();
        internal Task ServerTask;

        public NamedPipeServerContext()
        {
        }

        public NamedPipeServerContext(NamedPipeServerConfiguration configuration) : base(configuration)
        {
        }

        public NamedPipeServerContext(IConfigurationSource configurationSource) : base(configurationSource)
        {
        }

        public dynamic GenerateInitializeCommand()
        {
            return new StartServer();
        }

        internal void RunServerAndBlock()
        {
            Check.ArgumentNull(Configuration, nameof(Configuration));

            using (var namedPipeServerStream = new NamedPipeServerStream(Configuration.PipeName, PipeDirection.InOut))
            {
                while (true)
                {
                    namedPipeServerStream.WaitForConnection();

                    try
                    {
                        while (true)
                        {
                            // Let the stream protocol get the pieces
                            streamSession.Read(namedPipeServerStream);

                            if (streamSession.CommandsTextReadyToBeParsedQueue.TryDequeue(out var commandString))
                            {
                                var commandSpecification = JsonConvert.DeserializeObject<CommandSpecification>(commandString);
                                var commandSpecificationService = new CommandSpecificationService();
                                var command = commandSpecificationService.CreateCommandFromSpecification(commandSpecification);

                                var eventResults =
                                    PublishCommandEventAndGetResults(new GraphFlowEventPushControlApplyCommandInsteadOfEvent(command))
                                        .Where(x => x.ResultObject != null).ToList();

                                if (eventResults.Count > 0)
                                {
                                    var results = eventResults.Select(x => x.ResultObject);
                                    var commandResultSpecifications = results
                                        .Where(x => x != null)
                                        .Select(x => commandSpecificationService.CreateSpecificationForCommandResult(command.GetType(), x));

                                    streamSession.Write(namedPipeServerStream, JsonConvert.SerializeObject(commandResultSpecifications.ToArray()));
                                }
                                else if (command is IRemotable)
                                {
                                    // We have to send something back
                                    var commandResultSpecification = new[]
                                    {
                                        commandSpecificationService.CreateSpecificationForCommandResultWithoutAValue(command.GetType())
                                    };
                                    streamSession.Write(namedPipeServerStream, JsonConvert.SerializeObject(commandResultSpecification));
                                }
                            }

                            namedPipeServerStream.WaitForPipeDrain();
                        }
                    }
                    catch (IOException)
                    {
                        namedPipeServerStream.Disconnect();
                        // Reconnect
                    }
                }
            }
        }
    }
}