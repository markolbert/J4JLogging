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
using System.Collections;
using System.Collections.Generic;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JLoggerCache : IEnumerable<CachedEntry>
    {
        private readonly List<CachedEntry> _entries = new();

        public IEnumerator<CachedEntry> GetEnumerator()
        {
            foreach( var entry in _entries ) yield return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(
            Type? loggedType,
            bool includeSms,
            LogEventLevel level,
            string template,
            string memberName,
            string sourcePath,
            int sourceLine,
            params object[] propertyValues )
        {
            _entries.Add( new CachedEntry
            {
                LoggedType = loggedType,
                IncludeSms = includeSms,
                LogEventLevel = level,
                Template = template,
                MemberName = memberName,
                SourcePath = sourcePath,
                SourceLine = sourceLine,
                PropertyValues = propertyValues
            } );
        }

        public void Clear()
        {
            _entries.Clear();
        }
    }
}