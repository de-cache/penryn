using System.Security;
using Fluid;
using Microsoft.Extensions.FileProviders;

namespace Penryn.Core;

public class Parser
{
    private static FluidParser _fluidParser = new();
    private readonly TemplateContext _context;

    public Parser(Config config, FluidParserOptions? parserOptions = null, TemplateOptions? templateOptions = null)
    {
        if (parserOptions != null)
        {
            _fluidParser = new FluidParser(parserOptions);
        }

        templateOptions ??= new TemplateOptions
        {
            MemberAccessStrategy =
            {
                IgnoreCasing = true
            }
        };

        templateOptions.MemberAccessStrategy.Register<Config>();
        
        _fluidParser = new FluidParser();
        _context = new TemplateContext(templateOptions);

        // project globals
        _context.SetValue("project", config);
    }

    /// <summary>
    /// Sets the file provider for the parser, based on a given directory.
    /// </summary>
    /// <param name="folder">Directory path</param>
    public void SetFileProvider(string folder)
    {
        try
        {
            _context.Options.FileProvider = new PhysicalFileProvider(folder);
        }
        catch (DirectoryNotFoundException e)
        {
            throw new DirectoryNotFoundException(e.Message);
        }
        catch (FileNotFoundException e)
        {
            throw new FileNotFoundException(e.Message, folder);
        }
        catch (IOException e)
        {
            throw new IOException(e.Message, e);
        }
        catch (SecurityException e)
        {
            throw new SecurityException(e.Message, e);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    /// <summary>
    /// Parse a given file
    /// </summary>
    /// <param name="path"></param>
    /// <returns>The rendered output of a given file path</returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="PenrynParserException"></exception>
    public string ParseFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found", path);
        }

        string contents;
        try
        {
            contents = File.ReadAllText(path);
        }
        catch (PathTooLongException)
        {
            throw new PenrynParserException("File path is too long");
        }
        catch (SecurityException e)
        {
            throw new PenrynParserException($"Unable to access file: {e.Message}");
        }
        catch (IOException e)
        {
            throw new PenrynParserException($"Unable to read file: {e.Message}");
        }

        return _fluidParser.Parse(contents).Render(_context);
    }
}