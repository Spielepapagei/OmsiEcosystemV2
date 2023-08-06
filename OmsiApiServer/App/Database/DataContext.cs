using DiscordLogging.App.Database.Models;
using Microsoft.EntityFrameworkCore;
using OmsiApiServer.App.Services;

namespace OmsiApiServer.App.Database;

public class DataContext : DbContext
{
    private readonly ConfigService ConfigService;
    
    public DataContext(ConfigService configService)
    {
        ConfigService = configService;
    }
    
    //Data Context
    public DbSet<User> User { get; set; }
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = ConfigService
                .GetSection("OmsiApi")
                .GetSection("Database");

            var connectionString = $"host={config.GetValue<string>("Host")};" +
                                   $"port={config.GetValue<int>("Port")};" +
                                   $"database={config.GetValue<string>("Database")};" +
                                   $"uid={config.GetValue<string>("Username")};" +
                                   $"pwd={config.GetValue<string>("Password")}";
            
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.Parse("5.7.37-mysql"),
                builder =>
                {
                    builder.EnableRetryOnFailure(5);
                }
            );
        }
    }
}