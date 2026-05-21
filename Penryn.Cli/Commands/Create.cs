using System.Reflection;
using System.Security;
using Penryn.Cli.Internal.Enums;
using Penryn.Cli.Internal.Logging;
using Penryn.Core;

namespace Penryn.Cli.Commands;

public static class Create
{
    public static int CreateProject(string? projectName, bool force = false,
        TemplateOptions template = TemplateOptions.Basic)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), projectName ?? Constants.DefaultProjectName);

        directory = Path.GetFullPath(directory);
        if (!Directory.Exists(directory))
        {
            Logger.LogVerbose($"Directory {directory} does not exist, creating blank folder");
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (IOException e)
            {
                Logger.LogError($"Failed to create project folder: {e.Message}");
                return (int)ReturnCodes.FileAccessError;
            }
            catch (SecurityException e)
            {
                Logger.LogError($"Failed to create project folder: {e.Message}");
                return (int)ReturnCodes.FilePermissionError;
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to create project folder: {e.Message}");
                return (int)ReturnCodes.UnknownError;
            }
        }

        Logger.LogVerbose($"Creating project in {directory}");

        try
        {
            Directory.SetCurrentDirectory(directory);
        }
        catch (IOException e)
        {
            Logger.LogError($"Failed to set current directory to {directory}: {e.Message}");
            return (int)ReturnCodes.FileAccessError;
        }
        catch (SecurityException e)
        {
            Logger.LogError($"Failed to set current directory to {directory}: {e.Message}");
            return (int)ReturnCodes.FilePermissionError;
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to create project in {directory}: {e.Message}");
            return (int)ReturnCodes.UnknownError;
        }

        if (File.Exists(Path.Combine(directory, Constants.ConfigFileName)) && !force)
        {
            Logger.LogWarning("penryn.json already exists in folder, skipping... (use --force to overwrite)");
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
                Logger.LogError($"Failed to create penryn.json: {e.Message}");
                return (int)ReturnCodes.ConfigInstantiationError;
            }
            catch (IOException e)
            {
                Logger.LogError($"Failed to create penryn.json: {e.Message}");
                return (int)ReturnCodes.ConfigInstantiationError;
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to create penryn.json: {e.Message}");
                return (int)ReturnCodes.ConfigInstantiationError;
            }

            Logger.LogVerbose("Copying template files");

            if (template == TemplateOptions.None) return (int)ReturnCodes.Success;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames =
                assembly.GetManifestResourceNames();
            if (resourceNames.Length == 0)
            {
                Logger.LogError("Unable to find template while copying");
                return (int)ReturnCodes.TemplateCopyError;
            }

            // evil block! not a fan
            foreach (var resourceName in resourceNames)
            {
                var fileName = resourceName.Replace("Penryn.Cli.Template.", "")
                    .Replace('.', Path.DirectorySeparatorChar);
                var lastSlash = fileName.LastIndexOf('/');
                if (lastSlash != -1)
                    fileName = string.Concat(fileName.AsSpan(0, lastSlash), ".",
                        fileName.AsSpan(lastSlash + 1));
                Logger.LogDebug(fileName);
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
                    Logger.LogError($"Failed to copy {resourceName}: {e.Message}");
                    return (int)ReturnCodes.TemplateCopyError;
                }
            }
        }

        return (int)ReturnCodes.Success;
    }
}