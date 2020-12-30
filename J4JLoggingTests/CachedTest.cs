using System;
using System.IO;
using FluentAssertions;
using J4JSoftware.Logging;
using Xunit;

namespace J4JLoggingTests
{
    public class CachedTest
    {
        [Theory]
        [InlineData("config-files/Simple.json", "Logger", "Channels")]
        [InlineData("config-files/Derived.json", "Logger", "Channels")]
        [InlineData("config-files/Embedded.json", "Container:Logger", "Container:Channels")]
        public void Simple( string configPath, string loggerKey, string channelsKey )
        {
            var cached = new J4JCachedLogger();
            cached.SetLoggedType(this.GetType());

            var template = "{0} ({1})";

            cached.Verbose<string, string>(template, "Verbose", configPath);
            cached.Warning<string, string>(template, "Warning", configPath);
            cached.Information<string, string>(template, "Information", configPath);
            cached.Debug<string, string>(template, "Debug", configPath);
            cached.Error<string, string>(template, "Error", configPath);
            cached.Fatal<string, string>(template, "Fatal", configPath);
            cached.IncludeSms().Verbose<string, string>( "{0} ({1})", "Verbose", configPath );

            var compRoot = new CompositionRoot(configPath, loggerKey, channelsKey);
            var logger = compRoot.J4JLogger;
            var lastEvent = compRoot.LastEventConfig;

            foreach( var entry in cached.Cache )
            {
                if( entry.IncludeSms )
                    logger.IncludeSms();

                logger.Write( entry.LogEventLevel, entry.Template, entry.PropertyValues, entry.MemberName,
                    entry.SourcePath, entry.SourceLine );

                lastEvent.LastLogMessage.Should().Be( format_message( entry.LogEventLevel.ToString() ) );
            }

            string format_message( string prop1 ) =>
                template.Replace( "{0}", $"\"{prop1}\"" ).Replace( "{1}", $"\"{configPath}\"" );
        }
    }
}
