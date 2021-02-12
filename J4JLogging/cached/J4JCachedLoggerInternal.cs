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

namespace J4JSoftware.Logging
{
    public class J4JCachedLoggerInternal
    {
        private readonly J4JLoggerCache _cache;

        internal J4JCachedLoggerInternal( J4JLoggerCache cache )
        {
            _cache = cache;
        }

        private J4JCachedLoggerInternal( J4JCachedLoggerInternal toCopy )
        {
            _cache = toCopy._cache;
        }

        public Type? LoggedType { get; private set; }

        public J4JCachedLoggerInternal SetLoggedType( Type loggedType )
        {
            return new( this ) { LoggedType = loggedType };
        }

        public void Add(
            bool includeSms,
            LogEventLevel level,
            string template,
            string memberName,
            string sourcePath,
            int sourceLine,
            params object[] propertyValues )
        {
            _cache.Add( LoggedType, includeSms, level, template, memberName,
                sourcePath, sourceLine, propertyValues );
        }
    }
}