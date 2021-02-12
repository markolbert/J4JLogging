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
using Serilog.Events;

#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    public class CachedEntry
    {
        public Type? LoggedType { get; set; }
        public bool IncludeSms { get; set; }
        public LogEventLevel LogEventLevel { get; set; }
        public string Template { get; set; }
        public string MemberName { get; set; }
        public string SourcePath { get; set; }
        public int SourceLine { get; set; }
        public object[] PropertyValues { get; set; }
    }
}