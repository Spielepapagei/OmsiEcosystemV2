using Spectre.Console;

namespace OmsiClient.App.Services.UI;
public class MainMenuService
{
    public async void StartUi(string username)
    {
        AnsiConsole.Console.Clear();
    
        var live = new Table().Border(TableBorder.Simple);

        await AnsiConsole.Live(live)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Top)
            .StartAsync(async ctx =>
            {
                live.AddColumn("Options");
                live.AddColumn("Display").Expand();
                live.Title($"[orange3]OmsiApiClient[/] - [green]Welcome[/] {username}").LeftAligned();

                string[] c = new[]
                {
                    "Live Display",
                    "Options"
                };
            
                var options = new Table().LeftAligned().Border(TableBorder.Minimal);
                var display = new Table().LeftAligned().Border(TableBorder.Minimal);
            
                options.AddColumn(new TableColumn("Tabs").LeftAligned().Width(14));
                display.AddColumn(new TableColumn("Status").LeftAligned());
            
                live.AddRow(options, display);

                options.AddRow("Live Display");
                options.AddRow("Options");
                options.AddRow("Time Table");
                options.AddRow("Start new Shift");
                options.AddRow("Status");
                options.AddRow("Company View");
                
            
                display.AddRow("Start");
            
                return Task.CompletedTask;
            });
    
    }

}