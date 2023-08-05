using System.Text;
using Spectre.Console;

namespace OmsiApiClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        System.Console.OutputEncoding = Encoding.UTF8;
        System.Console.InputEncoding = Encoding.UTF8;

        WebApplicationBuilder builder;
        WebApplication app = null;

        var rule = new Rule("[orange3]OmsiApiClient[/]")
            .RuleStyle("silver dim");
        AnsiConsole.Write(rule);
        
        await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Dots2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Checkup", async ctx => 
            {
                AnsiConsole.MarkupLine("Initializing");
                builder = WebApplication.CreateBuilder(args);
                
                AnsiConsole.MarkupLine("Initializing Controllers");
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                
                AnsiConsole.MarkupLine("Initializing Swagger");
                builder.Services.AddSwaggerGen();
                
                AnsiConsole.MarkupLine("Loading Services");
                //builder.Services.AddSingleton<DiscordBotService>();
                
                AnsiConsole.MarkupLine("Starting Up now!");
                app = builder.Build();
                
                if (app.Environment.IsDevelopment())
                {
                    AnsiConsole.MarkupLine("Starting Swagger");
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
        
                AnsiConsole.MarkupLine("Starting Background Services");
                //_ = app.Services.GetRequiredService<DiscordBotService>();
                
                AnsiConsole.MarkupLine("Finishing up");
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                
            });
        
        if(app == null) Environment.Exit(500);
        
        
        await app.RunAsync();
    }
}
