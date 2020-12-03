using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace J4JSoftware.Logging
{
    // converts between string values and LogFileLocation enum values. Any text other than 
    // "appdata" (case insensitive) translates to LogFileLocation.ExeFolder.
    public class LogFileLocationConverter : JsonConverter<LogFileLocation>
    {
        public override LogFileLocation Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            return reader.GetString()!.ToLowerInvariant() switch
            {
                "appdata" => LogFileLocation.AppData,
                _ => LogFileLocation.ExeFolder
            };
        }
        
        public override void Write( Utf8JsonWriter writer, LogFileLocation value, JsonSerializerOptions options )
        {
            writer.WriteStringValue( value.ToString() );
        }
    }
}
