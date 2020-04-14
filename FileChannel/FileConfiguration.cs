using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    [Channel("File")]
    public class FileConfiguration : ChannelConfiguration
    {
        public LogFileLocation Location { get; set; } = LogFileLocation.AppData;
        public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;
        public string FilePath { get; set; }
        public string FileName { get; set; } = "log.txt";

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            var path = Location == LogFileLocation.AppData
                ? DefineLocalAppDataLogPath( FileName, FilePath )
                : DefineExeLogPath( FileName, FilePath );

            return sinkConfig.File(
                path: path,
                restrictedToMinimumLevel: MinimumLevel,
                rollingInterval: RollingInterval );
        }

        public static string DefineLocalAppDataLogPath( string fileStub, string folder = null )
        {
            fileStub = IsFileNameValid( fileStub ) ? fileStub : "log.txt";

            if( string.IsNullOrEmpty( folder ) )
                folder = GetProgramName();

            DirectoryInfo logDir = null;

            if( ( logDir = CreateLogFileDirectory( folder ) ) == null )
            {
                if( ( logDir = CreateLogFileDirectory( "LogFiles" ) ) == null )
                    throw new ApplicationException(
                        $"Couldn't create log file directory {folder} or the backup/default directory 'LogFiles'" );
            }

            return Path.Combine( logDir.FullName, fileStub );
        }

        public static string DefineExeLogPath( string fileStub, string folder = null )
        {
            fileStub = IsFileNameValid( fileStub ) ? fileStub : "log.txt";

            if( folder != null && !IsFileNameValid( folder ) )
                folder = null;

            return string.IsNullOrEmpty( folder )
                ? Path.Combine( Environment.CurrentDirectory, fileStub )
                : Path.Combine( Environment.CurrentDirectory, folder, fileStub );
        }

        private static bool IsFileNameValid( string fileName )
        {
            return !string.IsNullOrEmpty( fileName )
                   && fileName.IndexOfAny( Path.GetInvalidFileNameChars() ) >= 0
                   && fileName.IndexOfAny( Path.GetInvalidPathChars() ) >= 0;
        }

        private static DirectoryInfo CreateLogFileDirectory( string folder )
        {
            var appDataFolder = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
            var fullPath = Path.Combine( appDataFolder, folder );

            try
            {
                return Directory.CreateDirectory( fullPath );
            }
            catch( IOException ioException )
            {
                return null;
            }
        }

        private static string GetProgramName()
        {
            var defaultName =
                Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().GetType().Assembly.Location );

            var trace = new StackTrace();

            // we want the last frame
            return trace.GetFrame( trace.FrameCount - 1 )?
                       .GetMethod()?
                       .DeclaringType?
                       .Assembly?
                       .GetName()?
                       .Name
                   ?? defaultName;
        }
    }
}