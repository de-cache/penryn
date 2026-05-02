using System.Security;
using Penryn.Core;

namespace Penryn.Cli;

public static class Create
{
    public static void CreateProject(string? directory, bool force = false)
    {
        directory ??= Path.Combine(Directory.GetCurrentDirectory(), "project");

        directory = Path.GetFullPath(directory);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        Console.WriteLine($"Creating project in {directory}"); // todo: replace with logging solution

        try
        {
            Directory.SetCurrentDirectory(directory);
        }
        catch (IOException e)
        {
            Console.WriteLine($"Failed to set current directory to {directory}: {e.Message}"); // err
            return;
        }

        if (File.Exists(Path.Combine(directory, "penryn.json")) && !force)
        {
            Console.WriteLine("penryn.json already exists, skipping... (use --force to overwrite)"); // err
        }
        else
        {
            try
            {
                var file = new StreamWriter(Path.Combine(directory, "penryn.json"));
                var defaultConf = Config.DefaultConfig().Serialize();
                file.Write(defaultConf);
                file.Close();
                Directory.CreateDirectory(Path.Combine(directory,
                    "src")); // maybe make some example files here later
                Console.WriteLine("Project created!");
            }
            catch (SecurityException e)
            {
                Console.WriteLine($"Failed to create penryn.json: {e.Message}");
            }
            catch (IOException e)
            {
                Console.WriteLine($"Failed to create penryn.json: {e.Message}"); // err
            }
        }
    }
}