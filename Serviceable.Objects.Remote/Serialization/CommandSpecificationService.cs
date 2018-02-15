namespace Serviceable.Objects.Remote.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Dependencies;
    using Exceptions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Objects.Composition.Graph.Commands.NodeInstance.ExecutionData;
    using Objects.Exceptions;
    using Security;

    public sealed class CommandSpecificationService
    {
        public object CreateCommandFromSpecification(CommandSpecification commandSpecification)
        {
            Check.ArgumentNullOrWhiteSpace(commandSpecification.CommandType, nameof(commandSpecification.CommandType));

            var type = Types.FindType(commandSpecification.CommandType);

            object generatedObject;

            if (type.GetTypeInfo().ImplementedInterfaces.All(x => x != typeof(IReproducibleWithData)))
            {
                generatedObject = Types.CreateObjectWithParameters(type);
            }
            else
            {
                Check.ArgumentNull(commandSpecification.DataAsJson, nameof(commandSpecification.DataAsJson));

                var reproducibleWithWellknowData = type.GetTypeInfo().ImplementedInterfaces
                    .First(x => x.Name == typeof(IReproducibleWithKnownData<>).Name);
                var data = JsonConvert.DeserializeObject(commandSpecification.DataAsJson, reproducibleWithWellknowData.GenericTypeArguments[0]);
                generatedObject = Types.CreateObjectWithParameters(type, data);
            }

            if (generatedObject == null)
            {
                return null;
            }

            if (generatedObject is ISessionAuthorizableCommand sessionAuthorizableCommand)
            {
                sessionAuthorizableCommand.Session = commandSpecification.Session;
            }

            if (generatedObject is IApplicationAuthorizableCommand applicationAuthorizableCommand)
            {
                applicationAuthorizableCommand.ApiKey = commandSpecification.ApiKey;
            }

            return generatedObject;
        }

        public T CreateCommandFromSpecification<T>(CommandSpecification commandSpecification)
            where T : IReproducible
        {
            return (T)CreateCommandFromSpecification(commandSpecification);
        }

        public CommandSpecification CreateSpecificationForCommand(object command)
        {
            string data = null;
            if (command is IReproducibleWithData repoReproducibleWithData)
            {
                data = JsonConvert.SerializeObject(repoReproducibleWithData.DataAsObject);
            }

            var commandSpecification = new CommandSpecification
            {
                CommandType = command.GetType().AssemblyQualifiedName,
                DataAsJson = data
            };

            if (command is ISessionAuthorizableCommand sessionAuthorizableCommand)
            {
                commandSpecification.Session = sessionAuthorizableCommand.Session;
            }

            if (command is IApplicationAuthorizableCommand applicationAuthorizableCommand)
            {
                commandSpecification.ApiKey = applicationAuthorizableCommand.ApiKey;
            }

            return commandSpecification;
        }

        public CommandSpecification CreateSpecificationForCommand<T>(T command)
            where T : IReproducible
        {
            return CreateSpecificationForCommand((object)command);
        }

        public IEnumerable<CommandResultSpecification> CreateSpecificationForEventResults(Type remotableCommandType, IEnumerable<EventResult> eventResults)
        {
            Check.ArgumentNull(remotableCommandType, nameof(remotableCommandType));

            var eventResultsAsArray = eventResults.ToArray();
            if (eventResultsAsArray.Any())
            {
                var results = eventResultsAsArray.Select(x => x.ResultObject);
                return results
                    .Where(x => x != null)
                    .Select(x => CreateSpecificationForCommandResult(remotableCommandType, x));
            }

            if (remotableCommandType.GetTypeInfo().ImplementedInterfaces.Any(x => x.Name == typeof(IRemotable).Name))
            {
                // We have to send something back
                return new[]
                {
                    CreateSpecificationForCommandResultWithoutAValue(remotableCommandType)
                };
            }

            return new CommandResultSpecification[0];
        }

        public CommandResultSpecification CreateSpecificationForCommandResult(Type remotableCommandType, object result)
        {
            Check.ArgumentNull(remotableCommandType, nameof(remotableCommandType));
            Check.ArgumentNull(result, nameof(result));

            if (result is ExecutionCommandResult executionCommandResult)
            {
                return new CommandResultSpecification
                {
                    CommandType = remotableCommandType.AssemblyQualifiedName,
                    ResultDataObject = executionCommandResult.SingleContextExecutionResultWithInfo.ResultObject,
                    ContainsError = executionCommandResult.IsFaulted,

                    GraphResultSpecification = new GraphResultSpecification
                    {
                        IsFaulted = executionCommandResult.IsFaulted,
                        IsIdle = executionCommandResult.IsIdle,
                    },

                    Exception = executionCommandResult.Exception != null
                        ? new CommandSpecificationExceptionCarrier
                        {
                            Message = executionCommandResult.Exception.Message,
                            RealExceptionType = executionCommandResult.Exception.GetType().FullName,
                            StackTrace = executionCommandResult.Exception.StackTrace
                        }
                        : null,

                    PublishedEvents = executionCommandResult.PublishedEvents.Select(x => new EventResultSpecification
                    {
                        EventObject = x,
                        EventType = x.GetType().AssemblyQualifiedName,
                    }).ToList(),
                };
            }

            if (result is Exception exception)
            {
                return new CommandResultSpecification
                {
                    CommandType = remotableCommandType.AssemblyQualifiedName,
                    ContainsError = true,

                    Exception = new CommandSpecificationExceptionCarrier
                    {
                        Message = exception.Message,
                        RealExceptionType = exception.GetType().FullName,
                        StackTrace = exception.StackTrace
                    },
                };
            }

            return new CommandResultSpecification
            {
                CommandType = remotableCommandType.AssemblyQualifiedName,
                ResultDataObject = result,
                ContainsError = false,
            };
        }

        public CommandResultSpecification CreateSpecificationForCommandResultWithoutAValue(Type remotableCommandType)
        {
            Check.ArgumentNull(remotableCommandType, nameof(remotableCommandType));

            var dataAsJson = JsonConvert.SerializeObject(null);
            return new CommandResultSpecification
            {
                CommandType = remotableCommandType.AssemblyQualifiedName,
                ResultDataObject = dataAsJson,
                ContainsError = false,
            };
        }

        public object CreateResultDataFromCommandSpecification(CommandResultSpecification commandResultSpecification)
        {
            if (commandResultSpecification.ContainsError)
            {
                return new Exception($"{commandResultSpecification.Exception.Message}\n{commandResultSpecification.Exception.RealExceptionType}\n\n{commandResultSpecification.Exception.StackTrace}");
            }

            if (commandResultSpecification.ResultDataObject == null)
            {
                return null;
            }

            Check.ArgumentNullOrWhiteSpace(commandResultSpecification.CommandType, nameof(commandResultSpecification.CommandType));

            var type = Types.FindType(commandResultSpecification.CommandType);

            Check.ConditionNotSupported(
                type.GetTypeInfo().ImplementedInterfaces.All(x => x.Name == typeof(IRemotableCommand<,>).Name),
                $"The command should be derived from {typeof(IRemotableCommand<,>).FullName} for the data to be deserialized.");

            var remotableWithWellknowData = type.GetTypeInfo().ImplementedInterfaces
                .First(x => x.Name == typeof(IRemotableCommand<,>).Name);

            if (commandResultSpecification.ResultDataObject is JObject jObject)
            {
                return jObject.ToObject(remotableWithWellknowData.GenericTypeArguments[1]);
            }

            return commandResultSpecification.ResultDataObject;
        }

        public T CreateResultDataFromCommandSpecification<T>(CommandResultSpecification commandResultSpecification)
        {
            var result = CreateResultDataFromCommandSpecification(commandResultSpecification);
            return (T) result;
        }

        public object CreateEventFromEventResultSpecification(EventResultSpecification eventResultSpecification)
        {
            var type = Types.FindType(eventResultSpecification.EventType);

            Check.ConditionNotSupported(
                type.GetTypeInfo().ImplementedInterfaces.All(x => x.Name != typeof(IEvent).Name),
                $"The event should be derived from {typeof(IEvent).FullName} in order to be deserialized.");

            if (eventResultSpecification.EventObject is JObject jObject)
            {
                return jObject.ToObject(type);
            }

            return eventResultSpecification.EventObject;
        }

        public T CreateEventFromEventResultSpecification<T>(EventResultSpecification eventResultSpecification)
        {
            var result = CreateEventFromEventResultSpecification(eventResultSpecification);
            return (T) result;
        }
    }
}