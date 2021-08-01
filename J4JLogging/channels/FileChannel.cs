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

namespace J4JSoftware.Logging
{
    // Base class for containing the information needed to configure an instance of FileChannel
    [ChannelID("File", typeof(FileChannel))]
    public class FileChannel : Channel
    {
        private RollingInterval _interval = RollingInterval.Day;
        private string _folder = Environment.CurrentDirectory;
        private string _fileName = "log.txt";

        public RollingInterval RollingInterval
        {
            get => _interval;
            set => SetPropertyAndNotifyLogger(ref _interval, value);
        }

        public string Folder
        {
            get => _folder;
            set => SetPropertyAndNotifyLogger(ref _folder, value);
        }

        public string FileName
        {
            get => _folder;
            set => SetPropertyAndNotifyLogger(ref _fileName, value);
        }

        public string FileTemplatePath => Path.Combine(Folder, FileName);

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.File( FileTemplatePath,
                MinimumLevel,
                EnrichedMessageTemplate,
                rollingInterval: RollingInterval);
        }
    }
}