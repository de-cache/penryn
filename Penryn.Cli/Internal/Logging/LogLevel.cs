namespace Penryn.Cli.Internal.Logging;

/// <summary>
/// Types of logs to be shown in console output. Sorted from most to least precedence.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Zero output from the terminal.
    /// </summary>
    Silent,

    /// <summary>
    /// Only show critical errors that cannot be recovered from.
    /// </summary>
    Error,

    /// <summary>
    /// Shows general warnings and recoverable errors, including higher precedence.
    /// </summary>
    Warning,

    /// <summary>
    /// Shows general command information, including higher precedence.
    /// </summary>
    Info,

    /// <summary>
    /// Shows additional details of command operation, including higher precedence.
    /// </summary>
    Verbose,

    /// <summary>
    /// Shows all information, including higher precedence. Used during development. 
    /// </summary>
    Debug
}

/// <summary>
/// Methods for log level management
/// </summary>
public static class LogLevels
{
    public static bool TryParse(string? value, out LogLevel level)
    {
        switch (value?.Trim().ToLowerInvariant())
        {
            case "s":
            case "silent":
            case "q":
            case "quiet":
                level = LogLevel.Silent;
                return true;

            case "e":
            case "error":
                level = LogLevel.Error;
                return true;

            case "w":
            case "warn":
            case "warning":
                level = LogLevel.Warning;
                return true;

            case "i":
            case "info":
                level = LogLevel.Info;
                return true;

            case "v":
            case "verbose":
                level = LogLevel.Verbose;
                return true;

            case "d":
            case "debug":
                level = LogLevel.Debug;
                return true;

            default:
                level = LogLevel.Info;
                return false;
        }
    }
}