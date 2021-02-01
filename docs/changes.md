### Change Log

#### Changes to v3.2
I moved the Twilio SMS stuff into a separate assembly because it's not commonly used
when logging and is large relative to the rest of the code base.

The libraries are now licensed under the GNU GPL-v3.0 (or later) license.

#### Changes to v3.1
I've modified, once again, how the output channels are configured when
initializing the logger. 

You can set up a class implementing `IJ4JLoggerConfiguration` (e.g.,
`J4JLoggerConfiguration`) manually and add the channels you want to its
`Channels` property.

Or, if you're using the Net5 `IConfiguration` system you can implement
an instance of `IChannelConfigProvider` and use the Autofac registration
methods to do the work for you. See the [configuration section](docs/configuration.md)
section for more details.

#### Significant Changes to v3
- The libraries now target Net5 only, and have null checking enabled.
- I consolidated all the default channels into the base J4JLogger assembly. Having
them in separate assemblies complicated things.
- The way log channels are configured was changed substantially (mostly because 
even the author found the earlier approach difficult to remember :)).
- The `Autofac`-based setup approach was simplified.
- To make logging possible before a program is fully set up a cached implementation
 of IJ4JLogger was added. The contents of the cache can be easily dumped into the actual
 logging system once it's established.
 
