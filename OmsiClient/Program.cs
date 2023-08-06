using System.Text;
using OmsiClient.App.Services;
using OmsiClient.App.Services.Configuration;
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
        
        //create builder
        var app = builder.Build();
        
        //Background Services
        _ = app.Services.GetRequiredService<SessionManager>();
        
        await app.RunAsync();
    }
}