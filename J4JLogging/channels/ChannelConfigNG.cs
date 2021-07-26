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
using System.Text;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the base configuration for a log channel
    public abstract class ChannelConfigNG
    {
        protected ChannelConfigNG(
            J4JLogger logger
        )
        {
            Logger = logger;

            LoggedType = logger.LoggedType;
            IncludeSourcePath = logger.IncludeSourcePath;
            SourceRootPath = logger.SourceRootPath;
            MultiLineEvents = logger.MultiLineEvents;
            OutputTemplate = logger.OutputTemplate;
            RequireNewline = logger.RequireNewline;
        }

        protected internal J4JLogger Logger { get; }

        public Type? LoggedType { get; internal set; }
        public bool IncludeSourcePath { get; internal set; }
        public string? SourceRootPath { get; internal set; }
        public bool MultiLineEvents { get; internal set; }
        public string OutputTemplate { get; internal set; }
        public bool RequireNewline { get; internal set; }

        public abstract LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );

        public virtual bool IsValid => true;

        protected string EnrichedMessageTemplate
        {
            get
            {
                var sb = new StringBuilder( OutputTemplate );

                if( LoggedType != null )
                    sb.Append(" {SourceContext}{MemberName}");

                if(IncludeSourcePath)
                    sb.Append(" {SourceCodeInformation}");

                if( RequireNewline )
                    sb.Append( "{NewLine}" );

                return sb.ToString();
            }
        }
    }
}