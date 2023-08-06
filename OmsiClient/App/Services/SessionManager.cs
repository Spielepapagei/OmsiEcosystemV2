using Flurl;
using Flurl.Http;
using OmsiApiServer.App.Dtos;
using OmsiClient.App.Services.Configuration;
using Spectre.Console;

namespace OmsiClient.App.Services;

public class SessionManager
{
    private ConfigService Config;
    private readonly string AppUrl;
    private string Token;
    private ConfigV1 ConfigModel;
    
    public SessionManager(ConfigService config)
    {
        ConfigModel = config.Get();

        Config = config;
        
        AppUrl = config.Get().OmsiClient.AppUrl;
        Token = config.Get().OmsiClient.AuthData.Token;
            
        Task.Run(LoginPage);
    }

    private async Task LoginPage()
    {
        AnsiConsole.Console.Clear();
        Rule rule = Program.AppRule;
        rule.Title = Program.AppRule.Title + "[silver] -[/] [green]Login[/]";
        AnsiConsole.Write(rule);
        AnsiConsole.MarkupLine("");
        
        if (AppUrl == string.Empty)
        {
            AnsiConsole.MarkupLine("[red]AppUrl is Empty[/]");
            AnsiConsole.MarkupLine("");
        }

        ServiceResponse<string> i = new ServiceResponse<string>();
        
        while (true)
        {
            var checkToken = await CheckToken();

            if (checkToken is { Success: true })
            {
                i = checkToken;
                break;
            }
            
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
        
            try
            {
                var response = await AppUrl
                    .AppendPathSegment("api/auth/login")
                    .PostJsonAsync(new
                    {
                        username = username,
                        password = password
                    })
                    .ReceiveJson<ServiceResponse<string>>();

                if (response != null && response.Data != null)
                {
                    Token = response.Data;
                    ConfigModel.OmsiClient.AuthData.Token = response.Data;
                    Config.Save(ConfigModel);
                }
            
                //AnsiConsole.MarkupLine($"[blue]{response.Data}[/]");
            }
            catch (Exception e)
            {
                AnsiConsole.Write(e.Message + e.InnerException);
                throw;
            }
        }
        
        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine($"Logged In [green]Successful[/] as [green]{i.Data}[/]");
    }

    private async Task<ServiceResponse<string>> CheckToken()
    {
        var response = new ServiceResponse<string>
        {
            Success = false
        };
        
        try
        {
            AnsiConsole.MarkupLine("[green]Try Logging in...[/]");
            var check = await AppUrl.AppendPathSegment("api/auth/check").WithOAuthBearerToken(Token).GetJsonAsync<ServiceResponse<string>>();
            response.Data = check.Data;
            response.Success = check.Success;
            response.Message = check.Message;
            
            return response;
        }
        catch (Exception e)
        {
            AnsiConsole.Write(e.Message + e.InnerException);
            AnsiConsole.MarkupLine("");
        }

        return response;
    }
}