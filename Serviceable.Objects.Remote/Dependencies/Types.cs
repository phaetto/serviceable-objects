namespace Serviceable.Objects.Remote.Dependencies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exceptions;

    public static class Types
    {
        [Obsolete]
        public static object CreateObjectWithParameters(string unqualifiedTypeName, params object[] parameters)
        {
            return CreateObjectWithParametersAndInjection(FindType(unqualifiedTypeName), parameters);
        }

        public static object CreateObjectWithParameters(Type type, params object[] parameters)
        {
            return CreateObjectWithParametersAndInjection(type, parameters);
        }

        [Obsolete]
        public static object CreateObjectWithParametersAndInjection(string unqualifiedTypeName, object[] parameters, object[] injectedParameters = null)
        {
            return CreateObjectWithParametersAndInjection(FindType(unqualifiedTypeName), parameters, injectedParameters);
        }

        public static object CreateObjectWithParametersAndInjection(Type type, object[] parameters, object[] injectedParameters = null)
        {
            var constructors = type.GetTypeInfo().DeclaredConstructors.OrderByDescending(x => x.GetParameters().Length);
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
                                foreach (var injectedParameter in injectedParameters)
                                {
                                    if (injectedParameter != null
                                        && (constructorParameters[m].ParameterType == injectedParameter.GetType()
                                            || constructorParameters[m].ParameterType.GetTypeInfo().IsSubclassOf(
                                                injectedParameter.GetType())))
                                    {
                                        transformedObjects.Add(
                                            Convert.ChangeType(injectedParameter, constructorParameters[m].ParameterType));
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

        public static Type FindType(string fullyQualifiedName)
        {
            Check.ArgumentNullOrWhiteSpace(fullyQualifiedName, nameof(fullyQualifiedName));

            return Type.GetType(fullyQualifiedName, true);
        }
    }
}
