using Newtonsoft.Json;
using OmsiClient.App.Helpers;

namespace OmsiClient.App.Services.Configuration;

public class ConfigService
{
    private readonly StorageService StorageService;
    private readonly string Path;
    private ConfigV1 Configuration;
    
    public bool DebugMode { get; private set; } = false;

    public ConfigService(StorageService storageService)
    {
        StorageService = storageService;
        StorageService.EnsureCreated();
        
        Path = PathBuilder.File("storage", "configs", "config.json");

        Reload();
    }

    public void Reload()
    {
        if (!File.Exists(Path))
        {
            File.WriteAllText(Path, "{}");
        }

        Configuration = JsonConvert.DeserializeObject<ConfigV1>(
            File.ReadAllText(Path)
        ) ?? new ConfigV1();
        
        File.WriteAllText(Path, JsonConvert.SerializeObject(Configuration, Formatting.Indented));
    }

    public void Save(ConfigV1 configV1)
    {
        Configuration = configV1;
        Save();
    }

    public void Save()
    {
        if (!File.Exists(Path))
        {
            File.WriteAllText(Path, "{}");
        }
        
        File.WriteAllText(Path, JsonConvert.SerializeObject(Configuration, Formatting.Indented));
        
        Reload();
    }

    public ConfigV1 Get()
    {
        return Configuration;
    }
}