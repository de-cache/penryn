namespace Penryn.Cli.Internal.Logging;

/// <summary>
/// Class to manage logging in CLI
/// </summary>
public static class Logger
{
    public static LogLevel Level { get; set; } = LogLevel.Info;

    private static TextWriter Output { get; set; } = Console.Out;

    private static TextWriter ErrorOutput { get; set; } = Console.Error;

    /// <summary>
    /// Check whether the log level can be shown as output.
    /// </summary>
    /// <param name="level">The LogLevel to check.</param>
    /// <returns>Whether the level is enabled or not.</returns>
    private static bool IsEnabled(LogLevel level)
    {
        if (Level == LogLevel.Silent || level == LogLevel.Silent) return false;

        return level <= Level;
    }

    /// <summary>
    /// Write a line to the console.
    /// </summary>
    /// <param name="message">Message to write./</param>
    /// <param name="level">Log level to use.</param>
    private static void WriteLine(string message, LogLevel level = LogLevel.Info)
    {
        if (!IsEnabled(level)) return;

        var writer = level is LogLevel.Error or LogLevel.Warning ? ErrorOutput : Output;
        writer.WriteLine(message);
    }

    public static void LogError(string message) => WriteLine(message, LogLevel.Error);

    public static void LogWarning(string message) => WriteLine(message, LogLevel.Warning);

    public static void LogInfo(string message) => WriteLine(message);

    public static void LogVerbose(string message) => WriteLine(message, LogLevel.Verbose);

    public static void LogDebug(string message) => WriteLine(message, LogLevel.Debug);
}