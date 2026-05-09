using System.Text.Json;
using System.Text.Json.Serialization;

namespace Penryn.Core;

/// <summary>
/// Class that represents the configuration of a project.
/// </summary>
public class Config
{
    /// <summary>
    /// The name of the project, embeddable across the project.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    /// <summary>
    /// A string that briefly describes the project.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    /// <summary>
    /// The language information of the project.
    /// </summary>
    /// <value>The default is the current culture of the operating system.</value>
    [JsonPropertyName("language")]
    public string Language { get; set; } =
        "en"; // culture keeps locking itself on "iv" when doing it right, fuck my i18n life

    /// <summary>
    /// The build options of the project, such as what folders are used for discovering and outputting files.
    /// </summary>
    [JsonPropertyName("buildOptions")]
    public BuildDefinition BuildOptions { get; set; } = new();

    /// <summary>
    /// Initializes and returns a default project configuration.
    /// </summary>
    /// <param name="projectName">The name of the project. If not set, defaults to <c>penryn</c>.</param>
    /// <returns>
    /// A new instance of the <see cref="Config"/> class with pre-defined values.
    /// </returns>
    public static Config DefaultConfig(string projectName = Constants.DefaultProjectName) => new()
    {
        Name = projectName
    };

    /// <summary>
    /// Transforms the current object into a serialized JSON string.
    /// </summary>
    /// <returns>A string containing the JSON representation of the <see cref="Config"/> object.</returns>
    /// <exception cref="JsonException">Thrown when an unexpected error occurs while serializing the <see cref="Config"/>
    /// object.</exception>
    public string Serialize()
    {
        try
        {
            return JsonSerializer.Serialize((object?)this, ConfigGenerationContext.Default.Config);
        }
        catch (NotSupportedException e)
        {
            throw new JsonException($"Unexpected error while serializing: {e.Message}");
        }
        catch (InvalidOperationException e)
        {
            throw new JsonException($"Unexpected error while serializing: {e.Message}");
        }
    }

    /// <summary>
    /// Translates the passed JSON string into an object of type <see cref="Config"/>.
    /// </summary>
    /// <param name="json">A JSON string representing the object</param>
    /// <exception cref="InvalidConfigException">Occurs when the configuration is invalid</exception>
    /// <exception cref="JsonException">Occurs when the JSON is unable to be parsed successfully</exception>
    public void Deserialize(string json)
    {
        Config? newObj;
        try
        {
            newObj = JsonSerializer.Deserialize<Config>(json, ConfigGenerationContext.Default.Config);
        }
        catch (JsonException e)
        {
            throw new JsonException($"Failed to deserialize config: {e.Message}");
        }
        catch (ArgumentNullException)
        {
            throw new JsonException("Passed JSON is null");
        }
        catch (Exception e)
        {
            throw new JsonException($"Unexpected error while deserializing: {e.Message}");
        }

        if (newObj != null)
        {
            Name = newObj.Name;
            Description = newObj.Description;
            Language = newObj.Language;
        }
        else
        {
            throw new InvalidConfigException("Failed to parse config");
        }
    }

    public override string ToString()
    {
        return Name;
    }
}

/// <summary>
/// Used for serialization code generation when compiling as an AOT executable.
/// </summary>
[JsonSourceGenerationOptions(
    ReferenceHandler = JsonKnownReferenceHandler.IgnoreCycles,
    WriteIndented = true)]
[JsonSerializable(typeof(Config))]
internal partial class ConfigGenerationContext : JsonSerializerContext;