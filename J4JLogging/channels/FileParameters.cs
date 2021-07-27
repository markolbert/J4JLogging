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

namespace J4JSoftware.Logging
{
    public record FileParameters : ChannelParameters
    {
        private readonly RollingInterval _interval = RollingInterval.Day;
        private readonly string _folder = Environment.CurrentDirectory;
        private readonly string _fileName = "log.txt";

        public FileParameters(
            J4JLogger logger )
            : base( logger )
        {
        }

        public RollingInterval RollingInterval
        {
            get => _interval;
            init => SetProperty( ref _interval, value );
        }

        public string Folder
        {
            get => _folder;
            init => SetProperty( ref _folder, value );
        }

        public string FileName
        {
            get => _folder;
            init => SetProperty( ref _fileName, value );
        }

        public string FileTemplatePath => Path.Combine( Folder, FileName );
    }
}