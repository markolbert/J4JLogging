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
using System.Collections.Generic;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public abstract class J4JEnricher : ILogEventEnricher
    {
        private sealed class EnricherQualityComparer : IEqualityComparer<J4JEnricher>
        {
            public bool Equals( J4JEnricher? x, J4JEnricher? y )
            {
                if( ReferenceEquals( x, y ) ) return true;
                if( ReferenceEquals( x, null ) ) return false;
                if( ReferenceEquals( y, null ) ) return false;
                if( x.GetType() != y.GetType() ) return false;

                return x.EnricherID.Equals( y.EnricherID, StringComparison.OrdinalIgnoreCase );
            }

            public int GetHashCode( J4JEnricher obj )
            {
                return obj.EnricherID.GetHashCode();
            }
        }

        public static IEqualityComparer<J4JEnricher> DefaultComparer { get; } = new EnricherQualityComparer();

        protected J4JEnricher(
            string propName
            )
        {
            PropertyName = propName;
        }

        public string PropertyName { get; }
        public virtual string EnricherID => PropertyName;
        public virtual bool EnrichContext { get; }
        
        public abstract object GetValue();

        public void Enrich( LogEvent logEvent, ILogEventPropertyFactory propertyFactory )
        {
            if( !EnrichContext )
                return;

            logEvent.AddPropertyIfAbsent( propertyFactory.CreateProperty( PropertyName, GetValue() ) );
        }
    }
}