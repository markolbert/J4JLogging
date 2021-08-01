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
using System.Linq;

namespace J4JSoftware.Logging
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChannelIDAttribute : Attribute
    {
        public ChannelIDAttribute( string name, Type channelType )
        {
            if( string.IsNullOrEmpty( name ) )
                throw new ArgumentException( "Supplied J4JLogger Channel name cannot be empty" );

            Name = name;

            if( !typeof(IChannel).IsAssignableFrom(channelType))
                throw new ArgumentException($"Supplied type '{channelType.Name}' does not implement {nameof(IChannel)}");

            if (channelType.IsAbstract)
                throw new ArgumentException($"Supplied type '{channelType.Name}' is an abstract type");

            if( channelType.GetConstructors().All( c => c.GetParameters().Length != 0 ) )
                throw new ArgumentException(
                    $"Supplied type '{channelType.Name}' does not have a public parameterless constructor" );

            ChannelType = channelType;
        }

        public string Name { get; }
        public Type ChannelType { get; }
    }
}