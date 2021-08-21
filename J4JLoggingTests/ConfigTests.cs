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
        [ Theory ]
        [ MemberData( nameof(SimpleData.TestData), MemberType = typeof(SimpleData) ) ]
        public void Simple( string filePath, LoggerInfo loggerInfo )
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, filePath ) )
                .Build();

            var settings = configRoot.Get<LoggerInfo>();

            CheckSourceVersusStored( loggerInfo, settings );
        }

        [ Theory ]
        [ MemberData( nameof(EmbeddedData.TestData), MemberType = typeof(EmbeddedData) ) ]
        public void Embedded( string filePath, LoggerInfo loggerInfo )
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, filePath ) )
                .Build();

            var settings = configRoot.GetSection( "LoggerInfo" ).Get<LoggerInfo>();

            CheckSourceVersusStored( loggerInfo, settings );
        }

        [ Theory ]
        [ MemberData( nameof(DerivedData.TestData), MemberType = typeof(DerivedData) ) ]
        public void Derived( string filePath, LoggerInfo loggerInfo )
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, filePath ) )
                .Build();

            var settings = configRoot.Get<LoggerInfo>();

            CheckSourceVersusStored( loggerInfo, settings );
        }

        private void CheckSourceVersusStored( LoggerInfo loggerInfo, LoggerInfo settings )
        {
            settings.Global.Should().NotBeNull();

            CheckCommonParameters( settings.Global!, loggerInfo!.Global! );

            if( loggerInfo.ChannelSpecific.Count == 0 )
                settings.ChannelSpecific.Should().BeEmpty();
            else
            {
                settings.ChannelSpecific.Should().NotBeNull();

                foreach( var kvp in settings.ChannelSpecific! )
                {
                    var original = loggerInfo.ChannelSpecific![ kvp.Key ];
                    CheckChannelParameters( kvp.Value, original );
                }
            }
        }

        private static void CheckChannelParameters(
            ChannelConfiguration parsed,
            ChannelConfiguration original )
        {
            switch( parsed )
            {
                case FileConfiguration fileParsed:
                    if( original is not FileConfiguration fileOriginal )
                        throw new ArgumentException(
                            $"Original parameter type ({original.GetType()}) does not match parsed parameter type ({parsed.GetType()})" );

                    CheckFileParameters( fileParsed, fileOriginal );

                    break;

                case TwilioConfiguration twilioParsed:
                    if( original is not TwilioConfiguration twilioOriginal )
                        throw new ArgumentException(
                            $"Original parameter type ({original.GetType()}) does not match parsed parameter type ({parsed.GetType()})" );

                    CheckTwilioParameters( twilioParsed, twilioOriginal );

                    break;

                default:
                    CheckCommonParameters( parsed, original );
                    break;
            }
        }

        private static void CheckCommonParameters(
            ChannelConfiguration parsed,
            ChannelConfiguration original )
        {
            parsed.IncludeSourcePath.Should().Be( original.IncludeSourcePath );
            parsed.MinimumLevel.Should().Be( original.MinimumLevel );
            parsed.OutputTemplate.Should().Be( original.OutputTemplate );
            parsed.RequireNewLine.Should().Be( original.RequireNewLine );
            parsed.SourceRootPath.Should().Be( original.SourceRootPath );
        }

        private static void CheckFileParameters(
            FileConfiguration parsed,
            FileConfiguration original )
        {
            CheckCommonParameters( parsed, original );

            parsed.RollingInterval.Should().Be( original.RollingInterval );
            parsed.Folder.Should().Be( original.Folder );
            parsed.FileName.Should().Be( original.FileName );
        }

        private static void CheckTwilioParameters(
            TwilioConfiguration parsed,
            TwilioConfiguration original )
        {
            CheckCommonParameters( parsed, original );

            parsed.AccountToken.Should().Be( original.AccountToken );
            parsed.AccountSID.Should().Be( original.AccountSID );
            parsed.FromNumber.Should().Be( original.FromNumber );
        }
    }
}
