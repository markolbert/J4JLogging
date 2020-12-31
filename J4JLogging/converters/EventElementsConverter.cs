using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace J4JSoftware.Logging
{
    // converts between string values and EventElements enum values. Any recognized text
    // results results in an EventElements.Basic value.
    public class EventElementsConverter : JsonConverter<EventElements>
    {
        public override EventElements Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            // we return Basic if something goes wrong
            try
            {
                return Enum.Parse<EventElements>( reader.GetString()!, true );
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
