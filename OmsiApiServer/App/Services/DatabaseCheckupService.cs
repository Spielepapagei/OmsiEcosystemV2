﻿using System.Diagnostics;
using System.IO.Compression;
using Logging.Net;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using OmsiApiServer.App.Database;
using OmsiApiServer.App.Helpers;
using OmsiApiServer.App.Services.Configuration;

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
            Logger.Fatal("OmsiApiServer will wait 1 minute, then exit");
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
            
            await CreateBackup(PathBuilder.File("storage", "backups", $"{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()}.zip"));
            
            Logger.Info("Applying migrations");
            
            await context.Database.MigrateAsync();
            
            Logger.Info("Successfully applied migrations");
        }
        else
        {
            Logger.Info("Database is up-to-date. No migrations have been performed");
        }
    }

    public async Task CreateBackup(string path)
    {
        Logger.Info("Started moonlight backup creation");
        Logger.Info($"This backup will be saved to '{path}'");

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        var cachePath = PathBuilder.Dir("storage", "backups", "cache");

        Directory.CreateDirectory(cachePath);

        //
        // Exporting database
        //
        
        Logger.Info("Exporting database");

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
        
        Logger.Info("Saving configuration");
        File.Copy(
            PathBuilder.File("storage", "configs", "config.json"), 
            PathBuilder.File(cachePath, "config.json"));
        
        //
        // Saving all storage items needed to restore the panel
        //
        
        Logger.Info("Saving resources");
        CopyDirectory(
            PathBuilder.Dir("storage", "resources"), 
            PathBuilder.Dir(cachePath, "resources"));

        Logger.Info("Saving logs");
        CopyDirectory(
            PathBuilder.Dir("storage", "logs"), 
            PathBuilder.Dir(cachePath, "logs"));

        Logger.Info("Saving uploads");
        CopyDirectory(
            PathBuilder.Dir("storage", "uploads"), 
            PathBuilder.Dir(cachePath, "uploads"));
        
        //
        // Compressing the backup to a single file
        //
        
        Logger.Info("Compressing");
        ZipFile.CreateFromDirectory(cachePath, 
            path, 
            CompressionLevel.Fastest, 
            false);
        
        Directory.Delete(cachePath, true);
        
        stopWatch.Stop();
        Logger.Info($"Backup successfully created. Took {stopWatch.Elapsed.TotalSeconds} seconds");
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