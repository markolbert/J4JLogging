using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class J4JLoggingExtensions
    {
        public static string GetConfigValue(this IConfigurationRoot configRoot, string path, string value = null)
        {
            return configRoot.AsEnumerable()
                .SingleOrDefault( kvp =>
                    Regex.IsMatch( kvp.Key, path, RegexOptions.IgnoreCase )
                    && ( string.IsNullOrEmpty( value )
                         || kvp.Value.Equals( value, StringComparison.OrdinalIgnoreCase ) ) )
                .Value;
        }

        public static ILogger CreateLogger( this IJ4JLoggerConfiguration config )
        {
            if( config == null )
                return null;

            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .SetMinimumLevel( config.Channels?.Min( c => c.MinimumLevel ) ?? LogEventLevel.Verbose );

            if( config.Channels != null )
            {
                foreach( var channel in config.Channels )
                {
                    channel.Configure( loggerConfig.WriteTo );
                }
            }

            return loggerConfig.CreateLogger();
        }

        private static LoggerConfiguration SetMinimumLevel( this LoggerConfiguration config, LogEventLevel minLevel )
        {
            if( config == null )
                return null;

            switch( minLevel )
            {
                case LogEventLevel.Debug:
                    config.MinimumLevel.Debug();
                    break;

                case LogEventLevel.Error:
                    config.MinimumLevel.Error();
                    break;

                case LogEventLevel.Fatal:
                    config.MinimumLevel.Fatal();
                    break;

                case LogEventLevel.Information:
                    config.MinimumLevel.Information();
                    break;

                case LogEventLevel.Verbose:
                    config.MinimumLevel.Verbose();
                    break;

                case LogEventLevel.Warning:
                    config.MinimumLevel.Warning();
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof(minLevel), minLevel, null );
            }

            return config;
        }
    }
}