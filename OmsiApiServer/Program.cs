using System.Text;
using OmsiApiServer.App.Services;

namespace OmsiApiServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        //Services
        builder.Services.AddSingleton<SessionManagerService>();
        
        //create builder
        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        //Start app
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        await app.RunAsync();
    }
}