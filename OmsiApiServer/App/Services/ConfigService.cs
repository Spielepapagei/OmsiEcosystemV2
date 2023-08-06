using System.Text;
using DiscordLogging.App.Services;
using Microsoft.Extensions.Primitives;
using Spectre.Console;

namespace OmsiApiServer.App.Services;

public class ConfigService : IConfiguration
{
    private readonly StorageService StorageService;

    private IConfiguration Configuration;

    public bool DebugMode { get; private set; } = false;

    public ConfigService(StorageService storageService)
    {
        StorageService = storageService;
        StorageService.EnsureCreated();

        Reload();

        // Env vars
        var debugVar = Environment.GetEnvironmentVariable("APP_DEBUG");

        if (debugVar != null)
            DebugMode = bool.Parse(debugVar);

        if (DebugMode)
            AnsiConsole.MarkupLine("[orange3]Debug mode enabled[/]");
    }

    public void Reload()
    {
        var path = new TextPath(PathBuilder.File("storage", "configs", "config.json"));

        path.RootStyle = new Style(foreground: Color.Blue);
        path.SeparatorStyle = new Style(foreground: Color.Blue);
        path.StemStyle = new Style(foreground: Color.Green);
        path.LeafStyle = new Style(foreground: Color.Green);
        
        AnsiConsole.Markup($"[green]Reading config from[/]: ");
        AnsiConsole.Write(path);
        
        Configuration = new ConfigurationBuilder().AddJsonStream(
            new MemoryStream(Encoding.ASCII.GetBytes(
                    File.ReadAllText(
                        PathBuilder.File("storage", "configs", "config.json")
                    )
                )
            )).Build();

        AnsiConsole.MarkupLine("[green]Reloaded configuration file.[/]");
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return Configuration.GetChildren();
    }

    public IChangeToken GetReloadToken()
    {
        return Configuration.GetReloadToken();
    }

    public IConfigurationSection GetSection(string key)
    {
        return Configuration.GetSection(key);
    }

    public string this[string key]
    {
        get => Configuration[key];
        set => Configuration[key] = value;
    }
}