namespace J4JSoftware.Logging
{
    // distinguishes between allowable locations for log files
    public enum LogFileLocation
    {
        // store the log file in the user's AppData folder
        AppData,

        // store the log file in the application's exe folder
        ExeFolder,

        // store the log file at an absolute path
        Absolute
    }
}