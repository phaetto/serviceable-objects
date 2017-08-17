namespace Serviceable.Objects.Remote.Serialization
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public static class DeserializableSpecification<T>
        where T : SerializableSpecification, new()
    {
        public static T DeserializeFromJson(string data)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            return MapObject(dict);
        }

        public static T[] DeserializeManyFromJson(string data)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(data);
            return dict.Select(MapObject).ToArray();
        }

        private static T MapObject(IDictionary<string, object> dict)
        {
            var model = new T();
            model.LoadValues(dict);
            return model;
        }
    }
}