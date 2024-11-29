using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

[CreateAssetMenu]
public class Dependencies : ScriptableObject
{
    [SerializeField] private string packageRootDirectory = "/Users/adamknight/Library/Unity/Asset Store-5.x";
    [SerializeField] private string packageDirectory = "Third Party";
    [SerializeField] private bool interactive = false;
    [SerializeField] private string[] packages;
    [SerializeField, ExternalPath] private string[] dependencies;
    [SerializeField] private string dependenciesDirectory = "Dependencies";
    [SerializeField] private string[] topLevelDirectories = new string[4] { "CodeEcho", "Third Party", "Dependencies", "Demo" };

    public void Update()
    {
        ImportPackages(packages);
        foreach (var dependency in dependencies)
        {
            ImportDependency(dependency);
        }
    }

    private void ImportPackages(string[] packages)
    {
        string[] files = Directory.GetFiles(packageRootDirectory, "*.unitypackage", SearchOption.AllDirectories);
        foreach (var package in packages)
        {
            string file = Array.Find(files, x => x.EndsWith(package + ".unitypackage"));
            if (file == null)
            {
                Debug.Log("WARNING: Could not find package: " + package);
            }
            else
            {
                ImportPackage(file);
            }
        }
    }

    private void ImportPackage(string file)
    {
        Debug.Log("Importing " + file);
        AssetDatabase.ImportPackage(file, interactive);
    }

    private void ImportDependency(string directory)
    {
        Debug.Log("Importing " + directory);
        var dir = CopyDirectory(directory, "Assets/" + dependenciesDirectory, true);
        AssetDatabase.Refresh();
        Dependencies subDependencies = (Dependencies)AssetDatabase.LoadAssetAtPath(dir + "/Dependencies.asset", typeof(Dependencies));
        if (subDependencies == null) return;
        ImportPackages(subDependencies.GetPackages());
        foreach (var dependency in subDependencies.GetDependencies())
        {
            ImportDependency(dependency);
        }
    }

    public string[] GetPackages()
    {
        return packages;
    }

    public string[] GetDependencies()
    {
        return dependencies;
    }

    public void TidyUp()
    {
        var directories = Directory.GetDirectories("./Assets").Select(x => x.Replace("./Assets/", "")).ToArray();
        var toMove = directories.Except(topLevelDirectories);
        foreach (var directory in toMove)
        {
            Debug.Log("Moving directory: " + directory);
            AssetDatabase.MoveAsset("Assets/" + directory, "Assets/" + packageDirectory + "/" + directory);
        }
    }

    private string CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        destinationDir = Path.Combine(destinationDir, dir.Name);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }

        return destinationDir;
    }
}
