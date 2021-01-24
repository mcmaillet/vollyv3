using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VollyV3.Controllers.Databind.Serialization
{
    public class LineBreakingDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("dd-MM-yyyy H:mm"));
        }
    }
}
