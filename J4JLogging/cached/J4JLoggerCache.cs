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
    public class J4JLoggerCache : IEnumerable<CachedEntries>
    {
        private readonly List<CachedEntries> _entries = new();

        private CachedEntries _context = new( null, false, null, false, false );

        public CachedEntries Context
        {
            get => _context;

            set
            {
                _context = value;

                _entries.Add( value );
            }
        }

        public void Clear( bool resetContext = false )
        {
            _entries.Clear();

            if( resetContext )
                Context = new CachedEntries( null, false, null, false, false );
        }

        public IEnumerator<CachedEntries> GetEnumerator()
        {
            foreach (var entry in _entries)
            {
                yield return entry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}