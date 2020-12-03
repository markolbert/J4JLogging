using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // converts between string values and LogEventLevel enum values. Any recognized text
    // results results in a LogEventLevel.Verbose value.
    public class LogEventLevelConverter : JsonConverter<LogEventLevel>
    {
        public override LogEventLevel Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            return reader.GetString()!.ToLowerInvariant() switch
            {
                "debug" => LogEventLevel.Debug,
                "error" => LogEventLevel.Error,
                "fatal" => LogEventLevel.Fatal,
                "information" => LogEventLevel.Information,
                "warning" => LogEventLevel.Warning,
                _ => LogEventLevel.Verbose
            };
        }

        public override void Write( Utf8JsonWriter writer, LogEventLevel value, JsonSerializerOptions options )
        {
            writer.WriteStringValue( value.ToString() );
        }
    }
}
