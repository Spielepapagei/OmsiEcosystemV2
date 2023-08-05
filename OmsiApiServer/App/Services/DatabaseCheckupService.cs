﻿using System.Diagnostics;
using DiscordLogging.App.Database;
using DiscordLogging.App.Services;
using Logging.Net;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

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
        var context = new DataContext(ConfigService);

        Logger.Info("Checking database");
        
        if (!await context.Database.CanConnectAsync())
        {
            Logger.Fatal("-----------------------------------------------");
            Logger.Fatal("Unable to connect to mysql database");
            Logger.Fatal("Please make sure the configuration is correct");
            Logger.Fatal("");
            Logger.Fatal("DiscordLogging will wait 1 minute, then exit");
            Logger.Fatal("-----------------------------------------------");
            
            Thread.Sleep(TimeSpan.FromMinutes(1));
            Environment.Exit(10324);
        }

        Logger.Info("Checking for pending migrations");

        var migrations = (await context.Database
            .GetPendingMigrationsAsync())
            .ToArray();

        if (migrations.Any())
        {
            Logger.Info($"{migrations.Length} migrations pending. Updating now");
            
            await BackupDatabase();
            
            Logger.Info("Applying migrations");
            
            await context.Database.MigrateAsync();
            
            Logger.Info("Successfully applied migrations");
        }
        else
        {
            Logger.Info("Database is up-to-date. No migrations have been performed");
        }
    }

    public async Task BackupDatabase()
    {
        Logger.Info("Creating backup from database");
        
        var configService = new ConfigService(new StorageService());
        var dateTimeService = new DateTimeService();

        var config = configService
            .GetSection("DiscordLogging")
            .GetSection("Database");

        var connectionString = $"host={config.GetValue<string>("Host")};" +
                               $"port={config.GetValue<int>("Port")};" +
                               $"database={config.GetValue<string>("Database")};" +
                               $"uid={config.GetValue<string>("Username")};" +
                               $"pwd={config.GetValue<string>("Password")}";
        
        string file = PathBuilder.File("storage", "backups", $"{dateTimeService.GetCurrentUnix()}-mysql.sql");
        
        Logger.Info($"Saving it to: {file}");
        Logger.Info("Starting backup...");

        var sw = new Stopwatch();
        sw.Start();

        await using MySqlConnection conn = new MySqlConnection(connectionString);
        await using MySqlCommand cmd = new MySqlCommand();
        using MySqlBackup mb = new MySqlBackup(cmd);
        
        cmd.Connection = conn;
        await conn.OpenAsync();
        mb.ExportToFile(file);
        await conn.CloseAsync();

        sw.Stop();
        Logger.Info($"Done. {sw.Elapsed.TotalSeconds}s");
    }
}