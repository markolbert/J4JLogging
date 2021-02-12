#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLogging' is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
// 
// This library or program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with
// this library or program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

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

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.File( GetFileTemplatePath(),
                MinimumLevel,
                EnrichedMessageTemplate,
                rollingInterval: RollingInterval );
        }
    }
}