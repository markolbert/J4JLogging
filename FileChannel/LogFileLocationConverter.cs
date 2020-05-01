using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class LogFileLocationConverter : JsonConverter<LogFileLocation>
    {
        public override LogFileLocation Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            return reader.GetString().ToLowerInvariant() switch
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
