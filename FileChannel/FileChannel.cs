using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    [Channel("File")]
    public partial class FileChannel : LogChannel
    {
        public FileChannel()
        {
        }

        public FileChannel( IConfigurationRoot configRoot, string loggerSection = "Logger" )
            : base( configRoot, loggerSection )
        {
            var text = configRoot.GetConfigValue( $@"{loggerSection}:Channels:\d:{nameof(Location)}" );
            if( !String.IsNullOrEmpty( text ) )
                Location = Enum.Parse<LogFileLocation>( text, true );

            text = configRoot.GetConfigValue( $@"{loggerSection}:Channels:\d:{nameof(RollingInterval)}" );
            if( !string.IsNullOrEmpty( text ) )
                RollingInterval = Enum.Parse<RollingInterval>( text, true );

            text = configRoot.GetConfigValue( $@"{loggerSection}:Channels:\d:{nameof(FilePath)}" );
            if( !string.IsNullOrEmpty( text ) )
                FilePath = text;

            text = configRoot.GetConfigValue($@"{loggerSection}:Channels:\d:{nameof(FileName)}");
            if (!string.IsNullOrEmpty(text))
                FileName = text;
        }

        public LogFileLocation Location { get; set; } = LogFileLocation.AppData;
        public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;
        public string FilePath { get; set; }
        public string FileName { get; set; } = "log.txt";

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig, string? outputTemplate = null )
        {
            var path = Location == LogFileLocation.AppData
                ? DefineLocalAppDataLogPath( FileName, FilePath )
                : DefineExeLogPath( FileName, FilePath );

            return string.IsNullOrEmpty( outputTemplate )
                ? sinkConfig.File( path : path, restrictedToMinimumLevel : MinimumLevel,
                    rollingInterval : RollingInterval )
                : sinkConfig.File( path : path, restrictedToMinimumLevel : MinimumLevel,
                    rollingInterval : RollingInterval, outputTemplate : outputTemplate );
        }
    }
}