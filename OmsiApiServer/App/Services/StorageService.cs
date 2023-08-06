using DiscordLogging.App.Services;

namespace OmsiApiServer.App.Services;

public class StorageService
{
    public StorageService()
    {
        EnsureCreated();
    }
    
    public void EnsureCreated()
    {
        Directory.CreateDirectory(PathBuilder.Dir("storage", "configs"));
        Directory.CreateDirectory(PathBuilder.Dir("storage", "backup"));
    }
}