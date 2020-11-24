using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Configuration;
#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    public partial class FileChannel : LogChannel
    {
        public FileChannel(IJ4JLoggerConfiguration config, FileConfig channelConfig)
            : base(config, channelConfig)
        {
            Location = channelConfig.Location;
            RollingInterval = channelConfig.RollingInterval;
            FilePath = channelConfig.FilePath;
            FileName = channelConfig.FileName;
        }

        public FileChannel( IOptions<J4JLoggerConfiguration> config, IOptions<FileConfig> channelConfig )
            : base(config.Value, channelConfig.Value)
        {
            Location = channelConfig.Value.Location;
            RollingInterval = channelConfig.Value.RollingInterval;
            FilePath = channelConfig.Value.FilePath;
            FileName = channelConfig.Value.FileName;
        }

        public LogFileLocation Location { get; } 
        public RollingInterval RollingInterval { get; }
        public string FilePath { get; }
        public string FileName { get; }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            var path = Location == LogFileLocation.AppData
                ? DefineLocalAppDataLogPath( FileName, FilePath )
                : DefineExeLogPath( FileName, FilePath );

            return string.IsNullOrEmpty( OutputTemplate )
                ? sinkConfig.File( path : path, restrictedToMinimumLevel : MinimumLevel,
                    rollingInterval : RollingInterval )
                : sinkConfig.File( path : path, restrictedToMinimumLevel : MinimumLevel,
                    rollingInterval : RollingInterval,
                    outputTemplate : LoggerConfiguration.EnrichMessageTemplate( OutputTemplate ) );
        }
    }
}