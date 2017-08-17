namespace Serviceable.Objects.Remote.Serialization
{
    using Newtonsoft.Json;

    public static class Json<T>
    {
        public static T Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string Serialize(T data)
        {
            return JsonConvert.SerializeObject(
                data,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        public static string SerializeWithPadding(T data)
        {
            return JsonConvert.SerializeObject(
                data,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });
        }
    }
}