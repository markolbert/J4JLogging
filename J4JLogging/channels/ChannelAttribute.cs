using System;
using System.Collections.Generic;

namespace J4JSoftware.Logging
{
    // attribute used to associate a name/ID with a particular log channel (e.g., console, debug).
    // Names/IDs should be unique but are not required to be so.
    [ AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false ) ]
    public class ChannelAttribute : Attribute
    {
        private sealed class ChannelSubChannelEqualityComparer : IEqualityComparer<ChannelAttribute>
        {
            public bool Equals( ChannelAttribute x, ChannelAttribute y )
            {
                if( ReferenceEquals( x, y ) ) return true;
                if( ReferenceEquals( x, null ) ) return false;
                if( ReferenceEquals( y, null ) ) return false;
                if( x.GetType() != y.GetType() ) return false;
                return string.Equals( x.ChannelID, y.ChannelID, StringComparison.OrdinalIgnoreCase );
            }

            public int GetHashCode( ChannelAttribute obj )
            {
                var hashCode = new HashCode();
                hashCode.Add( obj.ChannelID, StringComparer.OrdinalIgnoreCase );

                return hashCode.ToHashCode();
            }
        }

        public static IEqualityComparer<ChannelAttribute> DefaultComparer { get; } = new ChannelSubChannelEqualityComparer();

        public ChannelAttribute( string channelID )
        {
            ChannelID = !string.IsNullOrEmpty( channelID )
                ? channelID
                : throw new NullReferenceException( nameof(channelID) );
        }

        public string ChannelID { get; }
    }
}
