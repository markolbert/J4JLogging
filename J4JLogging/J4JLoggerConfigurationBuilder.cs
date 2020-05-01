using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace J4JSoftware.Logging
{
    public class J4JLoggerConfigurationBuilder
    {
        private readonly Dictionary<string, Type> _channelTypes =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        private string _jsonText;

        public J4JLoggerConfigurationBuilder AddChannel<TChannel>()
            where TChannel : LogChannel
            => AddChannel(typeof(TChannel));

        public J4JLoggerConfigurationBuilder AddChannel(Type channelType)
        {
            if (channelType == null
                || !(typeof(IChannelConfiguration).IsAssignableFrom(channelType)))
                return this;

            if (channelType.GetConstructor(Type.EmptyTypes) == null)
                return this;

            // check that TChannel is decorated with the required attribute
            var attr = channelType.GetCustomAttributes(typeof(ChannelAttribute), false)
                .Cast<ChannelAttribute>()
                .FirstOrDefault();

            if (attr == null)
                return this;

            if (_channelTypes.ContainsKey(attr.ChannelID)) _channelTypes[attr.ChannelID] = channelType;
            else _channelTypes.Add(attr.ChannelID, channelType);

            return this;
        }

        public J4JLoggerConfigurationBuilder FromFile(string filePath)
        {
            if (File.Exists(filePath))
                _jsonText = File.ReadAllText(filePath);

            return this;
        }

        public J4JLoggerConfigurationBuilder FromJson(string jsonText)
        {
            if (!string.IsNullOrEmpty(jsonText))
                _jsonText = jsonText;

            return this;
        }

        public JsonSerializerOptions BuildSerializerSettings( JsonSerializerOptions options = null )
        {
            if (_channelTypes.Count == 0)
                throw new InvalidOperationException($"{nameof(J4JLoggerConfigurationBuilder.Build)}: no channels are defined");

            var retVal = options ?? new JsonSerializerOptions();

            retVal.Converters.Add( new LogChannelListConverter( _channelTypes ) );
            retVal.Converters.Add(new LogEventLevelConverter());

            // we need to grab any converters in the assemblies defining channels
            foreach( var kvp in _channelTypes )
            {
                foreach( var converterType in kvp.Value.Assembly.GetTypes()
                    .Where( t => t.IsPublic
                                 && !t.IsAbstract
                                 && typeof(JsonConverter).IsAssignableFrom(t)
                                 && t.GetConstructor( Type.EmptyTypes ) != null ) )
                {
                    retVal.Converters.Add( (JsonConverter) Activator.CreateInstance( converterType ) );
                }
            }

            return retVal;
        }

        public TConfig Build<TConfig>( JsonSerializerOptions options = null )
            where TConfig : class
        {
            if (string.IsNullOrEmpty(_jsonText))
                throw new InvalidOperationException($"{nameof(J4JLoggerConfigurationBuilder.Build)}: configuration file is undefined");

            try
            {
                return JsonSerializer.Deserialize<TConfig>(_jsonText, BuildSerializerSettings(options));
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