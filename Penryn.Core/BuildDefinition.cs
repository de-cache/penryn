using System.Text.Json.Serialization;

namespace Penryn.Core;

/// <summary>
/// Represents the build definition for a project, such as what folders to use for discovering and outputting files.
/// </summary>
/// <param name="OutputFolder">Where a project's build artifacts are created.</param>
/// <param name="TemplateFolder">Where a project's templates are stored.</param>
/// <param name="StaticFolder">Where a project's static files are stored.</param>
/// <param name="ContentFolder">Where a project's content (e.g., blog posts) is stored for insertion.</param>
/// <param name="BaseTemplateFile">The name of the base template to use for all pages.</param>
public record BuildDefinition(
    string OutputFolder = "public",
    string TemplateFolder = "templates",
    string StaticFolder = "static",
    string ContentFolder = "content",
    string BaseTemplateFile = Constants.BaseTemplateFileName)
{
    [JsonPropertyName("outputFolder")] public string OutputFolder { get; set; } = OutputFolder;
    [JsonPropertyName("templateFolder")] public string TemplateFolder { get; set; } = TemplateFolder;
    [JsonPropertyName("staticFolder")] public string StaticFolder { get; set; } = StaticFolder;
    [JsonPropertyName("contentFolder")] public string ContentFolder { get; set; } = ContentFolder;

    [JsonPropertyName("baseTemplateFile")] public string BaseTemplateFile { get; set; } = BaseTemplateFile;
}