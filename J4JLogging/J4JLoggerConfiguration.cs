using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JsonSubTypes;
using Newtonsoft.Json;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JLoggerConfiguration : IJ4JLoggerConfiguration
    {
        public static JsonSerializerSettings GetSerializerSettings( Dictionary<LogChannel, Type> channelTypes )
        {
            var retVal = new JsonSerializerSettings();

            // configure converterbuilder
            var builder = JsonSubtypesConverterBuilder.Of(typeof(LogChannelConfiguration), "Channel");

            foreach (var kvp in channelTypes)
            {
                builder.RegisterSubtype(kvp.Value, kvp.Key);
            }

            retVal.Converters.Add(builder.SerializeDiscriminatorProperty().Build());

            return retVal;
        }

        public static IJ4JLoggerConfiguration CreateFromFile( string configFilePath, Dictionary<LogChannel, Type> channelTypes )
        {
            if( !File.Exists( configFilePath ) )
                throw new IOException(
                    $"Couldn't find {nameof(J4JLoggerConfiguration)} configuration file '{configFilePath}'" );

            return Create( File.ReadAllText( configFilePath ), channelTypes );
        }

        public static IJ4JLoggerConfiguration Create( string text, Dictionary<LogChannel, Type> channelTypes )
        {
            var settings = GetSerializerSettings( channelTypes );

            try
            {
                return JsonConvert.DeserializeObject<J4JLoggerConfiguration>( text, settings );
            }
            catch( Exception e )
            {
                throw new InvalidOperationException(
                    $"Couldn't parse JSON text to a {nameof(J4JLoggerConfiguration)} object", e );
            }
        }

        public string SourceMessageTemplate { get; set; } = "({File}:{Line})";
        public string MemberMessageTemplate { get; set; } = "{SourceContext}::{Member}";
        public string SourceRootPath { get; set; }

        public EntryElements DefaultElements
        {
            get
            {
                var retVal = EntryElements.None;

                if( IncludeAssemblyName ) retVal |= EntryElements.Assembly;
                if( IncludeSource ) retVal |= EntryElements.SourceCode;

                return retVal;
            }
        }

        public bool IncludeSource { get; set; }
        public bool IncludeAssemblyName { get; set; }

        public List<LogChannelConfiguration> Channels { get; set; }

        public LogChannel ChannelsDefined
        {
            get
            {
                LogChannel retVal = LogChannel.None;

                if( Channels == null || Channels.Count == 0 )
                    return retVal;

                return Channels.Aggregate( retVal, ( current, channel ) => current | channel.GetChannelType() );
            }
        }

        public bool IsChannelDefined( LogChannel channel ) => ( ChannelsDefined & channel ) == channel;

        public LogEventLevel MinimumLogLevel
        {
            get
            {
                if( Channels == null || Channels.Count == 0 )
                    return LogEventLevel.Verbose;

                return Channels.Min( c => c.MinimumLevel );
            }
        }
    }
}