using OmsiHook;
using Spectre.Console;

namespace OmsiClient.App.Services.OmsiDataClient.Modules;

public class LoadBusStops : BaseModule
{
    private List<int> LoadedTiles;
        
    public LoadBusStops(OmsiDataService client) : base(client)
    {
        LoadedTiles = new List<int>();
    }

    public override Task Run()
    {
        throw new NotImplementedException();
    }

    public override Task MapChange()
    {
        AnsiConsole.MarkupLine($"[green]Hello World![/]");
        LoadedTiles = Client.Hook.Globals.Map.KachelInfos.Select(x => x.mapKachel).ToList();

        foreach (var tile in LoadedTiles)
        {
            AnsiConsole.MarkupLine($"[green]{tile}[/]");
        }
        
        return Task.CompletedTask;
    }
}