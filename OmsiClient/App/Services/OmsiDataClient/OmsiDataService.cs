using System.Diagnostics;
using OmsiDataClient.Models;
using Spectre.Console;

namespace OmsiClient.App.Services.OmsiDataClient;

public class OmsiDataService
{

    public OmsiDataService()
    {
        Task.Run(Run);
    }
    
    public readonly OmsiHook.OmsiHook Hook = new();
    private readonly List<BaseModule> Modules = new();
    public List<BusStop> BusStops = new();
    public string Map = new("Empty");

    private string MapName = string.Empty;

    private readonly PeriodicTimer Clock = new(TimeSpan.FromMilliseconds(1000));

    // Add your services here
    private Task RegisterServices()
    {
        //RegisterService<TestService>();
        
        return Task.CompletedTask;
    }
    
    public async Task Run()
    {
        await Hook.AttachToOMSI();
        
        await RegisterServices();
        await Task.Run(ProcessTicks);

        await Hook.OmsiProcess.WaitForExitAsync();
    }
    private async Task ProcessTicks()
    {
        while (!Hook.OmsiProcess.HasExited)
        {
            var mapName = Hook.Globals.Map.Name;
            
            foreach (var service in Modules)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                try
                {
                    await service.Run();

                    if (MapName == string.Empty || MapName != mapName)
                    {
                        Map = mapName;
                        if (Map != "")
                        {
                            await service.MapChange();
                        }
                    }
                }
                catch (NotImplementedException e)
                {
                    continue;
                }
                catch (Exception e)
                {
                    AnsiConsole.MarkupLine($"Service {service.GetType().Name} has thrown an unhandled exception");
                    AnsiConsole.Console.WriteException(e);
                }
                
                stopWatch.Stop();

                if (stopWatch.Elapsed.TotalSeconds > 1)
                {
                    //AnsiConsole.MarkupLine($"Service {service.GetType().Name} took to long to process. Took: {stopWatch.Elapsed.TotalSeconds}s");
                }
            }
            
            if (MapName == "Empty" || MapName != mapName)
            {
                MapName = mapName;
            }

            await Clock.WaitForNextTickAsync();
        }
    }
    // Internal service api
    public T GetService<T>() where T : class
    {
        var service = Modules
            .FirstOrDefault(x => x.GetType().FullName == typeof(T).FullName);

        return service as T ?? null!;
    }
    private void RegisterService<T>()
    {
        var instance = Activator.CreateInstance(typeof(T), this);
        var baseInstance = instance as BaseModule;
        
        Modules.Add(baseInstance!);
    }
}