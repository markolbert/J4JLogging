using System;
using System.Collections.Generic;
using System.Linq;

namespace J4JSoftware.Logging
{
    public class J4JLoggerConfigurationBuilder
    {
        protected J4JLoggerConfigurationBuilder()
        {
        }

        protected Dictionary<string, Type> ChannelTypes { get; } =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        protected virtual bool TypeIsValid(Type channelType, out ChannelAttribute attr)
        {
            attr = null;

            if (channelType == null
                || !(typeof(IChannelConfiguration).IsAssignableFrom(channelType)))
                return false;

            if (channelType.GetConstructor(Type.EmptyTypes) == null)
                return false;

            // check that TChannel is decorated with the required attribute
            attr = channelType.GetCustomAttributes(typeof(ChannelAttribute), false)
                .Cast<ChannelAttribute>()
                .FirstOrDefault();

            return attr != null;
        }

        public J4JLoggerConfigurationBuilder AddChannel<TChannel>()
            where TChannel : LogChannel
            => AddChannel(typeof(TChannel));

        public J4JLoggerConfigurationBuilder AddChannel(Type channelType)
        {
            if( TypeIsValid( channelType, out var attr ) )
            {
                if( ChannelTypes.ContainsKey( attr.ChannelID ) ) ChannelTypes[ attr.ChannelID ] = channelType;
                else ChannelTypes.Add( attr.ChannelID, channelType );
            }

            return this;
        }
    }
}