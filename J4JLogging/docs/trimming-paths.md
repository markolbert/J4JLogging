# Trimming Source Code File Paths

The README example uses two static methods, `FilePathTrimmer` and `GetProjectPath`, which
at first glance don't appear to do anything. In reality, what they do is determine the path to the
project's root which, in turn, is used to trim the source code file paths so that they don't get
"too long".

Those two static methods can be incorporated in any project, as is, and used as they are in this
example:

```csharp
public static class SourceCodeFilePathModifiers
{
    // copy these next two methods to the source code file where you configure J4JLogger
    // and then reference FilePathTrimmer as the context converter you
    // want to use
    private static string FilePathTrimmer(
        Type? loggedType,
        string callerName,
        int lineNum,
        string srcFilePath)
    {
        return CallingContextEnricher.DefaultFilePathTrimmer(loggedType,
            callerName,
            lineNum,
            CallingContextEnricher.RemoveProjectPath(srcFilePath, GetProjectPath()));
    }

    private static string GetProjectPath([CallerFilePath] string filePath = "")
    {
        var dirInfo = new DirectoryInfo(Path.GetDirectoryName(filePath)!);

        while (dirInfo.Parent != null)
        {
            if (dirInfo.EnumerateFiles("*.csproj").Any())
                break;

            dirInfo = dirInfo.Parent;
        }

        return dirInfo.FullName;
    }
}
```

Unfortunately, accessing the methods contained in the `J4JLogger` library, even if they were public, won't work. That's because they'd pick up the path to wherever the library was stored rather than the project in which you're using `J4JLogger`.
