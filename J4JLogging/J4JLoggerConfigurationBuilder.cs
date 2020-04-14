using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JsonSubTypes;
using Newtonsoft.Json;

namespace J4JSoftware.Logging
{
    public class J4JLoggerConfigurationBuilder
    {
        private readonly Dictionary<string, Type> _channels =
            new Dictionary<string, Type>( StringComparer.OrdinalIgnoreCase );

        private string _jsonText;

        public J4JLoggerConfigurationBuilder AddChannel<TChannel>()
            where TChannel : LogChannel 
            => AddChannel( typeof(TChannel) );

        public J4JLoggerConfigurationBuilder AddChannel(Type channelType)
        {
            if( channelType == null 
                || !( typeof(IChannelConfiguration).IsAssignableFrom( channelType ) ) )
                return this;

            // check that TChannel is decorated with the required attribute
            var attr = channelType.GetCustomAttributes(typeof(ChannelAttribute), false)
                .Cast<ChannelAttribute>()
                .FirstOrDefault();

            if( attr == null )
                return this;

            if( _channels.ContainsKey( attr.ChannelID ) ) _channels[ attr.ChannelID ] = channelType;
            else _channels.Add( attr.ChannelID, channelType );

            return this;
        }

        public J4JLoggerConfigurationBuilder FromFile( string filePath )
        {
            if( File.Exists( filePath ) )
                _jsonText = File.ReadAllText(filePath);

            return this;
        }

        public J4JLoggerConfigurationBuilder FromJson( string jsonText )
        {
            if( !string.IsNullOrEmpty( jsonText ) )
                _jsonText = jsonText;

            return this;
        }

        public JsonSerializerSettings BuildSerializerSettings()
        {
            if (_channels.Count == 0)
                throw new InvalidOperationException($"{nameof(J4JLoggerConfigurationBuilder.Build)}: no channels are defined");

            var retVal = new JsonSerializerSettings();

            var builder = JsonSubtypesConverterBuilder.Of(typeof(LogChannel), "Channel");

            foreach (var kvp in _channels)
            {
                builder.RegisterSubtype(kvp.Value, kvp.Key);
            }

            retVal.Converters.Add(builder.Build());

            return retVal;
        }

        public TConfig Build<TConfig>()
            where TConfig : class, IJ4JLoggerConfiguration
        {
            var settings = BuildSerializerSettings();

            if( string.IsNullOrEmpty(_jsonText))
                throw new InvalidOperationException($"{nameof(J4JLoggerConfigurationBuilder.Build)}: configuration file is undefined");

            try
            {
                return JsonConvert.DeserializeObject<TConfig>(_jsonText, settings);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $"Couldn't parse JSON text to a {typeof(TConfig)} object. The likely cause is using an incorrect or invalid Channel property in the JSON file.",
                    e);
            }
        }
    }
}