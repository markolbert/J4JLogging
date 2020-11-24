using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace J4JSoftware.Logging
{
    // converts between structured string values and ILogChannel values. Any recognized text
    // results results in an EventElements.All value.
    public class LogChannelListConverter : JsonConverter<List<IChannelConfig>>
    {
        private readonly Dictionary<string, Type> _channelTypes;

        internal LogChannelListConverter( Dictionary<string, Type> channelTypes )
        {
            _channelTypes = channelTypes ?? throw new NullReferenceException( nameof(channelTypes) );
        }

        public override List<IChannelConfig> Read( ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options )
        {
            if( reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            var retVal = new List<IChannelConfig>();

            using JsonDocument doc = JsonDocument.ParseValue( ref reader );
            {
                if( doc.RootElement.ValueKind != JsonValueKind.Array )
                    throw new JsonException();
            }

            // iterate over all the LogChannel entries
            foreach( var jsonChannel in doc.RootElement.EnumerateArray() )
            {
                if( jsonChannel.ValueKind != JsonValueKind.Object )
                    throw new JsonException();

                // determine type of LogChannel being created
                var channelProp = jsonChannel.EnumerateObject()
                    .FirstOrDefault( j => j.Name.Equals( "Channel", StringComparison.OrdinalIgnoreCase ) );

                if( channelProp.Value.ValueKind != JsonValueKind.String )
                    throw new JsonException();

                var channelType = channelProp.Value.GetString();
                if( !_channelTypes.ContainsKey( channelType ) )
                    continue;

                var newChannel =
                    (LogChannel) JsonSerializer.Deserialize( jsonChannel.ToString(), _channelTypes[ channelType ], options );

                retVal.Add( newChannel );
            }

            return retVal;
        }

        public override void Write( Utf8JsonWriter writer, List<IChannelConfig> value, JsonSerializerOptions options )
        {
            writer.WriteStartArray();

            foreach( var channel in value )
            {
                JsonSerializer.Serialize( writer, channel, channel.GetType(), options );
            }

            writer.WriteEndArray();
        }
    }
}
