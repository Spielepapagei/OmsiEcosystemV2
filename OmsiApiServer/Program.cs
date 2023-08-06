using OmsiApiServer.App.Database;
using OmsiApiServer.App.Services;
using Spectre.Console;

namespace OmsiApiServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        //Logger.UsedLogger = new CacheLogger();
        
        //TODO: Overall Logging
        
        var path = new TextPath(Directory.GetCurrentDirectory());

        path.RootStyle = new Style(foreground: Color.Blue);
        path.SeparatorStyle = new Style(foreground: Color.Blue);
        path.StemStyle = new Style(foreground: Color.Green);
        path.LeafStyle = new Style(foreground: Color.Green);
        
        AnsiConsole.Markup($"[green]Working dir[/]: ");
        AnsiConsole.Write(path);

        AnsiConsole.MarkupLine("[orange3]Running pre-init tasks[/]");

        // This will also copy all default config files
        var configService = new ConfigService(new StorageService());
        var databaseCheckupService = new DatabaseCheckupService(configService);

        await databaseCheckupService.Perform();
        
        
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // Databases
        builder.Services.AddDbContext<DataContext>();
        
        // Repositories
        builder.Services.AddScoped(typeof(Repository<>));
        
        // Services
        builder.Services.AddSingleton<ConfigService>();
        builder.Services.AddSingleton<StorageService>();
        builder.Services.AddSingleton<DateTimeService>();
        
        
        // Background services
        

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        
        app.UseAuthorization();
        
        app.MapControllers();
        
        //AutoStartServices
        //_ = app.Services.GetRequiredService<DiscordBotService>();
        
        await app.RunAsync();
    }
}