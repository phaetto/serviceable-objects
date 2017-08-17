namespace Serviceable.Objects.Remote.Serialization
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serviceable.Objects.Exceptions;

    internal class JsonFormatterConverter : IFormatterConverter
    {
        private readonly JsonSerializer serializer;

        public JsonFormatterConverter(JsonSerializer serializer)
        {
            Check.ArgumentNull(serializer, nameof(serializer));

            this.serializer = serializer;
        }

        private T GetTokenValue<T>(object value)
        {
            Check.ArgumentNull(value, nameof(value));

            if (typeof (T) == value.GetType())
            {
                return (T)value;
            }

            JValue v = (JValue)value;
            return (T)System.Convert.ChangeType(v.Value, typeof(T), CultureInfo.InvariantCulture);
        }

        public object Convert(object value, Type type)
        {
            Check.ArgumentNull(value, nameof(value));

            JToken token = value as JToken;
            Check.Argument(token == null, "Value is not a JToken.", nameof(value));

            return serializer.Deserialize(token.CreateReader(), type);
        }

        public object Convert(object value, TypeCode typeCode)
        {
            Check.ArgumentNull(value, nameof(value));

            if (value is JValue)
                value = ((JValue)value).Value;

            return System.Convert.ChangeType(value, typeCode, CultureInfo.InvariantCulture);
        }

        public bool ToBoolean(object value)
        {
            return GetTokenValue<bool>(value);
        }

        public byte ToByte(object value)
        {
            return GetTokenValue<byte>(value);
        }

        public char ToChar(object value)
        {
            return GetTokenValue<char>(value);
        }

        public DateTime ToDateTime(object value)
        {
            return GetTokenValue<DateTime>(value);
        }

        public decimal ToDecimal(object value)
        {
            return GetTokenValue<decimal>(value);
        }

        public double ToDouble(object value)
        {
            return GetTokenValue<double>(value);
        }

        public short ToInt16(object value)
        {
            return GetTokenValue<short>(value);
        }

        public int ToInt32(object value)
        {
            return GetTokenValue<int>(value);
        }

        public long ToInt64(object value)
        {
            return GetTokenValue<long>(value);
        }

        public sbyte ToSByte(object value)
        {
            return GetTokenValue<sbyte>(value);
        }

        public float ToSingle(object value)
        {
            return GetTokenValue<float>(value);
        }

        public string ToString(object value)
        {
            return GetTokenValue<string>(value);
        }

        public ushort ToUInt16(object value)
        {
            return GetTokenValue<ushort>(value);
        }

        public uint ToUInt32(object value)
        {
            return GetTokenValue<uint>(value);
        }

        public ulong ToUInt64(object value)
        {
            return GetTokenValue<ulong>(value);
        }
    }
}