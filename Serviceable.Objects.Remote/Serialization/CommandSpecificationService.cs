namespace Serviceable.Objects.Remote.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Dependencies;
    using Exceptions;
    using Newtonsoft.Json;
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
                var subdataAsJson = JsonConvert.SerializeObject(new CommandExecutionResultSpecification
                {
                    IsPaused = executionCommandResult.IsPaused,
                    CommandSpecificationExceptionCarrier = executionCommandResult.Exception != null
                        ? new CommandSpecificationExceptionCarrier
                            {
                                Message = executionCommandResult.Exception.Message,
                                RealExceptionType = executionCommandResult.Exception.GetType().FullName,
                                StackTrace = executionCommandResult.Exception.StackTrace
                            }
                        : null,
                    IsFaulted = executionCommandResult.IsFaulted,
                    IsIdle = executionCommandResult.IsIdle,
                    ResultDataAsJson = executionCommandResult.SingleContextExecutionResultWithInfo.ResultObject != null 
                        ? JsonConvert.SerializeObject(executionCommandResult.SingleContextExecutionResultWithInfo.ResultObject)
                        : null,
                    CommandType = remotableCommandType.AssemblyQualifiedName,
                    PublishedEvents = executionCommandResult.PublishedEvents,
                });

                return new CommandResultSpecification
                {
                    CommandType = remotableCommandType.AssemblyQualifiedName,
                    ResultDataAsJson = subdataAsJson,
                    ContainsError = false,
                    ContainsSubdata = true,
                };
            }

            if (result is Exception exception)
            {
                var errorAsJson = JsonConvert.SerializeObject(new CommandSpecificationExceptionCarrier
                {
                    Message = exception.Message,
                    RealExceptionType = exception.GetType().FullName,
                    StackTrace = exception.StackTrace
                });

                return new CommandResultSpecification
                {
                    CommandType = remotableCommandType.AssemblyQualifiedName,
                    ResultDataAsJson = errorAsJson,
                    ContainsError = true,
                    ContainsSubdata = false,
                };
            }

            var dataAsJson = JsonConvert.SerializeObject(result);
            return new CommandResultSpecification
            {
                CommandType = remotableCommandType.AssemblyQualifiedName,
                ResultDataAsJson = dataAsJson,
                ContainsError = false,
                ContainsSubdata = false,
            };
        }

        public CommandResultSpecification CreateSpecificationForCommandResultWithoutAValue(Type remotableCommandType)
        {
            Check.ArgumentNull(remotableCommandType, nameof(remotableCommandType));

            var dataAsJson = JsonConvert.SerializeObject(null);
            return new CommandResultSpecification
            {
                CommandType = remotableCommandType.AssemblyQualifiedName,
                ResultDataAsJson = dataAsJson,
                ContainsError = false,
                ContainsSubdata = false,
            };
        }

        public object CreateResultDataFromCommandSpecification(CommandResultSpecification commandResultSpecification)
        {
            Check.ArgumentNullOrWhiteSpace(commandResultSpecification.ResultDataAsJson, nameof(commandResultSpecification.ResultDataAsJson));

            if (commandResultSpecification.ContainsError)
            {
                var commandSpecificationExceptionCarrier = JsonConvert.DeserializeObject<CommandSpecificationExceptionCarrier>(commandResultSpecification
                    .ResultDataAsJson);

                return new Exception($"{commandSpecificationExceptionCarrier.Message}\n{commandSpecificationExceptionCarrier.RealExceptionType}\n\n{commandSpecificationExceptionCarrier.StackTrace}");
            }

            if (commandResultSpecification.ContainsSubdata)
            {
                return JsonConvert.DeserializeObject<CommandExecutionResultSpecification>(commandResultSpecification.ResultDataAsJson);
            }

            Check.ArgumentNullOrWhiteSpace(commandResultSpecification.CommandType, nameof(commandResultSpecification.CommandType));

            var type = Types.FindType(commandResultSpecification.CommandType);

            Check.ConditionNotSupported(
                type.GetTypeInfo().ImplementedInterfaces.All(x => x.Name == typeof(IRemotableCommand<,>).Name),
                $"The command should be derived from {typeof(IRemotableCommand<,>).FullName} for the data to be deserialized.");

            var remotableWithWellknowData = type.GetTypeInfo().ImplementedInterfaces
                .First(x => x.Name == typeof(IRemotableCommand<,>).Name);
            return JsonConvert.DeserializeObject(commandResultSpecification.ResultDataAsJson,
                remotableWithWellknowData.GenericTypeArguments[1]);
        }

        public T CreateResultDataFromCommandSpecification<T>(CommandResultSpecification commandResultSpecification)
        {
            var result = CreateResultDataFromCommandSpecification(commandResultSpecification);
            return result != null ? (T) result : default(T);
        }

        public object CreateResultDataFromCommandSpecification(CommandExecutionResultSpecification commandExecutionResultSpecification)
        {
            if (commandExecutionResultSpecification.IsFaulted || commandExecutionResultSpecification.IsPaused)
            {
                var commandSpecificationExceptionCarrier = commandExecutionResultSpecification.CommandSpecificationExceptionCarrier;
                return new Exception($"{commandSpecificationExceptionCarrier.Message}\n{commandSpecificationExceptionCarrier.RealExceptionType}\n\n{commandSpecificationExceptionCarrier.StackTrace}");
            }

            if (commandExecutionResultSpecification.ResultDataAsJson == null)
            {
                return null;
            }

            return CreateResultDataFromCommandSpecification(new CommandResultSpecification
            {
                ContainsSubdata = false,
                ContainsError = false,
                ResultDataAsJson = commandExecutionResultSpecification.ResultDataAsJson,
                CommandType = commandExecutionResultSpecification.CommandType,
            });
        }

        public T CreateResultDataFromCommandSpecification<T>(CommandExecutionResultSpecification commandExecutionResultSpecification)
        {
            var result = CreateResultDataFromCommandSpecification(commandExecutionResultSpecification);
            return result != null ? (T) result : default(T);
        }
    }
}