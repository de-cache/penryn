using System.CommandLine;
using Penryn.Cli.Commands;
using Penryn.Cli.Internal;
using Penryn.Core;

namespace Penryn.Cli;

public abstract class Program
{
    private static int Main(string[] args)
    {
        var rootCommand = new RootCommand("Static site generator with some tricks");

        var projectFolderOption = new Option<string>("--project", "-p")
            { Description = "Root folder of project", Recursive = true };
        var forceOption = new Option<bool>("--force") { Description = "Force destructive actions", Recursive = true };

        var projectName = new Argument<string>("project-name")
        {
            Description = "Name of the project and its containing directory",
            DefaultValueFactory = (_ => Constants.DefaultProjectName)
        };
        var templateOption = new Option<TemplateOptions>("--template", "-t")
        {
            Description = "Template to use when creating a new project",
            DefaultValueFactory = (_ => TemplateOptions.Basic)
        };
        var initCommand = new Command("create", "Create a new Penryn project")
            { projectName, projectFolderOption, templateOption };
        initCommand.SetAction(result =>
            Create.CreateProject(result.GetValue(projectName), result.GetValue(forceOption),
                result.GetValue(templateOption)));

        var buildCommand = new Command("build", "Build the project");
        buildCommand.SetAction(result => Build.BuildProject(result.GetValue(projectFolderOption)));

        // currently we're ordering the list manually, but you can hook into the help command to do this automatically
        rootCommand.Subcommands.Add(buildCommand);
        rootCommand.Subcommands.Add(initCommand);
        rootCommand.Options.Add(forceOption);
        rootCommand.Options.Add(projectFolderOption);

        return rootCommand.Parse(args).Invoke();
    }
}