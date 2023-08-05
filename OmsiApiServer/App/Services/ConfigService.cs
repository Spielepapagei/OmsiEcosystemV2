using System.Text;
using DiscordLogging.App.Services;
using Logging.Net;
using Microsoft.Extensions.Primitives;

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
            Logger.Debug("Debug mode enabled");
    }

    public void Reload()
    {
        Logger.Info($"Reading config from '{PathBuilder.File("storage", "configs", "config.json")}'");
        
        Configuration = new ConfigurationBuilder().AddJsonStream(
            new MemoryStream(Encoding.ASCII.GetBytes(
                    File.ReadAllText(
                        PathBuilder.File("storage", "configs", "config.json")
                    )
                )
            )).Build();

        Logger.Info("Reloaded configuration file");
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