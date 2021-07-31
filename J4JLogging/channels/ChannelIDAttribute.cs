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

            if( channelType.IsGenericType )
                throw new ArgumentException($"Supplied type '{channelType.Name}' is a generic type");

            if (channelType.IsAbstract)
                throw new ArgumentException($"Supplied type '{channelType.Name}' is an abstract type");

            if( !channelType.GetConstructors().Any( c =>
            {
                var parameters = c.GetParameters();

                return parameters.Length == 1
                       && typeof(J4JLogger).IsAssignableFrom( parameters[ 0 ].ParameterType );
            } ) )
                throw new ArgumentException(
                    $"Supplied type '{channelType.Name}' does not have a public constructor taking a single {nameof(J4JLogger)} parameter" );

            var channelInterface = channelType.GetInterfaces()
                .FirstOrDefault( i => i.IsGenericType
                                      && typeof(IChannel<>) == i.GetGenericTypeDefinition()
                                      && i.GenericTypeArguments.Length == 1
                                      && typeof(ChannelParameters).IsAssignableFrom( i.GenericTypeArguments[ 0 ] ) );

            if( channelInterface == null )
                throw new ArgumentException(
                    $"Supplied type '{channelType.Name}' does not implement {typeof(IChannel<>)}" );

            ChannelType = channelType;

            ParametersType = channelInterface.GenericTypeArguments[ 0 ];
        }

        public string Name { get; }
        public Type ChannelType { get; }
        public Type ParametersType { get; }
    }
}