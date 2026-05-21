namespace Penryn.Cli.Internal.Enums;

/// <summary>
/// Return codes to be used in commands.
/// </summary>
public enum ReturnCodes
{
    /// <summary>
    /// The command has returned successfully.
    /// </summary>
    Success,

    /// <summary>
    /// An unknown error has occurred during the command, typically an undetermined exception.
    /// </summary>
    UnknownError,

    /// <summary>
    /// A file or directory could not be accessed.
    /// </summary>
    FileAccessError,

    /// <summary>
    /// A file or directory could not be accessed or modified due to system permission errors/
    /// </summary>
    FilePermissionError,

    /// <summary>
    /// The project configuration is invalid.
    /// </summary>
    BadConfigFile,

    /// <summary>
    /// The project configuration file is missing.
    /// </summary>
    MissingConfigFile,

    /// <summary>
    /// The project configuration file is empty.
    /// </summary>
    EmptyConfigFile,

    /// <summary>
    /// An error involving the build definition of a project.
    /// </summary>
    BuildSettingsError,

    /// <summary>
    /// The configured base template file (baseof.liquid default) could not be found.
    /// </summary>
    BaseTemplateNotFound,

    /// <summary>
    /// The template file could not be properly parsed or rendered.
    /// </summary>
    TemplateRenderError,

    /// <summary>
    /// The project configuration file could not be instantiated while creating a new project.
    /// </summary>
    ConfigInstantiationError,

    /// <summary>
    /// Failed to copy template files to new project.
    /// </summary>
    TemplateCopyError
}