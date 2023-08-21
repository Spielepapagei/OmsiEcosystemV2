using Flurl;
using Flurl.Http;
using OmsiApiServer.App.Dtos;
using OmsiClient.App.Helpers;
using OmsiClient.App.Services.Configuration;
using OmsiClient.App.Services.UI;
using Spectre.Console;

namespace OmsiClient.App.Services;

public class SessionManager
{
    private readonly ConfigService Config;
    private string? AppUrl;
    private string Token;
    private ConfigV1 ConfigModel;
    private readonly PingHelper PingHelper;
    private readonly MainMenuService MainMenu;
    
    public SessionManager(ConfigService config, PingHelper pingHelper, MainMenuService mainMenu)
    {
        ConfigModel = config.Get();

        Config = config;
        PingHelper = pingHelper;
        MainMenu = mainMenu;

        AppUrl = config.Get().OmsiClient.AppUrl;
        Token = config.Get().OmsiClient.AuthData.Token;
            
        //Task.Run(LoginPage);
    }
    
    private async Task LoginPage()
    {
        AnsiConsole.Console.Clear();
        Rule rule = Program.AppRule;
        rule.Title = Program.AppRule.Title + "[silver] -[/] [green]Login[/]";
        AnsiConsole.Write(rule);
        AnsiConsole.MarkupLine("");
        
        while (AppUrl is null)
        {
            await SetAppUrl();
        }
        
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("green"))
            .StartAsync("Testing Connection.", async ctx =>
            {
                await PingHelper.PingIp(AppUrl);
            });

        ServiceResponse<string> i = new ServiceResponse<string>();
        
        var checkToken = await CheckToken();
            
        if (checkToken is { Success: true })
        {
            i = checkToken;
        }
        
        while (!checkToken.Success)
        {
            var response = await LogIn(rule);
            
            if (response.Success)
            {
                checkToken = await CheckToken();
            
                if (checkToken is { Success: true })
                {
                    i = checkToken;
                    break;
                }
            }
        }
        
        
        
        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine($"Logged In [green]Successful[/] as [green]{i.Data}[/]");
        await Task.Delay(TimeSpan.FromMilliseconds(1500));

        i.Data ??= "Undefined";
        MainMenu.StartUi(i.Data);
    }

    private async Task<ServiceResponse<string>> LogIn(Rule rule)
    {
        AnsiConsole.Console.Clear();
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
        
        try
        {
            var response = await AppUrl
                .AppendPathSegment("/api/auth/login")
                .PostJsonAsync(new
                {
                    username = username,
                    password = password
                })
                .ReceiveJson<ServiceResponse<string>>();

            if (response is { Data: not null })
            {
                Token = response.Data;
                ConfigModel.OmsiClient.AuthData.Token = response.Data;
                Config.Save(ConfigModel);
                ConfigModel = Config.Get();
                return response;
            }
                
        }
        catch (FlurlHttpException  e)
        {
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"Api Call Error StatusCode: [red]{e.StatusCode ?? 404}[/]");
            await Task.Delay(TimeSpan.FromMilliseconds(3000));
        }

        return new ServiceResponse<string>()
        {
            Success = false
        };
    }
    
    private async Task<ServiceResponse<string>> CheckToken()
    {
        var response = new ServiceResponse<string>
        {
            Success = false
        };
        
        try
        {
            AnsiConsole.MarkupLine("[green]Success[/] Checking Token");
            var check = await AppUrl.AppendPathSegment("/api/auth/check").WithOAuthBearerToken(Token).GetJsonAsync<ServiceResponse<string>>();
            response.Data = check.Data;
            response.Success = check.Success;
            response.Message = check.Message;
            
            return response;
        }
        catch (FlurlHttpException e)
        {
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"Api Call Error StatusCode: [red]{e.StatusCode ?? 404}[/]");
            await Task.Delay(TimeSpan.FromMilliseconds(3000));
        }

        return response;
    }

    private Task SetAppUrl()
    {
        AnsiConsole.MarkupLine("[red]AppUrl is Empty[/]");
        AnsiConsole.MarkupLine("");
            
        var appUrl = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter an [blue]AppUrl[/]?")
                .PromptStyle("blue"));

        AppUrl = appUrl;
        ConfigModel.OmsiClient.AppUrl = appUrl;
        Config.Save(ConfigModel);
        ConfigModel = Config.Get();
        
        return Task.CompletedTask;
    }
}