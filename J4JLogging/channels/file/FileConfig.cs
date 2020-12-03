using System;
using System.IO;
using Serilog;
using Serilog.Configuration;

#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    // Base class for containing the information needed to configure an instance of FileChannel
    public class FileConfig : ChannelConfig
    {
        public LogFileLocation Location { get; set; } = LogFileLocation.ExeFolder;
        public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;
        public string Folder { get; set; } = Environment.CurrentDirectory;
        public string FileName { get; set; } = "log.txt";

        public string GetFileTemplatePath()
        {
            var folder = Path.IsPathFullyQualified( Folder )
                ? Folder
                : Location switch
                {
                    LogFileLocation.Absolute => Environment.CurrentDirectory,
                    LogFileLocation.AppData => Path.Combine(
                        Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ),
                        Folder ),
                    LogFileLocation.ExeFolder => Path.Combine( Environment.CurrentDirectory ),
                    _ => throw new ArgumentException( $"Unhandled {typeof(LogFileLocation)}" )
                };

            return Path.Combine( folder, FileName );
        }

        public override bool IsValid
        {
            get
            {
                try
                {
                    var junk = GetFileTemplatePath();
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        public override LoggerConfiguration Configure(LoggerSinkConfiguration sinkConfig) =>
            Serilog.FileLoggerConfigurationExtensions.File(
                sinkConfig,
                path: GetFileTemplatePath(),
                restrictedToMinimumLevel: MinimumLevel,
                outputTemplate: OutputTemplate,
                rollingInterval: RollingInterval);
    }
}