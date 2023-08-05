using System.Text;
using OmsiApiClient.App.Services;
using Spectre.Console;

namespace OmsiApiClient;

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
        
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        //Services
        builder.Services.AddSingleton<SessionManager>();
        
        //create builder
        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        //Background Services
        _ = app.Services.GetRequiredService<SessionManager>();
        
        //Start app
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        await app.RunAsync();
    }
}
