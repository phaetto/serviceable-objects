namespace Serviceable.Objects.Dependencies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Serviceable.Objects.Exceptions;

    public sealed class Container : IDisposable
    {
        private readonly Dictionary<string, object> objectsCache = new Dictionary<string, object>();
        private bool isDisposed;

        public Container(Dictionary<string, object> customObjectsCache = null)
        {
            objectsCache = customObjectsCache ?? objectsCache;
        }

        public TOut Resolve<TOut>()
        {
            return (TOut) Resolve(typeof(TOut));
        }

        public object Resolve(Type typeRequested)
        {
            Check.ArgumentNull(typeRequested, nameof(typeRequested));

            if (objectsCache.ContainsKey(typeRequested.FullName))
            {
                return objectsCache[typeRequested.FullName];
            }

            return CreateObject(typeRequested, new Stack<Type>(10), true);
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

        private object CreateObject(Type type, Stack<Type> typeStackCall, bool cacheable)
        {
            Check.ArgumentNull(type, nameof(type));
            Check.ArgumentNull(typeStackCall, nameof(typeStackCall));

            CheckStack(type, typeStackCall);

            typeStackCall.Push(type);

            var constructors = type.GetTypeInfo().DeclaredConstructors.OrderByDescending(x => x.GetParameters().Length);
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
                        if (objectsCache.ContainsKey(parameterTypeFullName))
                        {
                            transformedObjects.Add(Convert.ChangeType(objectsCache[parameterTypeFullName], parameterInfo.ParameterType));
                        }
                        else
                        {
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
                    transformedObjects.Add(Convert.ChangeType(newObject, parameterInfo.ParameterType));
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
