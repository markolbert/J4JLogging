using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // converts between string values and RollingInterval values. Any text other than day, hour, minute,
    // month or year (case insensitive) maps to RollingInterval.Infinite.
    public class RollingIntervalConverter : JsonConverter<RollingInterval>
    {
        public override RollingInterval Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            return reader.GetString().ToLowerInvariant() switch
            {
                "day" => RollingInterval.Day,
                "hour" => RollingInterval.Hour,
                "minute" => RollingInterval.Minute,
                "month" => RollingInterval.Month,
                "year" => RollingInterval.Year,
                _ => RollingInterval.Infinite
            };
        }
        
        public override void Write( Utf8JsonWriter writer, RollingInterval value, JsonSerializerOptions options )
        {
            writer.WriteStringValue( value.ToString() );
        }
    }
}
