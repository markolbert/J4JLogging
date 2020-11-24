using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using Serilog.Events;
using Twilio.Types;
#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    // Base class for containing the information needed to configure an instance of FileChannel
    public class FileConfig : J4JChannelConfig
    {
        public LogFileLocation Location { get; set; } = LogFileLocation.ExeFolder;
        public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;
        public string FilePath { get; set; } = Environment.CurrentDirectory;
        public string FileName { get; set; } = "log.txt";

        public override bool IsValid
        {
            get
            {
                if( string.IsNullOrEmpty( FilePath ) ) return false;
                if( string.IsNullOrEmpty( FileName ) ) return false;

                return Directory.Exists( FilePath );
            }
        }
    }
}