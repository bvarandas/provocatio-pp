using System.Text.Json;
using System.Text.Json.Serialization;

namespace Challenge.Api.Converters
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            int days = 0, hours = 0, minutes = 0, seconds = 0, milliseconds = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new TimeSpan(days, hours, minutes, seconds, milliseconds);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName.ToLower())
                    {
                        case "days":
                            days = reader.GetInt32();
                            break;
                        case "hours":
                            hours = reader.GetInt32();
                            break;
                        case "minutes":
                            minutes = reader.GetInt32();
                            break;
                        case "seconds":
                            seconds = reader.GetInt32();
                            break;
                        case "milliseconds":
                            milliseconds = reader.GetInt32();
                            break;
                    }
                }
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("days", value.Days);
            writer.WriteNumber("hours", value.Hours);
            writer.WriteNumber("minutes", value.Minutes);
            writer.WriteNumber("seconds", value.Seconds);
            writer.WriteEndObject();
        }
    }
}
