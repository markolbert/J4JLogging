### Adding a Channel

Adding a `LogChannel` to the framework is a matter of defining a
class which implements `ILogChannel` and handles configuration properly.

As an example, consider `FileChannel`, which wraps Serilog's **File** sink:
```csharp
[Channel("File")]
public partial class FileChannel : LogChannel
{
    public FileChannel()
    {
    }

    public FileChannel( IConfigurationRoot configRoot, string loggerSection = "Logger" )
        : base( configRoot, loggerSection )
    {
        var text = configRoot.GetConfigValue( $@"{loggerSection}:Channels:\d:{nameof(Location)}" );
        if( !String.IsNullOrEmpty( text ) )
            Location = Enum.Parse<LogFileLocation>( text, true );

        text = configRoot.GetConfigValue( $@"{loggerSection}:Channels:\d:{nameof(RollingInterval)}" );
        if( !string.IsNullOrEmpty( text ) )
            RollingInterval = Enum.Parse<RollingInterval>( text, true );

        text = configRoot.GetConfigValue( $@"{loggerSection}:Channels:\d:{nameof(FilePath)}" );
        if( !string.IsNullOrEmpty( text ) )
            FilePath = text;

        text = configRoot.GetConfigValue($@"{loggerSection}:Channels:\d:{nameof(FileName)}");
        if (!string.IsNullOrEmpty(text))
            FileName = text;
    }

    public LogFileLocation Location { get; set; } = LogFileLocation.AppData;
    public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;
    public string FilePath { get; set; }
    public string FileName { get; set; } = "log.txt";

    public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig, string outputTemplate = null )
    {
        var path = Location == LogFileLocation.AppData
            ? DefineLocalAppDataLogPath( FileName, FilePath )
            : DefineExeLogPath( FileName, FilePath );

        return string.IsNullOrEmpty( outputTemplate )
            ? sinkConfig.File( path : path, restrictedToMinimumLevel : MinimumLevel,
                rollingInterval : RollingInterval )
            : sinkConfig.File( path : path, restrictedToMinimumLevel : MinimumLevel,
                rollingInterval : RollingInterval, outputTemplate : outputTemplate );
    }
}
```
This is only part of the definition for `FileChannel`. The rest consists of
static methods used by the part of the code shown above and some extension
utility methods.

There are two different constructors to implement, one for when you're using
*derived* configuration information (the configuration file's structure maps
directly to `IJ4JLoggerConfiguration`) or an *embedded* configuration (the
configuration information is contained in a section of the configuration
file). Embedded configurations are built off of Microsoft's
`ConfigurationBuilder` and revolve around `IConfigurationRoot`.

The channel configruation information uniquely needed by `FileChannel` is
contained in a series of public properties. These are either set in the
constructor, for *embedded* configurations, or must be set manually for
*derived* configurations. The latter can be simplified by using 
`J4JLoggerConfigurationJsonBuilder`.

The `GetConfigValue()` extension method is defined as follows:
```csharp
// Gets an IConfigurationRoot value given a path to that value and, optionally, the value of the value.
public static string GetConfigValue(this IConfigurationRoot configRoot, string path, string? value = null)
{
    return configRoot.AsEnumerable()
        .SingleOrDefault( kvp =>
            Regex.IsMatch( kvp.Key, path, RegexOptions.IgnoreCase )
            && ( string.IsNullOrEmpty( value )
                    || kvp.Value.Equals( value, StringComparison.OrdinalIgnoreCase ) ) )
        .Value;
}
```