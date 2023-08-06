﻿using DiscordLogging.App.Services;
using Spectre.Console;

namespace OmsiApiServer.App.Services;

public class StorageService
{
    public StorageService()
    {
        EnsureCreated();
    }
    
    public void EnsureCreated()
    {
        Directory.CreateDirectory(PathBuilder.Dir("storage", "configs"));
        Directory.CreateDirectory(PathBuilder.Dir("storage", "backup"));
        
        if (IsEmpty(PathBuilder.Dir("storage", "configs")))
        {
            AnsiConsole.MarkupLine("[orange3]Default configs not found.[/] [green]Copying default configs[/]");
            
            CopyFilesRecursively(
                PathBuilder.Dir("defaultstorage", "configs"), 
                PathBuilder.Dir("storage", "configs")
            );
        }
    }
    private bool IsEmpty(string path)
    {
        return !Directory.EnumerateFileSystemEntries(path).Any();
    }
    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}