using System;
using System.Collections.Generic;
using System.Linq;

namespace J4JSoftware.Logging
{
    // base class for building J4JLogging configuration instances
    public class J4JLoggerConfigurationBuilder
    {
        protected J4JLoggerConfigurationBuilder()
        {
        }

        protected Dictionary<string, Type> ChannelTypes { get; } =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        // Adds TChannel to the ChannelTypes collection, provided it's a valid ILogChannel type.
        // If it isn't, the specified type is ignored.
        public J4JLoggerConfigurationBuilder AddChannel<TChannel>()
            where TChannel : LogChannel
            => AddChannel(typeof(TChannel));

        // Adds TChannel to the ChannelTypes collection, provided it's a valid ILogChannel type.
        // If it isn't, the specified type is ignored.
        public J4JLoggerConfigurationBuilder AddChannel(Type channelType)
        {
            if( TypeIsValid( channelType, out var attr ) )
            {
                if( ChannelTypes.ContainsKey( attr!.ChannelID ) ) ChannelTypes[ attr.ChannelID ] = channelType;
                else ChannelTypes.Add( attr.ChannelID, channelType );
            }

            return this;
        }

        // determines whether the supplied type is a valid ILogChannel Type (i.e., has a public
        // parameterless constructor, implements ILogChannel and is decorated with a ChannelAttribute).
        // If so, returns the ChannelAttribute decorating the type.
        protected virtual bool TypeIsValid(Type channelType, out ChannelAttribute? attr)
        {
            attr = null;

            if (channelType == null
                || !(typeof(IChannelConfig).IsAssignableFrom(channelType)))
                return false;

            if (channelType.GetConstructor(Type.EmptyTypes) == null)
                return false;

            // check that TChannel is decorated with the required attribute
            attr = channelType.GetCustomAttributes(typeof(ChannelAttribute), false)
                .Cast<ChannelAttribute>()
                .FirstOrDefault();

            return attr != null;
        }
    }
}