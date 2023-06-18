using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphShield.Proxy.Exceptions.Models
{
    /// <summary>
    /// Custom JSON converter for ExceptionDetails.
    /// </summary>
    internal class ExceptionDetailsJsonConverter : JsonConverter<ExceptionDetails>
    {
        private static readonly JsonEncodedText Type = JsonEncodedText.Encode("type");
        private static readonly JsonEncodedText Title = JsonEncodedText.Encode("title");
        private static readonly JsonEncodedText Status = JsonEncodedText.Encode("status");
        private static readonly JsonEncodedText Detail = JsonEncodedText.Encode("detail");
        private static readonly JsonEncodedText Instance = JsonEncodedText.Encode("instance");

        /// <inheritdoc/>
        public override ExceptionDetails Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var exceptionDetails = new ExceptionDetails();
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Unexpected end when reading JSON.");

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                ReadValue(ref reader, exceptionDetails, options);

            if (reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException("Unexpected end when reading JSON.");

            return exceptionDetails;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ExceptionDetails value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            WriteExceptionDetails(writer, value, options);
            writer.WriteEndObject();
        }

        private static void ReadValue(ref Utf8JsonReader reader, ExceptionDetails value, JsonSerializerOptions options)
        {
            if (TryReadStringProperty(ref reader, Type, out var propertyValue))
                value.Type = propertyValue;
            else if (TryReadStringProperty(ref reader, Title, out propertyValue))
                value.Title = propertyValue;
            else if (TryReadStringProperty(ref reader, Detail, out propertyValue))
                value.Detail = propertyValue;
            else if (TryReadStringProperty(ref reader, Instance, out propertyValue))
                value.Instance = propertyValue;
            else if (reader.ValueTextEquals(Status.EncodedUtf8Bytes))
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.Null)
                    value.Status = reader.GetInt32();
            }
            else
            {
                var key = reader.GetString()!;
                reader.Read();
                value.Extensions[key] = JsonSerializer.Deserialize(ref reader, typeof(object), options);
            }
        }

        private static bool TryReadStringProperty(ref Utf8JsonReader reader, JsonEncodedText propertyName, [NotNullWhen(true)] out string? value)
        {
            if (!reader.ValueTextEquals(propertyName.EncodedUtf8Bytes))
            {
                value = default;
                return false;
            }

            reader.Read();
            value = reader.GetString()!;
            return true;
        }

        private static void WriteExceptionDetails(Utf8JsonWriter writer, ExceptionDetails value, JsonSerializerOptions options)
        {
            if (value.Type != null)
                writer.WriteString(Type, value.Type);
            if (value.Title != null)
                writer.WriteString(Title, value.Title);
            if (value.Status != null)
                writer.WriteNumber(Status, value.Status.Value);
            if (value.Detail != null)
                writer.WriteString(Detail, value.Detail);
            if (value.Instance != null)
                writer.WriteString(Instance, value.Instance);

            foreach (var kvp in value.Extensions)
            {
                writer.WritePropertyName(kvp.Key);
                JsonSerializer.Serialize(writer, kvp.Value, kvp.Value?.GetType() ?? typeof(object), options);
            }
        }
    }
}
