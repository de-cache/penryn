using System.CommandLine;

namespace Penryn.Cli;

public abstract class Program
{
    private static int Main(string[] args)
    {
        var rootCommand = new RootCommand("Static site generator with some tricks");

        var projectFolder = new Option<string>("--project", "-p")
            { Description = "Root folder of project", Recursive = true };
        var force = new Option<bool>("--force") { Description = "Force destructive actions", Recursive = true };

        var initCommand = new Command("new", "Create a new Penryn project") { projectFolder, force };
        initCommand.Arguments.Add(new Argument<string>("project-name")
        {
            Description = "Name of the project and its containing directory",
            DefaultValueFactory = (_ => "project")
        });
        initCommand.SetAction(result => Create.CreateProject(result.GetValue(projectFolder), result.GetValue(force)));
        
        rootCommand.Subcommands.Add(initCommand);
        rootCommand.Options.Add(projectFolder);
        rootCommand.Options.Add(force);

        return rootCommand.Parse(args).Invoke();
    }
}