using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // Miscellaneous utility extension methods
    public static class J4JLoggingExtensions
    {
        // Gets an IConfigurationRoot value given a path to that value and, optionally, the value of the value.
        public static string GetConfigValue(this IConfigurationRoot configRoot, string path, string? value = null)
        {
            return configRoot.AsEnumerable()
                .SingleOrDefault( kvp =>
                    Regex.IsMatch( kvp.Key, path, RegexOptions.IgnoreCase )
                    && ( string.IsNullOrEmpty( value )
                         || kvp.Value.Equals( value, StringComparison.OrdinalIgnoreCase ) ) )
                .Value;
        }

        // Creates an instance of Serilog's ILogger to be wrapped by IJ4JLogger. Configures the
        // Serilog ILogger instance to work with IJ4JLogger and sets up the configured channels.
        public static ILogger? CreateBaseLogger( this IJ4JLoggerConfiguration config )
        {
            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext();

            var minLevel = LogEventLevel.Fatal;

            foreach( var channel in config )
            {
                if( channel.MinimumLevel < minLevel ) 
                    minLevel = channel.MinimumLevel;

                channel.Configure( loggerConfig.WriteTo );
            }

            loggerConfig.SetMinimumLevel( minLevel );

            return loggerConfig.CreateLogger();
        }

        private static LoggerConfiguration SetMinimumLevel( this LoggerConfiguration config, LogEventLevel minLevel )
        {
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