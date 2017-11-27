namespace Serviceable.Objects.Dependencies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Serviceable.Objects.Exceptions;

    public sealed class Container : IDisposable
    {
        private readonly Container parentContainer;
        private readonly Dictionary<string, object> objectsCache = new Dictionary<string, object>();
        private bool isDisposed;

        public Container(Dictionary<string, object> customObjectsCache = null)
        {
            objectsCache = customObjectsCache ?? objectsCache;
            Register(this);
        }

        public Container(Container parentContainer)
        {
            this.parentContainer = parentContainer;
            Register(this);
        }

        public void Register(object implementation, bool replace = false)
        {
            Check.ArgumentNull(implementation, nameof(implementation));
            var type = implementation.GetType();
            
            Register(type, implementation, replace);
        }

        public void Register(Type type, object implementation, bool replace = false)
        {
            Check.ArgumentNull(type, nameof(type));
            Check.ArgumentNull(implementation, nameof(implementation));
            Check.Argument(objectsCache.ContainsKey(type.FullName) && !replace, nameof(type), "Type already exists in the container");

            if (objectsCache.ContainsKey(type.FullName))
            {
                objectsCache[type.FullName] = implementation;
            }
            else
            {
                objectsCache.Add(type.FullName, implementation);
            }
        }

        public void RegisterWithDefaultInterface(Type type)
        {
            Check.ArgumentNull(type, nameof(type));

            RegisterWithDefaultInterface(CreateObject(type));
        }

        public void RegisterWithDefaultInterface(object instance)
        {
            Check.ArgumentNull(instance, nameof(instance));

            var interfaces = instance.GetType().GetTypeInfo().ImplementedInterfaces.ToArray();

            Check.Argument(interfaces.Length != 1, nameof(instance), "Type should support only one interface.");

            objectsCache[interfaces[0].FullName] = instance;
        }

        public TOut Resolve<TOut>(bool throwOnError = true)
        {
            return (TOut) Resolve(typeof(TOut), throwOnError);
        }

        public object Resolve(Type typeRequested, bool throwOnError = true)
        {
            Check.ArgumentNull(typeRequested, nameof(typeRequested));

            return parentContainer?.ResolveFromCache(typeRequested)
                ?? ResolveFromCache(typeRequested)
                ?? CreateObject(typeRequested, new Stack<Type>(10), true, throwOnError);
        }

        public object CreateObject(Type type)
        {
            Check.ArgumentNull(type, nameof(type));

            return CreateObject(type, new Stack<Type>(10), false);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            foreach (var keyValue in objectsCache)
            {
                var disposable = keyValue.Value as IDisposable;
                disposable?.Dispose();
            }

            objectsCache.Clear();

            isDisposed = true;
        }

        private object ResolveFromCache(Type typeRequested)
        {
            Check.ArgumentNull(typeRequested, nameof(typeRequested));

            if (objectsCache.ContainsKey(typeRequested.FullName))
            {
                return objectsCache[typeRequested.FullName];
            }

            return parentContainer?.ResolveFromCache(typeRequested);
        }

        private object CreateObject(Type type, Stack<Type> typeStackCall, bool cacheable, bool throwOnError = true)
        {
            Check.ArgumentNull(type, nameof(type));
            Check.ArgumentNull(typeStackCall, nameof(typeStackCall));

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsInterface || typeInfo.IsAbstract)
            {
                if (typeStackCall.Count > 0 && parentContainer != null)
                {
                    var resolvedItem = parentContainer.ResolveFromCache(type);
                    if (resolvedItem != null)
                    {
                        return resolvedItem;
                    }
                }

                if (!throwOnError)
                {
                    return null;
                }

                Check.Argument(typeInfo.IsInterface || typeInfo.IsAbstract, nameof(type),
                    "Cannot instantiate an interface or abstract type.");
            }

            CheckStack(type, typeStackCall);

            typeStackCall.Push(type);

            var constructors = typeInfo.DeclaredConstructors.OrderByDescending(x => x.GetParameters().Length);
            foreach (var constructor in constructors)
            {
                try
                {
                    var constructorParameters = constructor.GetParameters();

                    // This is a candidate
                    var transformedObjects = new List<object>(constructorParameters.Length);
                    foreach (var parameterInfo in constructorParameters)
                    {
                        var parameterTypeFullName = parameterInfo.ParameterType.FullName;
                        var parameterTypeInfo = parameterInfo.ParameterType.GetTypeInfo();
                        if (objectsCache.ContainsKey(parameterTypeFullName))
                        {
                            var objectUnderInverstigation = objectsCache[parameterTypeFullName];
                            if (parameterTypeInfo.IsInterface && objectUnderInverstigation.GetType().GetTypeInfo()
                                    .ImplementedInterfaces.Any(x => x == parameterInfo.ParameterType))
                            {
                                transformedObjects.Add(objectUnderInverstigation);
                            }
                            else
                            {
                                transformedObjects.Add(Convert.ChangeType(objectUnderInverstigation, parameterInfo.ParameterType));
                            }
                        }
                        else
                        {
                            if (parameterTypeInfo.IsValueType)
                            {
                                throw new InvalidCastException("Value types are not allowed");
                            }

                            ResolveUnknownType(transformedObjects, parameterInfo, typeStackCall, cacheable);
                        }
                    }

                    var newObject = constructor.Invoke(transformedObjects.ToArray());

                    if (cacheable)
                    {
                        if (objectsCache.ContainsKey(type.FullName))
                        {
                            throw new TypeCreatedTwiceInConatinerException($"Type ${type.FullName} created twice - that should never have happened.");
                        }

                        objectsCache.Add(type.FullName, newObject);
                    }

                    return newObject;
                }
                catch (InvalidCastException)
                {
                    // Go to the next
                }
            }

            if (!throwOnError)
            {
                return null;
            }

            throw new Exception("No suitable constructor could be found to create the type: " + type.AssemblyQualifiedName);
        }

        private void ResolveUnknownType(List<object> transformedObjects, ParameterInfo parameterInfo, Stack<Type> typeStackCall, bool cacheable)
        {
            Check.ArgumentNull(transformedObjects, nameof(transformedObjects));
            Check.ArgumentNull(parameterInfo, nameof(parameterInfo));
            Check.ArgumentNull(typeStackCall, nameof(typeStackCall));

            var parameterTypeFullName = parameterInfo.ParameterType.FullName;
            var parameterType = parameterInfo.ParameterType.GetTypeInfo();

            // We have to check the full container for a supporting object
            var found = false;
            foreach (var keyValue in objectsCache)
            {
                if (parameterType.GetType() == keyValue.Value.GetType()
                    || parameterType.IsSubclassOf(keyValue.Value.GetType()))
                {
                    transformedObjects.Add(Convert.ChangeType(keyValue.Value, parameterInfo.ParameterType));
                    objectsCache.Add(parameterTypeFullName, keyValue.Value);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                if (!parameterInfo.IsOptional)
                {
                    CheckStack(parameterInfo.ParameterType, typeStackCall);

                    var newObject = CreateObject(parameterInfo.ParameterType, typeStackCall, cacheable);

                    if (parameterInfo.ParameterType.GetTypeInfo().IsInterface && newObject.GetType().GetTypeInfo()
                            .ImplementedInterfaces.Any(x => x == parameterInfo.ParameterType))
                    {
                        transformedObjects.Add(newObject);
                    }
                    else
                    {
                        transformedObjects.Add(Convert.ChangeType(newObject, parameterInfo.ParameterType));
                    }
                }
                else
                {
                    transformedObjects.Add(parameterInfo.DefaultValue);
                }
            }

            typeStackCall.Pop();
        }

        private static void CheckStack(Type type, Stack<Type> typeStackCall)
        {
            Check.ArgumentNull(type, nameof(type));
            Check.ArgumentNull(typeStackCall, nameof(typeStackCall));

            if (typeStackCall.Contains(type))
            {
                throw new CyclicDependencyException(
                    $"Error creating type {type.FullName}\n\nStack was:\n{string.Join("\n", typeStackCall.ToArray().Select(x => x.FullName))}");
            }
        }
    }
}
