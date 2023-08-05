using Spectre.Console;

namespace OmsiApiClient.App.Services;

public class SessionManager
{
    public SessionManager()
    {
        Task.Run(LoginPage);
    }

    private void LoginPage()
    {
        AnsiConsole.Console.Clear();
        Rule rule = Program.AppRule;
        rule.Title = Program.AppRule.Title + "[silver] -[/] [green]Login[/]";
        AnsiConsole.Write(rule);
        
        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("This is the [green]Login[/]. You have no Login? Then please ask a [red]Administrator[/].");
        AnsiConsole.MarkupLine("[grey19]With Logging in you Accept the Terms of Service and Data privacy policy.[/]");
        AnsiConsole.MarkupLine("");
        
        var username = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]username[/]?")
                .PromptStyle("gray"));
        
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]password[/]?")
                .PromptStyle("red")
                .Secret());
        
        
        
    }
    
}