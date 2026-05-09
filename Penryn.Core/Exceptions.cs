using Parlot;

namespace Penryn.Core;

/// <summary>
/// Exception thrown when the configuration file is invalid.
/// </summary>
[Serializable]
internal class InvalidConfigException : Exception
{
    public InvalidConfigException()
    {
    }

    public InvalidConfigException(string message) : base(message)
    {
    }

    public InvalidConfigException(string message, Exception inner) : base(message, inner)
    {
    }
}

/// <summary>
/// Exception thrown when the Penryn parser encounters an error.
/// </summary>
internal class PenrynParserException : Exception
{
    public PenrynParserException()
    {
    }
    
    public PenrynParserException(string message) : base(message)
    {
    }
    
    public PenrynParserException(string message, Exception inner) : base(message, inner)
    {
    }
}