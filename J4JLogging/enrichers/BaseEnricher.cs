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
using System.Runtime.CompilerServices;
using Serilog.Core;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public abstract class BaseEnricher : ILogEventEnricher
    {
        private sealed class BaseEnricherEqualityComparer : IEqualityComparer<BaseEnricher>
        {
            public bool Equals( BaseEnricher? x, BaseEnricher? y )
            {
                if( x == null && y == null )
                    return true;
                
                if( x != null && y != null )
                    return x.GetType() == y.GetType();

                return false;
            }

            public int GetHashCode( BaseEnricher obj )
            {
                var hashCode = new HashCode();
                hashCode.Add( obj.PropertyName, StringComparer.OrdinalIgnoreCase );
                hashCode.Add( obj.CachedProperty );
                hashCode.Add( obj.EnrichContext );
                return hashCode.ToHashCode();
            }
        }

        public static IEqualityComparer<BaseEnricher> BaseEnricherComparer { get; } = new BaseEnricherEqualityComparer();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static LogEventProperty CreateProperty(
            ILogEventPropertyFactory propertyFactory,
            string propertyName,
            object value) =>
            propertyFactory.CreateProperty(propertyName, value);

        protected BaseEnricher( string propertyName )
        {
            PropertyName = propertyName;
        }

        protected string PropertyName { get; }
        protected LogEventProperty? CachedProperty { get; private set; }
        protected virtual bool EnrichContext { get; }
        protected abstract object GetValue();

        public void Enrich( LogEvent logEvent, ILogEventPropertyFactory propertyFactory )
        {
            if( EnrichContext )
                logEvent.AddPropertyIfAbsent( GetLogEventProperty( propertyFactory ) );
            else logEvent.RemovePropertyIfPresent( PropertyName );
        }

        private LogEventProperty GetLogEventProperty( ILogEventPropertyFactory propertyFactory )
        {
            CachedProperty ??= CreateProperty( propertyFactory, PropertyName, GetValue() );
            return CachedProperty;
        }
    }
}