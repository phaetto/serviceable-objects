namespace Serviceable.Objects.Remote.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serviceable.Objects.Remote.Dependencies;

    [Serializable]
    public abstract class SerializableSpecification
    {
        private readonly TypeInfo dataSpecTypeInfo = typeof(DataSpecification).GetTypeInfo();

        private class FieldsAndPropertiesForClass
        {
            public IEnumerable<FieldInfo> fields;
            public IEnumerable<PropertyInfo> properties;
        }

        private static readonly Dictionary<string, FieldsAndPropertiesForClass> ReflectionCaching =
            new Dictionary<string, FieldsAndPropertiesForClass>();

        public abstract int DataStructureVersionNumber { get; }

        private void PopulateFieldsAndProperties()
        {
            var entry = new FieldsAndPropertiesForClass
                        {
                            properties =
                                GetType()
                                .GetProperties()
                                .Where(x => x.GetSetMethod(true) != null)
                                .Where(x => x.GetIndexParameters().Length == 0)
                                .ToList(),
                            fields =
                                GetType()
                                .GetFields()
                                .Where(x => x.IsPublic && !x.IsStatic)
                                .ToList()
                        };

            lock (ReflectionCaching)
            {
                ReflectionCaching[GetType().FullName] = entry;
            }
        }

        private IEnumerable<PropertyInfo> Properties
        {
            get
            {
                var thisFullTypeName = GetType().FullName;

                if (!ReflectionCaching.ContainsKey(thisFullTypeName))
                {
                    PopulateFieldsAndProperties();
                }

                return ReflectionCaching[thisFullTypeName].properties;
            }
        }

        private IEnumerable<FieldInfo> Fields
        {
            get
            {
                var thisFullTypeName = GetType().FullName;

                if (!ReflectionCaching.ContainsKey(thisFullTypeName))
                {
                    PopulateFieldsAndProperties();
                }

                return ReflectionCaching[thisFullTypeName].fields;
            }
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(
                this,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public string SerializeToJsonForCommandPrompt()
        {
            return SerializeToJson().Replace('\"', '\'');
        }

        public static string SerializeManyToJson(SerializableSpecification[] specs)
        {
            return JsonConvert.SerializeObject(
                specs,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public static string SerializeManyToJsonForCommandPrompt(SerializableSpecification[] specs)
        {
            return SerializeManyToJson(specs).Replace('\"', '\'');
        }

        public void LoadValues(IDictionary<string, object> values)
        {
            if (values.ContainsKey("DataStructureVersionNumber"))
            {
                var dataVersion = (int)Convert.ChangeType(values["DataStructureVersionNumber"], typeof(int));
                if (dataVersion != DataStructureVersionNumber)
                {
                    ResolveVersionConflict(values, dataVersion);
                }
            }

            foreach (var field in Fields)
            {
                if (!values.ContainsKey(field.Name))
                {
                    continue;
                }

                var data = values[field.Name];
                if (Equals(data, null))
                {
                    continue;
                }

                if (data.GetType() == typeof(JArray))
                {
                    var jarray = data as JArray;
                    data = NormalizeArrayTypes(field.FieldType, data, jarray);
                }

                if (data.GetType() == typeof(JObject) && dataSpecTypeInfo.IsAssignableFrom(GetType().GetTypeInfo()))
                {
                    var jcontainer = data as JObject;
                    var executableActionSpecification = this as ExecutableCommandSpecification;

                    var dataType = Types.FindType(executableActionSpecification.DataType);

                    if (dataType.GetTypeInfo().ImplementedInterfaces.Any(x => x == typeof(ISerializable)))
                    {
                        var serializationInfo = new SerializationInfo(dataType,
                            new JsonFormatterConverter(new JsonSerializer()));

                        foreach (var keyValuePair in jcontainer)
                        {
                            serializationInfo.AddValue(keyValuePair.Key, keyValuePair.Value);
                        }

                        data = Types.CreateObjectWithParametersAndInjection(dataType, new object[]
                        {
                            serializationInfo,
                            new StreamingContext(),
                        });
                    }
                    else
                    {
                        data = jcontainer.ToObject(dataType);
                    }
                }

                try
                {
                    field.SetValue(this, data);
                }
                catch (ArgumentException)
                {
                    if (field.FieldType == typeof(Guid))
                    {
                        field.SetValue(this, Guid.Parse(data as string));
                    }
                    else
                    {
                        field.SetValue(this, Convert.ChangeType(data, field.FieldType));
                    }
                }
            }

            foreach (var property in Properties)
            {
                if (!values.ContainsKey(property.Name))
                {
                    continue;
                }

                var data = values[property.Name];
                if (Equals(data, null))
                {
                    continue;
                }

                if (data.GetType() == typeof(JArray))
                {
                    var jarray = data as JArray;
                    data = NormalizeArrayTypes(property.PropertyType, data, jarray);
                }

                if (data.GetType() == typeof(JObject) && dataSpecTypeInfo.IsAssignableFrom(GetType().GetTypeInfo()))
                {
                    var jcontainer = data as JObject;
                    var executableActionSpecification = this as ExecutableCommandSpecification;
                    data = jcontainer.ToObject(Types.FindType(executableActionSpecification.DataType));
                }

                try
                {
                    property.SetValue(this, data, null);
                }
                catch (ArgumentException)
                {
                    if (property.PropertyType == typeof(Guid))
                    {
                        property.SetValue(this, Guid.Parse(data as string), null);
                    }
                    else
                    {
                        property.SetValue(this, Convert.ChangeType(data, property.PropertyType), null);
                    }
                }
            }
        }

        private static object NormalizeArrayTypes(Type type, object data, JArray jarray)
        {
            var elementType = type.GetElementType();

            if (elementType == null)
            {
                return jarray.ToObject(type);
            }

            switch (elementType.FullName)
            {
                case "System.String":
                    data = jarray.ToArray().Select(x => x.ToObject<string>()).ToArray();
                    break;
                case "System.Int32":
                    data = jarray.ToArray().Select(x => x.ToObject<int>()).ToArray();
                    break;
                case "System.Boolean":
                    data = jarray.ToArray().Select(x => x.ToObject<bool>()).ToArray();
                    break;
                case "System.Object":
                    data = jarray.ToArray().Select(x => x.ToObject(elementType)).ToArray();
                    break;
                default:
                    data = jarray.ToObject(type);
                    break;
            }

            return data;
        }

        protected virtual void ResolveVersionConflict(IDictionary<string, object> values, int dataVersion)
        {
        }
    }
}
