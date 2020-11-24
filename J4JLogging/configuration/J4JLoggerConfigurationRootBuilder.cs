using System;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    // Extends J4JLoggerConfigurationBuilder to support creating instances of IJ4JLoggerConfiguration types
    // from an IConfigurationRoot object
    public class J4JLoggerConfigurationRootBuilder : J4JLoggerConfigurationBuilder
    {
        // Creates an instance of TConfig using the information from a particular section of an 
        // IConfigurationRoot object. loggerSection specifies the key of the key/value pair containing
        // the configuration information to be used.
        public TConfig Build<TConfig>(IConfigurationRoot configRoot, string loggerSection = "Logger")
            where TConfig : class, IJ4JLoggerConfiguration
        {
            var ctor = typeof(TConfig).GetConstructor(new[] { typeof(IConfigurationRoot), typeof(string) });

            if (ctor == null)
                throw new ArgumentException(
                    $"{typeof(TConfig).Name} doesn't have a public constructor taking a {nameof(IConfigurationRoot)} and a {nameof(String)}");

            var retVal = (TConfig)ctor.Invoke(new object[] { configRoot, loggerSection });

            foreach (var kvp in ChannelTypes)
            {
                var text = configRoot.GetConfigValue(
                    $@"{loggerSection}:{nameof(IJ4JLoggerConfiguration.Channels)}:\d:{nameof(LogChannel.Channel)}", kvp.Key);

                if (!string.IsNullOrEmpty(text))
                {
                    var newChannel =
                        (LogChannel)Activator.CreateInstance(kvp.Value, new object[] { configRoot, loggerSection });

                    retVal.Channels.Add(newChannel);
                }
            }

            return retVal;
        }
    }
}