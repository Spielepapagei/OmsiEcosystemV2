namespace DiscordLogging.App.Services;

public class StorageService
{
    public StorageService()
    {
        EnsureCreated();
    }
    
    public void EnsureCreated()
    {
        Directory.CreateDirectory(PathBuilder.Dir("storage", "uploads"));
        Directory.CreateDirectory(PathBuilder.Dir("storage", "configs"));
        Directory.CreateDirectory(PathBuilder.Dir("storage", "resources"));
        Directory.CreateDirectory(PathBuilder.Dir("storage", "backups"));
    }
}