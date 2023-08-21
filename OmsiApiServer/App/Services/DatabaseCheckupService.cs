using System.Diagnostics;
using System.IO.Compression;
using System.Net.NetworkInformation;
using Logging.Net;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using OmsiApiServer.App.Database;
using OmsiApiServer.App.Helpers;
using OmsiApiServer.App.Services.Configuration;
using Spectre.Console;

namespace OmsiApiServer.App.Services;

public class DatabaseCheckupService
{
    private readonly ConfigService ConfigService;

    public DatabaseCheckupService(ConfigService configService)
    {
        ConfigService = configService;
    }

    public async Task Perform()
    {
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("green"))
            .StartAsync("Testing Connection.", async ctx =>
            {
                
                var pingReply = await PingIp(ConfigService.Get().OmsiClient.Database.Host);
                if (pingReply is not { Status: IPStatus.Success })
                {
                    ctx.Status("Exiting OmsiApi will wait 1 minute, then exit");
                    await Task.Run(AppExit);
                }
                
                AnsiConsole.MarkupLine("[green]Checked[/] Connection to Host.");
                
                var context = new DataContext(ConfigService);

                ctx.Status("[green]Checking[/] database");
                ctx.Refresh();
        
                if (!await context.Database.CanConnectAsync())
                {
                    ctx.Status("Exiting OmsiApi will wait 1 minute, then exit");
                    await Task.Run(AppExit);
                }
                
                AnsiConsole.MarkupLine("[green]Checked[/] database connection.");
                
                ctx.Status("Checking for pending migrations");
                ctx.Refresh();
                
                var migrations = (await context.Database
                    .GetPendingMigrationsAsync())
                    .ToArray();

                if (migrations.Any())
                {
                    ctx.Spinner(Spinner.Known.BouncingBar);
                    AnsiConsole.MarkupLine($"{migrations.Length} migrations [orange3]pending[/]. Updating now");
                    
            
                    var path = PathBuilder.File("storage", "backups", $"{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()}.zip");

                    ctx.Status("Started backup creation");
                    ctx.Refresh();
                    AnsiConsole.MarkupLine($"This backup will be [green]saved[/] to '{path}'");

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
        
                    var cachePath = PathBuilder.Dir("storage", "backups", "cache");

                    Directory.CreateDirectory(cachePath);

                    //
                    // Exporting database
                    //
        
                    ctx.Status("Exporting database");

                    var configService = new ConfigService(new());
                    var dataContext = new DataContext(configService);

                    await using MySqlConnection conn = new MySqlConnection(dataContext.Database.GetConnectionString());
                    await using MySqlCommand cmd = new MySqlCommand();
                    using MySqlBackup mb = new MySqlBackup(cmd);
        
                    cmd.Connection = conn;
                    await conn.OpenAsync();
                    mb.ExportToFile(PathBuilder.File(cachePath, "database.sql"));
                    await conn.CloseAsync();
        
                    //
                    // Saving config
                    //

                    ctx.Status("Saving configuration");
                    
                    File.Copy(
                        PathBuilder.File("storage", "configs", "config.json"), 
                        PathBuilder.File(cachePath, "config.json"));
        
                    //
                    // Compressing the backup to a single file
                    //
        
                    ctx.Status("Compressing");
                    ZipFile.CreateFromDirectory(cachePath, 
                        path, 
                        CompressionLevel.Fastest, 
                        false);
        
                    Directory.Delete(cachePath, true);
        
                    stopWatch.Stop();
                    AnsiConsole.MarkupLine($"Backup [green]successfully[/] created. Took {stopWatch.Elapsed.TotalSeconds} seconds");
            
                    ctx.Status("[green]Applying[/] migrations");
                    ctx.Refresh();
            
                    await context.Database.MigrateAsync();
            
                    ctx.Status("Finishing up");
                    ctx.Refresh();
                    AnsiConsole.MarkupLine("Successfully applied migrations");
                }
                else
                {
                    AnsiConsole.MarkupLine("Database is up-to-date. No migrations have been performed");
                }
            });
    }

    private void AppExit()
    {
        AnsiConsole.MarkupLine("[red]Unable[/] to connect to mysql database");
            
        Thread.Sleep(TimeSpan.FromMinutes(1));
        Environment.Exit(10324);
    }
    
    private async Task<PingReply?> PingIp(string ip)
    {
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

    private async Task CreateBackup(string path)
    {
        
    }
    
    private void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs = true)
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");
        }

        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        FileInfo[] files = dir.GetFiles();

        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        if (copySubDirs)
        {
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }
}