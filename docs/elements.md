### Specifying Event Elements
The `EventElements` property of `IJ4JLoggerConfiguration` lets you define whether or not
to include source code detail and type/caller detail in the log events. It's a flag `Enum`:
```csharp
[Flags]
public enum EventElements
{
    Type = 1 << 0,
    SourceCode = 1 << 1,

    None = 0,
    All = Type | SourceCode
}
```
When specifying multiple flags in a JSON config file you separate the elements with 
commas:
```json
"Logger": {
    "SourceRootPath": "C:/Programming/J4JLogging/",
    "EventElements" :  "SourceCode, Type", 
    ...
```
