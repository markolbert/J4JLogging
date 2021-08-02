### Adding a Channel

Channels are a concept I added to the Serilog environment because I wanted to be able 
to configure multiple Serilog sinks in a generalized way. Adding a channel is easy:
you define a configuration class for the channel which implements `IChannel`:
```csharp
public interface IChannel
{
    bool IncludeSourcePath { get; set; }
    string? SourceRootPath { get; set; }
    string OutputTemplate { get; set; }
    bool RequireNewLine { get; set; }
    LogEventLevel MinimumLevel { get; set; }
        
    string EnrichedMessageTemplate { get; }

    void ResetIncludeSourcePath();
    void ResetOutputTemplate();
    void ResetRequireNewLine();
    void ResetMinimumLevel();
    void ResetToGlobal();

    void SetAssociatedLogger(J4JBaseLogger? logger);
    LoggerConfiguration Configure(LoggerSinkConfiguration sinkConfig);
}
```
To make this even easier there's a base class, `Channel`, from which you
derive your own channel configuration class. It implements the general properties all channels
must support, and provides ways to tie a channel's configuration to the global configuration
defined in `J4JLogger`. Here's an example of how it does that for the `MinimumLevel` property,
plus the important general bits you should be aware of:
```csharp
public abstract class Channel : IChannel
{
    private J4JBaseLogger? _logger;
    private bool _loggerMustBeUpdated = false;

    private Func<LogEventLevel>? _globalMinLevel;
    private LogEventLevel? _minLevel;

    protected Channel()
    {
    }

    public void SetAssociatedLogger(J4JBaseLogger? logger)
    {
        _logger = logger;

        _globalInclSrcPath = _logger?.GetGlobalAccessor(x => x.IncludeSourcePath);
        _globalSrcPath = _logger?.GetGlobalAccessor(x => x.SourceRootPath);
        _globalOutputTemplate = _logger?.GetGlobalAccessor(x => x.OutputTemplate);
        _globalRequireNewLine = _logger?.GetGlobalAccessor(x => x.RequireNewLine);
        _globalMinLevel = _logger?.GetGlobalAccessor(x => x.MinimumLevel);

        if (!_loggerMustBeUpdated)
            return;

        _logger?.ResetBaseLogger();
        _loggerMustBeUpdated = false;
    }

    public LogEventLevel MinimumLevel
    {
        get => _minLevel ?? _globalMinLevel?.Invoke() ?? LogEventLevel.Verbose;
        set => SetPropertyAndNotifyLogger(ref _minLevel, value);
    }

    public void ResetMinimumLevel() => _minLevel = null;

    protected void SetPropertyAndNotifyLogger<TProp>(ref TProp field, TProp value)
    {
        var changed = !EqualityComparer<TProp>.Default.Equals(field, value);

        field = value;

        if (!changed) return;

        if (_logger == null)
            _loggerMustBeUpdated = true;
        else
        {
            _logger.ResetBaseLogger();
            _loggerMustBeUpdated = false;
        }
    }

    public abstract LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );
}
```
Because we want a logger channel's properties default to the global values defined in 
`J4JLogger` every channel property is defined as a nullable type. 

Each property also must have a getter function which can extract the corresponding global 
value from an instance of `J4JLogger`. The specific instance to use, the logger to which 
the channel belongs, is set by calling `SetAssociatedLogger()` (that's done automatically by
`J4JLogger`). That method call takes care of setting up the extractor getters.

When a channel's property is given an overriding value different from what it currently
has (which, remember, starts out as null) the `J4JLogger` instance must be notified. This
is done in the `SetPropertyAndNotifyLogger()` method, which also updates a property's
underlying field. If the associated logger is not yet set when a property 
updates the API records the need for a pending update whenever the associated logger is set.

The only method you must implement/override is `Configure()`. You can also add 
whatever additional configuration properties are needed for a particular channel. 
A good example of this is `FileConfig`, which holds the information for configuring 
a rolling log file sink and "knows" how to configure that particular sink.
