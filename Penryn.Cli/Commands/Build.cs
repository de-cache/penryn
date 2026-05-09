using System.Security;
using Penryn.Core;

namespace Penryn.Cli.Commands;

public static class Build
{
    public static void BuildProject(string? projectFolder)
    {
        projectFolder ??= Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(projectFolder);
        }
        catch (IOException e)
        {
            Console.WriteLine($"Failed to open project: {e.Message}"); // err
            return;
        }
        catch (SecurityException e)
        {
            Console.WriteLine($"Unable to open project: {e.Message}"); // err
            return;
        }

        if (!File.Exists("penryn.json"))
        {
            Console.WriteLine("Folder does not contain a Penryn project or is misconfigured"); // err
            return;
        }

        var rawConfig = File.ReadAllText(Constants.ConfigFileName);
        if (string.IsNullOrWhiteSpace(rawConfig))
        {
            Console.WriteLine("penryn.json is empty");
            return;
        }

        var config = new Config();
        config.Deserialize(rawConfig);

        var definition = config.BuildOptions;

        Parser parser = new(config);
        try
        {
            parser.SetFileProvider(Path.Combine(Directory.GetCurrentDirectory(), definition.TemplateFolder));
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Templates folder does not exist"); // err
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to open project: {e.Message}"); // err
            return;
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
        Console.WriteLine($"Copying static files"); // verbose
        foreach (var staticFile in staticFiles)
        {
            var fileName = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), definition.StaticFolder),
                staticFile);
            var fileAttr = File.GetAttributes(staticFile);
            if (fileAttr.HasFlag(FileAttributes.Hidden)) continue;
            if (fileAttr.HasFlag(FileAttributes.Directory))
            {
                Directory.CreateDirectory(Path.Combine(definition.OutputFolder, fileName));
            }
            else
            {
                File.Copy(staticFile, Path.Combine(definition.OutputFolder, fileName));
            }

            Console.WriteLine(Path.Combine(definition.OutputFolder, fileName));
        }

        // parse template files
        Console.WriteLine($"Parsing templates");
        if (Directory.Exists(definition.TemplateFolder))
        {
            if (File.Exists(Path.Combine(definition.TemplateFolder, definition.BaseTemplateFile)))
            {
                var res = parser.ParseFile(Path.Combine(definition.TemplateFolder, definition.BaseTemplateFile));
                var file = new StreamWriter(Path.Combine(definition.OutputFolder, "index.html"));
                Console.WriteLine(Path.Combine(definition.OutputFolder, "index.html")); // verbose
                file.Write(res);
                file.Close();
            }
            else
            {
                Console.WriteLine("Could not find baseof.liquid, please create a new template file"); // err
                return;
            }
        }
        else
        {
            Console.WriteLine("No templates found"); // err
            return;
        }

        Console.WriteLine("Built successfully"); // normal
    }
}