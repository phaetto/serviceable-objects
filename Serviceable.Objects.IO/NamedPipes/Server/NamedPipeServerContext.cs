﻿namespace Serviceable.Objects.IO.NamedPipes.Server
{
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using Composition.Graph.Events;
    using Composition.Graph.Stages.Initialization;
    using Configuration;
    using Exceptions;
    using Newtonsoft.Json;
    using Remote.Composition.Configuration;
    using Remote.Serialization;
    using Remote.Serialization.Streaming;

    public sealed class NamedPipeServerContext : ConfigurableContext<NamedPipeServerConfiguration, NamedPipeServerContext>, IInitializeStageFactoryWithDeinitSynchronization
    {
        private readonly StreamSession streamSession = new StreamSession();
        internal Task ServerTask;
        internal CancellationTokenSource CancellationTokenSource;

        public ReaderWriterLockSlim ReaderWriterLockSlim { get; set; }

        public NamedPipeServerContext()
        {
        }

        public NamedPipeServerContext(NamedPipeServerConfiguration configuration) : base(configuration)
        {
        }

        public object GenerateInitializationCommand()
        {
            return new StartServer();
        }

        public object GenerateDeinitializationCommand()
        {
            return new StopServer();
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
                                try
                                {
                                    ReaderWriterLockSlim?.EnterReadLock();

                                    if (CancellationTokenSource.IsCancellationRequested)
                                    {
                                        return;
                                    }

                                    var commandSpecification = JsonConvert.DeserializeObject<CommandSpecification>(commandString);
                                    var commandSpecificationService = new CommandSpecificationService();
                                    var command = commandSpecificationService.CreateCommandFromSpecification(commandSpecification);

                                    var eventResults =
                                        PublishContextEvent(
                                            new GraphFlowEventPushControlEventApplyCommandInsteadOfEvent(command));

                                    var commandResultSpecifications = commandSpecificationService.CreateSpecificationForEventResults(
                                        command.GetType(), eventResults);

                                    streamSession.Write(namedPipeServerStream, JsonConvert.SerializeObject(commandResultSpecifications));
                                }
                                finally 
                                {
                                    ReaderWriterLockSlim?.ExitReadLock();
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
