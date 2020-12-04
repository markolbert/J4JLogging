### Usage

`IJ4JLogger` instances provide a method, `SetLoggedType()`, that isn't 
required to be used but which are required if you want to include information
about the type of object generating logging information. As that's one of
the main points of using `IJ4JLogger` rather than just Serilog's ILogger
you'll almost always be calling `SetLoggedType()`, usually in the constructor
of whatever types you're logging.

Using **IJ4JLogger* is very similar to using Serilog since it's essentially 
just a wrapper around an `ILogger` instance. You can use any of Serilog's 
basic logging method calls, `Write()`, `Debug()`, `Error()`, `Information()`, 
`Verbose()` and `Warning()`.

Each method has overloaded variants accepting different arguments. The layout of these 
is identical for all the methods except Write:

```csharp
public interface IJ4JLogger<out TCalling>
{
    void Debug(
        string template,
        [ CallerMemberName ] string memberName = "",
        [ CallerFilePath ] string srcPath = "",
        [ CallerLineNumber ] int srcLine = 0
    );

    void Debug<T0>(
        string template,
        T0 propertyValue,
        [ CallerMemberName ] string memberName = "",
        [ CallerFilePath ] string srcPath = "",
        [ CallerLineNumber ] int srcLine = 0
    );

    void Debug<T0, T1>(
        string template,
        T0 propertyValue0,
        T1 propertyValue1,
        [ CallerMemberName ] string memberName = "",
        [ CallerFilePath ] string srcPath = "",
        [ CallerLineNumber ] int srcLine = 0
    );

    void Debug<T0, T1, T2>(
        string template,
        T0 propertyValue0,
        T1 propertyValue1,
        T2 propertyValue2,
        [ CallerMemberName ] string memberName = "",
        [ CallerFilePath ] string srcPath = "",
        [ CallerLineNumber ] int srcLine = 0
    );

    void Debug(
        string template,
        object[] propertyValues,
        [ CallerMemberName ] string memberName = "",
        [ CallerFilePath ] string srcPath = "",
        [ CallerLineNumber ] int srcLine = 0
    );
```

The `Write()` methods are similar, except they each start off with a Serilog 
`LogEventLevel` argument which specifies the log event level.

If you pass a simple string (i.e., a value for the template argument) to the methods you **must** specify the types of 
the propertyValue arguments explicitly in the method call. 

An example:

```csharp
string someStringValue = "abcd";
_logger.Debug<string>("The value of that argument is {someIntValue}", someStringValue);
```
This requirement comes about because the `memberName`, `srcPath` and `srcLine` 
arguments are automagically set for you by the compiler. The fact the 
`memberName` and `srcPath` arguments of the logging methods are strings and
"collide" string arguments you may specify. That makes explict type 
specifications for the arguments necessary when strings are referenced by the message template.