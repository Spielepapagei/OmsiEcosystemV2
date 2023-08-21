using System.Text;
using OmsiClient.App.Helpers;
using OmsiClient.App.Services;
using OmsiClient.App.Services.Configuration;
using OmsiClient.App.Services.OmsiDataClient;
using OmsiClient.App.Services.OmsiDataClient.Modules;
using OmsiClient.App.Services.UI;
using Spectre.Console;

namespace OmsiClient;

public class Program
{
    public static readonly Rule AppRule = new Rule("[orange3]OmsiApiClient[/]")
        .LeftJustified()
        .RuleStyle("silver dim");
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        
        AnsiConsole.Write(AppRule);
        
        //var configService = new ConfigService(new StorageService());
        
        var builder = WebApplication.CreateBuilder(args);
        
        //Services
        builder.Services.AddSingleton<ConfigService>();
        builder.Services.AddSingleton<StorageService>();
        builder.Services.AddSingleton<SessionManager>();
        builder.Services.AddSingleton<MainMenuService>();
        builder.Services.AddSingleton<OmsiDataService>();
        
        //Helpers
        builder.Services.AddSingleton<PingHelper>();
        
        //create builder
        var app = builder.Build();
        
        //Background Services
        _ = app.Services.GetRequiredService<SessionManager>();
        _ = app.Services.GetRequiredService<OmsiDataService>();
        
        await app.RunAsync();
    }
}