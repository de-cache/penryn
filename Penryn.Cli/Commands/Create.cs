using System.Reflection;
using System.Security;
using Penryn.Cli.Internal;
using Penryn.Core;

namespace Penryn.Cli.Commands;

public static class Create
{
    public static void CreateProject(string? projectName, bool force = false, TemplateOptions template = TemplateOptions.Basic)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), projectName ?? Constants.DefaultProjectName);

        directory = Path.GetFullPath(directory);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        Console.WriteLine($"Creating project in {directory}"); // normal

        try
        {
            Directory.SetCurrentDirectory(directory);
        }
        catch (IOException e)
        {
            Console.WriteLine($"Failed to set current directory to {directory}: {e.Message}"); // err
            return;
        }

        if (File.Exists(Path.Combine(directory, Constants.ConfigFileName)) && !force)
        {
            Console.WriteLine("penryn.json already exists, skipping... (use --force to overwrite)"); // err
        }
        else
        {
            try
            {
                var file = new StreamWriter(Path.Combine(directory, Constants.ConfigFileName));
                var defaultConf = Config.DefaultConfig().Serialize();
                file.Write(defaultConf);
                file.Close();
            }
            catch (SecurityException e)
            {
                Console.WriteLine($"Failed to create penryn.json: {e.Message}"); // err
            }
            catch (IOException e)
            {
                Console.WriteLine($"Failed to create penryn.json: {e.Message}"); // err
            }

            Console.WriteLine("Copying example files"); // debug

            if (template != TemplateOptions.None) return;
            
            // evil block! not a fan
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames =
                assembly.GetManifestResourceNames();
            if (resourceNames.Length == 0)
            {
                Console.WriteLine("Error while copying base template");
                return;
            }
            foreach (var resourceName in resourceNames)
            {
                var fileName = resourceName.Replace("Penryn.Cli.Template.", "")
                    .Replace('.', Path.DirectorySeparatorChar);
                var lastSlash = fileName.LastIndexOf('/');
                if (lastSlash != -1)
                    fileName = string.Concat(fileName.AsSpan(0, lastSlash), ".",
                        fileName.AsSpan(lastSlash + 1));
                Console.WriteLine(fileName);
                var outputPath = Path.Combine(directory, fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                try
                {
                    using var resourceStream = assembly.GetManifestResourceStream(resourceName);
                    if (resourceStream == null) continue;
                    using var fileStream = File.Create(outputPath);
                    resourceStream.CopyTo(fileStream);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to copy {resourceName}: {e.Message}"); // err
                }
            }
        }
    }
}