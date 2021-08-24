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
using System.Collections.ObjectModel;
using System.Linq;
using Serilog.Context;

namespace J4JSoftware.Logging
{
    public class MessageTemplateManager : IMessageTemplateManager
    {
        private readonly List<BaseEnricher> _enrichers = new();
        private readonly List<IDisposable> _pushedProperties = new();

        public ReadOnlyCollection<BaseEnricher> Enrichers => _enrichers.AsReadOnly();

        public T? GetEnricher<T>()
            where T : BaseEnricher =>
            _enrichers.FirstOrDefault( x => x is T ) is T retVal ? retVal : null;

        public MessageTemplateManager AddEnricher<T>()
            where T : BaseEnricher, new()
        {
            if( _enrichers.Any( x => x is T ) )
                return this;

            _enrichers.Add(new T() );

            return this;
        }

        public void PushToLogContext()
        {
            _pushedProperties.Clear();

            foreach( var enricher in _enrichers )
            {
                if( !enricher.EnrichContext )
                    continue;

                _pushedProperties.Add( LogContext.PushProperty( enricher.PropertyName, enricher.GetValue() ) );
            }
        }

        public void DisposeFromLogContext()
        {
            foreach( var disposable in _pushedProperties )
            {
                disposable.Dispose();
            }

            _pushedProperties.Clear();
        }
    }
}