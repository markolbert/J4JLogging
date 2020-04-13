using System;
using System.Collections.Generic;
using System.Text;

namespace J4JSoftware.Logging
{
    [ AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false ) ]
    public class LogChannelAttribute : Attribute
    {
        private sealed class ChannelSubChannelEqualityComparer : IEqualityComparer<LogChannelAttribute>
        {
            public bool Equals( LogChannelAttribute x, LogChannelAttribute y )
            {
                if( ReferenceEquals( x, y ) ) return true;
                if( ReferenceEquals( x, null ) ) return false;
                if( ReferenceEquals( y, null ) ) return false;
                if( x.GetType() != y.GetType() ) return false;
                return string.Equals( x.Channel, y.Channel, StringComparison.OrdinalIgnoreCase );
            }

            public int GetHashCode( LogChannelAttribute obj )
            {
                var hashCode = new HashCode();
                hashCode.Add( obj.Channel, StringComparer.OrdinalIgnoreCase );

                return hashCode.ToHashCode();
            }
        }

        public static IEqualityComparer<LogChannelAttribute> DefaultComparer { get; } = new ChannelSubChannelEqualityComparer();

        private readonly LogChannel _channel;
        private readonly string _subChannel;

        public LogChannelAttribute( LogChannel channel )
        {
            var text = Enum.Format( typeof(LogChannel), channel, "F" );

            if( string.IsNullOrEmpty( text ) || text.IndexOf( ",", StringComparison.OrdinalIgnoreCase ) >= 0 )
                throw new ArgumentException( $"{nameof(channel)} cannot be a composite value ({text})" );

            _channel = channel;
        }

        public LogChannelAttribute( LogChannel channel, string subChannel )
            : this( channel )
        {
            _subChannel = subChannel;
        }

        public string Channel => string.IsNullOrEmpty( _subChannel ) ? _channel.ToString() : _subChannel;
    }
}
