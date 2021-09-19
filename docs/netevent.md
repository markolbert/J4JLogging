# NetEventSink

I've found there are times when I want to display log events in a UI (e.g., a WPF app). To simplify doing this I created `NetEventSink`.

Unlike other sinks you don't add it directly to `Serilog`'s `LoggerConfiguration`. Instead, you add it to an instance of `J4JLoggerConfiguration`, which takes care of adding the sink to `Serilog` behind the scenes:

```csharp
loggerConfig.NetEvent( outputTemplate: outputTemplate,
    restrictedToMinimumLevel: NetEventConfiguration.MinimumLevel );
```

You can specify both an output template and a minimum log event level when adding the sink. In general I use a stripped-down output template, like `[{Level:u3}] {Message:lj}`, so as to not clutter the visual display too much.

That approach is necessary so that when you build an instance of `IJ4JLogger` it's aware of the sink and can generate traditional C# events when log events pass through `NetEventSink`.

You listen to events by attaching to `IJ4JLogger`'s `LogEvent`:

```csharp
if( _logger != null )
    _logger.LogEvent += DisplayLogEventAsync;
```
