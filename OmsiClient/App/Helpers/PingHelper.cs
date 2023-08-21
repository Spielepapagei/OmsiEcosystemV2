using System.Net.NetworkInformation;
using Spectre.Console;

namespace OmsiClient.App.Helpers;

public class PingHelper
{
    
    public async Task<PingReply?> PingIp(string url)
    {
        var ip = GetDomainFromUrl(url);
        
        if (ip == null)
        {
            AnsiConsole.MarkupLine($"Error Parsing Url: [gray]{url}[/] Please Verify this is Correct.");
            return null;
        }
        
        AnsiConsole.MarkupLine($"Pinging [gray]{ip}[/]");
        try
        {
            var reply = new Ping().Send(ip);

            if (reply.Status != IPStatus.Success)
            {
                AnsiConsole.MarkupLine($"Ping to [gray]{ip}[/] [red]failed.[/] [blue]Status: {reply.Status}[/]");
                return reply;
            }
            AnsiConsole.MarkupLine($"Ping to [gray]{ip}[/] [green]succeeded[/]. [blue]Ping: {reply.RoundtripTime}ms[/]");
            return reply;
        }
        catch (PingException ex)
        {
            AnsiConsole.MarkupLine($"An Unexpected Error has occurred. While pinning: [gray]{ip}[/]");
            await Task.Delay(TimeSpan.FromMinutes(1));
            throw;
        }
    }
    
    private static string? GetDomainFromUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.Host : null;
    }
}