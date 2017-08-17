namespace Serviceable.Objects.Remote.Dependencies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.DotNet.PlatformAbstractions;
    using Microsoft.Extensions.DependencyModel;
    using Serviceable.Objects.Dependencies;
    using Serviceable.Objects.Exceptions;

    public static class Types
    {
        public static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        public static object CreateObjectWithParameters(string unqualifiedTypeName, params object[] parameters)
        {
            return CreateObjectWithParametersAndInjection(FindType(unqualifiedTypeName), parameters);
        }

        public static object CreateObjectWithParameters(Type type, params object[] parameters)
        {
            return CreateObjectWithParametersAndInjection(type, parameters);
        }

        public static object CreateObjectWithParametersAndInjection(string unqualifiedTypeName, object[] parameters, object[] injectedParameters = null)
        {
            return CreateObjectWithParametersAndInjection(FindType(unqualifiedTypeName), parameters, injectedParameters);
        }

        public static object CreateObjectWithParametersAndInjection(Type type, object[] parameters, object[] injectedParameters = null)
        {
            var constructors = type.GetTypeInfo().GetConstructors(Container.ConstructorBindingFlags).OrderByDescending(x => x.GetParameters().Length);
            foreach (var constructor in constructors)
            {
                try
                {
                    var constructorParameters = constructor.GetParameters();
                    var totalParametersCheck = parameters.Length
                                               + (injectedParameters?.Length ?? 0);

                    if (constructorParameters.Length <= totalParametersCheck)
                    {
                        // This is a candidate
                        var transformedObjects = new List<object>(totalParametersCheck);
                        for (var n = 0; n < parameters.Length; ++n)
                        {
                            transformedObjects.Add(Convert.ChangeType(parameters[n], constructorParameters[n].ParameterType));
                        }

                        if (injectedParameters != null)
                        {
                            for (var m = parameters.Length; m < constructorParameters.Length; ++m)
                            {
                                var found = false;
                                for (var n = 0; n < injectedParameters.Length; ++n)
                                {
                                    if (injectedParameters[n] != null
                                        && (constructorParameters[m].ParameterType.GetTypeInfo().IsInstanceOfType(injectedParameters[n])
                                            || constructorParameters[m].ParameterType.GetTypeInfo().IsSubclassOf(
                                                injectedParameters[n].GetType())))
                                    {
                                        transformedObjects.Add(
                                            Convert.ChangeType(injectedParameters[n], constructorParameters[m].ParameterType));
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    if (!constructorParameters[m].IsOptional)
                                    {
                                        throw new InvalidCastException();
                                    }

                                    transformedObjects.Add(constructorParameters[m].DefaultValue);
                                }
                            }
                        }

                        for (var n = totalParametersCheck; n < constructorParameters.Length; ++n)
                        {
                            if (!constructorParameters[n].IsOptional)
                            {
                                transformedObjects.Add(constructorParameters[n].DefaultValue);
                            }
                            else
                            {
                                transformedObjects.Add(null);
                            }
                        }

                        return constructor.Invoke(transformedObjects.ToArray());
                    }
                }
                catch (InvalidCastException)
                {
                    // Go to the next
                }
            }

            throw new Exception("No constructor could be found to create the unqualifiedTypeName: " + type.AssemblyQualifiedName);
        }

        public static Type FindType(string unqualifiedType)
        {
            Check.ArgumentNullOrWhiteSpace(unqualifiedType, nameof(unqualifiedType));

            if (typeCache.ContainsKey(unqualifiedType))
            {
                return typeCache[unqualifiedType];
            }

            var type = Type.GetType(unqualifiedType, false);
            if (type != null)
            {
                typeCache.Add(unqualifiedType, type);
                return type;
            }

            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var assemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId);

            foreach (var assemblyName in assemblyNames)
            {
                var fullyQualifiedName = $"{unqualifiedType},{assemblyName}";
                type = Type.GetType(fullyQualifiedName, false);
                if (type != null)
                {
                    typeCache.Add(unqualifiedType, type);
                    return type;
                }
            }

            throw new Exception("Type could not be found: " + unqualifiedType);
        }
    }
}
