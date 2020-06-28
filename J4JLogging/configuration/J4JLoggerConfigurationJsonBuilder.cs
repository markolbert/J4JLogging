using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace J4JSoftware.Logging
{
    // Extends J4JLoggerConfigurationBuilder to work with JSON configuration information
    public class J4JLoggerConfigurationJsonBuilder : J4JLoggerConfigurationBuilder
    {
#pragma warning disable 8618
        private string _jsonText;
#pragma warning restore 8618

        // Stores the JSON configuration information in the specified file.
        // Note that no checking is done of the file contents to see if it is actually
        // valid JSON, or indeed if the file is even a text file.
        public J4JLoggerConfigurationJsonBuilder FromFile(string filePath)
        {
            if (File.Exists(filePath))
                _jsonText = File.ReadAllText(filePath);

            return this;
        }

        // Stores the provided JSON configuration information. Note that no checking is
        // done to see if it is actually valid JSON.
        public J4JLoggerConfigurationJsonBuilder FromJson(string jsonText)
        {
            if (!string.IsNullOrEmpty(jsonText))
                _jsonText = jsonText;

            return this;
        }

        // Expands upon the supplied JsonSerializerOptions, if any, to include ones needed for
        // the builder to function.
        public JsonSerializerOptions BuildSerializerSettings(JsonSerializerOptions? options = null)
        {
            if (ChannelTypes.Count == 0)
                throw new InvalidOperationException($"{nameof(J4JLoggerConfigurationJsonBuilder.Build)}: no channels are defined");

            var retVal = options ?? new JsonSerializerOptions();

            retVal.Converters.Add(new LogChannelListConverter(ChannelTypes));
            retVal.Converters.Add(new LogEventLevelConverter());
            retVal.Converters.Add( new EventElementsConverter() );

            // we need to grab any converters in the assemblies defining channels
            foreach (var kvp in ChannelTypes)
            {
                foreach (var converterType in kvp.Value.Assembly.GetTypes()
                    .Where(t => t.IsPublic
                                && !t.IsAbstract
                                && typeof(JsonConverter).IsAssignableFrom(t)
                                && t.GetConstructor(Type.EmptyTypes) != null))
                {
                    retVal.Converters.Add((JsonConverter)Activator.CreateInstance(converterType));
                }
            }

            return retVal;
        }

        // Creates an instance of TConfig based on the presumption the previously-supplied JSON data
        // can be parsed into an instance of it.
        public TConfig Build<TConfig>(JsonSerializerOptions? options = null)
            where TConfig : class
        {
            if (string.IsNullOrEmpty(_jsonText))
                throw new InvalidOperationException($"{nameof(J4JLoggerConfigurationJsonBuilder.Build)}: configuration file is undefined");

            try
            {
                return JsonSerializer.Deserialize<TConfig>(_jsonText, BuildSerializerSettings(options));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $"Couldn't parse JSON text to a {typeof(TConfig)} object.",
                    e);
            }
        }
    }
}