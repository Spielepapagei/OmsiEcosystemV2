using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OmsiApiServer.App.Database;
using OmsiApiServer.App.Services;
using OmsiApiServer.App.Services.Auth;
using OmsiApiServer.App.Services.Configuration;
using Spectre.Console;
using Swashbuckle.AspNetCore.Filters;

namespace OmsiApiServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        //Logger.UsedLogger = new CacheLogger();
        
        //TODO: Overall Logging
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        
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
        builder.Services.AddSwaggerGen(x =>
        {
            x.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
            {
                Description = "Standard Authorization header using the Bearer scheme, e.g \"bearer {token} \"",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            
            x.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        
        //Jwt
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                        .GetBytes(configService.Get().OmsiClient.Security.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        
        // Databases
        builder.Services.AddDbContext<DataContext>();
        
        // Repositories
        builder.Services.AddScoped(typeof(Repository<>));
        
        // Services
        builder.Services.AddSingleton<ConfigService>();
        builder.Services.AddSingleton<StorageService>();
        builder.Services.AddSingleton<DateTimeService>();
        builder.Services.AddScoped<IdentityService>();
        builder.Services.AddScoped<SessionManagerService>();
        
        // Background services
        

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        //AutoStartServices
        //_ = app.Services.GetRequiredService<DiscordBotService>();
        
        await app.RunAsync();
    }
}