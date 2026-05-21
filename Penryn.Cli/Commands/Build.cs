using System.Security;
using System.Text.Json;
using Penryn.Cli.Internal.Enums;
using Penryn.Cli.Internal.Logging;
using Penryn.Core;

namespace Penryn.Cli.Commands;

public static class Build
{
    public static int BuildProject(string? projectFolder)
    {
        projectFolder ??= Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(projectFolder);
        }
        catch (IOException e)
        {
            Logger.LogError($"Failed to open project: {e.Message}");
            return (int)ReturnCodes.FileAccessError;
        }
        catch (SecurityException e)
        {
            Logger.LogError($"Unable to open project: {e.Message}");
            return (int)ReturnCodes.FilePermissionError;
        }
        catch (Exception e)
        {
            Logger.LogError($"Unexpected errors while opening project: {e.Message}");
            return (int)ReturnCodes.UnknownError;
        }

        if (!File.Exists("penryn.json"))
        {
            Logger.LogError("Folder does not contain a Penryn project or is misconfigured");
            return (int)ReturnCodes.MissingConfigFile;
        }

        var rawConfig = File.ReadAllText(Constants.ConfigFileName);
        if (string.IsNullOrWhiteSpace(rawConfig))
        {
            Logger.LogError("penryn.json is empty");
            return (int)ReturnCodes.EmptyConfigFile;
        }

        var config = new Config();
        try
        {
            config.Deserialize(rawConfig);
        }
        catch (InvalidConfigException e)
        {
            Logger.LogError($"Invalid config file: {e.Message}");
            return (int)ReturnCodes.BadConfigFile;
        }
        catch (JsonException e)
        {
            Logger.LogError($"Invalid config file: {e.Message}");
            return (int)ReturnCodes.BadConfigFile;
        }

        var definition = config.BuildOptions;

        Parser parser = new(config);
        try
        {
            parser.SetFileProvider(Path.Combine(Directory.GetCurrentDirectory(), definition.TemplateFolder));
        }
        catch (DirectoryNotFoundException)
        {
            Logger.LogError("Templates folder does not exist");
            return (int)ReturnCodes.BuildSettingsError;
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to open project: {e.Message}");
            return (int)ReturnCodes.UnknownError;
        }

        if (Directory.Exists(definition.OutputFolder))
        {
            Directory.Delete(definition.OutputFolder, true);
            Directory.CreateDirectory(definition.OutputFolder);
        }
        else Directory.CreateDirectory(definition.OutputFolder);

        // copy static files
        var staticFiles =
            Directory.EnumerateFileSystemEntries(Path.Combine(Directory.GetCurrentDirectory(), definition.StaticFolder),
                "*", SearchOption.AllDirectories);
        Logger.LogVerbose("Copying static files");
        foreach (var staticFile in staticFiles)
        {
            var fileName = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), definition.StaticFolder),
                staticFile);
            var fileAttr = File.GetAttributes(staticFile);
            if (fileAttr.HasFlag(FileAttributes.Hidden)) continue;
            var path = Path.Combine(definition.OutputFolder, fileName);
            if (fileAttr.HasFlag(FileAttributes.Directory))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                File.Copy(staticFile, path);
            }

            Logger.LogDebug(path);
        }

        // parse template files
        Logger.LogVerbose("Parsing templates");
        if (Directory.Exists(definition.TemplateFolder))
        {
            if (File.Exists(Path.Combine(definition.TemplateFolder, definition.BaseTemplateFile)))
            {
                try
                {
                    var res = parser.ParseFile(Path.Combine(definition.TemplateFolder, definition.BaseTemplateFile));
                    var file = new StreamWriter(Path.Combine(definition.OutputFolder, "index.html"));
                    Logger.LogDebug(Path.Combine(definition.OutputFolder, "index.html"));
                    file.Write(res);
                    file.Close();
                }
                catch (FileNotFoundException)
                {
                    Logger.LogError("Could not find template file");
                    return (int)ReturnCodes.FileAccessError;
                }
                catch (PenrynParserException)
                {
                    Logger.LogError($"Unable to parse {Path.Combine(definition.OutputFolder, "index.html")}");
                    return (int)ReturnCodes.TemplateRenderError;
                }
            }
            else
            {
                Logger.LogError("Could not find baseof.liquid, please create a new template file");
                return (int)ReturnCodes.BaseTemplateNotFound;
            }
        }
        else
        {
            Logger.LogError("No templates found");
            return (int)ReturnCodes.FileAccessError;
        }

        Logger.LogInfo("Built successfully");
        return (int)ReturnCodes.Success;
    }
}