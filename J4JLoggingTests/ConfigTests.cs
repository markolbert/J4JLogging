using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Xunit;

namespace J4JLoggingTests
{
    public class ConfigTests
    {
        [Theory ]
        [MemberData(nameof(SimpleData.FilePaths), MemberType = typeof(SimpleData))]
        public void Simple( string filePath )
        {
            var loggerInfo = SimpleData.GetLoggerInfo( filePath );
            loggerInfo.Should().NotBeNull();

            var configRoot = new ConfigurationBuilder()
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, filePath ) )
                .Build();

            var settings = new LoggerInfo( configRoot, string.Empty );

            settings.Global.Should().NotBeNull();

            ChannelInfoMatches(settings.Global!, loggerInfo!.Global!);

            if ( loggerInfo!.ChannelSpecific!.Count == 0 )
                settings.ChannelSpecific.Should().BeNull();
            else
            {
                settings.ChannelSpecific.Should().NotBeNull();

                foreach( var kvp in settings.ChannelSpecific! )
                {
                    var original = loggerInfo.ChannelSpecific![ kvp.Key ];

                    ChannelInfoMatches( kvp.Value!, original );
                }
            }
        }

        [Theory]
        [MemberData(nameof(EmbeddedData.FilePaths), MemberType = typeof(EmbeddedData))]
        public void Embedded(string filePath)
        {
            var loggerInfo = EmbeddedData.GetLoggerInfo(filePath);
            loggerInfo.Should().NotBeNull();

            var configRoot = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, filePath))
                .Build();

            var settings = new LoggerInfo( configRoot.GetSection( nameof(EmbeddedData.Embedded.LoggerInfo) ), string.Empty );

            settings.Global.Should().NotBeNull();

            ChannelInfoMatches(settings.Global!, loggerInfo!.Global!);

            if (loggerInfo!.ChannelSpecific!.Count == 0)
                settings.ChannelSpecific.Should().BeNull();
            else
            {
                settings.ChannelSpecific.Should().NotBeNull();

                foreach (var kvp in settings.ChannelSpecific!)
                {
                    var original = loggerInfo.ChannelSpecific![kvp.Key];

                    ChannelInfoMatches(kvp.Value!, original);
                }
            }
        }

        [Theory]
        [MemberData(nameof(DerivedData.FilePaths), MemberType = typeof(DerivedData))]
        public void Derived(string filePath)
        {
            var loggerInfo = DerivedData.GetLoggerInfo(filePath);
            loggerInfo.Should().NotBeNull();

            var configRoot = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, filePath))
                .Build();

            var settings = new LoggerInfo( configRoot, string.Empty );

            settings.Global.Should().NotBeNull();

            ChannelInfoMatches(settings.Global!, loggerInfo!.Global!);

            if (loggerInfo!.ChannelSpecific!.Count == 0)
                settings.ChannelSpecific.Should().BeNull();
            else
            {
                settings.ChannelSpecific.Should().NotBeNull();

                foreach (var kvp in settings.ChannelSpecific!)
                {
                    var original = loggerInfo.ChannelSpecific![kvp.Key];

                    ChannelInfoMatches(kvp.Value!, original);
                }
            }
        }

        private void ChannelInfoMatches( ChannelInfo parsed, ChannelInfo original )
        {
            parsed.IncludeSourcePath.Should().Be(original.IncludeSourcePath);
            parsed.MinimumLevel.Should().Be(original.MinimumLevel);
            parsed.OutputTemplate.Should().Be(original.OutputTemplate);
            parsed.RequireNewLine.Should().Be(original.RequireNewLine);
            parsed.SourceRootPath.Should().Be(original.SourceRootPath);
        }

    }
}
