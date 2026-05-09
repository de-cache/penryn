namespace Penryn.Core;

/// <summary>
/// Represents the build definition for a project, such as what folders to use for discovering and outputting files.
/// </summary>
/// <param name="OutputFolder">Where a project's build artifacts are created.</param>
/// <param name="TemplateFolder">Where a project's templates are stored.</param>
/// <param name="StaticFolder">Where a project's static files are stored.</param>
/// <param name="ContentFolder">Where a project's content (e.g., blog posts) is stored for insertion.</param>
public record BuildDefinition(
    string OutputFolder = "public",
    string TemplateFolder = "templates",
    string StaticFolder = "static",
    string ContentFolder = "content")
{
    public string OutputFolder { get; set; } = OutputFolder;
    public string TemplateFolder { get; set; } = TemplateFolder;
    public string StaticFolder { get; set; } = StaticFolder;
    public string ContentFolder { get; set; } = ContentFolder;
}