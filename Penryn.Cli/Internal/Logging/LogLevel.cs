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
