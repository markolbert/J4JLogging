using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class EventElementsConverter : JsonConverter<EventElements>
    {
        public override EventElements Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            // we return All if something goes wrong
            try
            {
                return Enum.Parse<EventElements>( reader.GetString(), true );
            }
            catch
            {
                return EventElements.All;
            }
        }

        public override void Write( Utf8JsonWriter writer, EventElements value, JsonSerializerOptions options )
        {
            writer.WriteStringValue( value.ToString() );
        }
    }
}
