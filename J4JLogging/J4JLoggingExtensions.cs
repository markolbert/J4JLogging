using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class J4JLoggingExtensions
    {
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

            //foreach( var channel in Enum.GetValues(typeof(LogChannel) ).Cast<LogChannel>())
            //{
            //    if( config.IsChannelDefined( channel ) )
            //    {
            //        var channelInfo = config.Channels
            //            .FirstOrDefault( c => c.Channel == channel );

            //        if( channelInfo == null ) continue;

            //        switch( channel )
            //        {
            //            case LogChannel.All:
            //            case LogChannel.None:
            //                // nothing to do here; these are composite/helper values
            //                break;

            //            case LogChannel.Console:
            //                loggerConfig.WriteTo.Console( restrictedToMinimumLevel : channelInfo.MinimumLevel );
            //                break;

            //            case LogChannel.Debug:
            //                loggerConfig.WriteTo.Debug( restrictedToMinimumLevel: channelInfo.MinimumLevel );
            //                break;

            //            case LogChannel.File:
            //                var fileInfo = channelInfo as LogFileConfiguration ?? new LogFileConfiguration();

            //                var path = fileInfo.Location == LogFileLocation.AppData
            //                    ? DefineLocalAppDataLogPath( fileInfo.FileName, fileInfo.FilePath )
            //                    : DefineExeLogPath( fileInfo.FileName, fileInfo.FilePath );

            //                loggerConfig.WriteTo.File(
            //                    path : path,
            //                    restrictedToMinimumLevel : channelInfo.MinimumLevel,
            //                    rollingInterval : fileInfo.RollingInterval );

            //                break;

            //            default:
            //                throw new ArgumentOutOfRangeException($"Unsupported {nameof(LogChannel)} '{channel}'");
            //        }
            //    }
            //}

            return loggerConfig.CreateLogger();
        }

        //public static string DefineLocalAppDataLogPath( string fileStub, string folder = null )
        //{
        //    fileStub = IsFileNameValid( fileStub ) ? fileStub : "log.txt";

        //    if( string.IsNullOrEmpty( folder ) ) folder = GetProgramName();

        //    DirectoryInfo logDir = null;

        //    if( ( logDir = CreateLogFileDirectory( folder ) ) == null )
        //    {
        //        if( ( logDir = CreateLogFileDirectory( "LogFiles" ) ) == null )
        //            throw new ApplicationException(
        //                $"Couldn't create log file directory {folder} or the backup/default directory 'LogFiles'" );
        //    }

        //    return Path.Combine(logDir.FullName, fileStub);
        //}

        //public static string DefineExeLogPath( string fileStub, string folder = null )
        //{
        //    fileStub = IsFileNameValid( fileStub ) ? fileStub : "log.txt";

        //    if( folder != null && !IsFileNameValid( folder ) ) folder = null;

        //    return string.IsNullOrEmpty( folder )
        //        ? Path.Combine(Environment.CurrentDirectory, fileStub)
        //        : Path.Combine( Environment.CurrentDirectory, folder, fileStub );
        //}

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

        //private static bool IsFileNameValid( string fileName )
        //{
        //    return !string.IsNullOrEmpty( fileName )
        //           && fileName.IndexOfAny( Path.GetInvalidFileNameChars() ) >= 0
        //           && fileName.IndexOfAny( Path.GetInvalidPathChars() ) >= 0;
        //}

        //private static DirectoryInfo CreateLogFileDirectory( string folder )
        //{
        //    var appDataFolder = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
        //    var fullPath = Path.Combine( appDataFolder, folder );

        //    try
        //    {
        //        return Directory.CreateDirectory( fullPath );
        //    }
        //    catch( IOException ioException )
        //    {
        //        return null;
        //    }
        //}

        //private static string GetProgramName()
        //{
        //    var defaultName =
        //        Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().GetType().Assembly.Location );

        //    var trace = new StackTrace();

        //    // we want the last frame
        //    return trace.GetFrame( trace.FrameCount - 1 )?
        //               .GetMethod()?
        //               .DeclaringType?
        //               .Assembly?
        //               .GetName()?
        //               .Name
        //           ?? defaultName;
        //}
    }
}