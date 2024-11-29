// Link & Sync // Copyright 2023 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class LasUtilities
{
    
    /************************************************************************************************************************/

    public static bool Exists(string fileOrDirectoryPath)
        => File.Exists(fileOrDirectoryPath) || Directory.Exists(fileOrDirectoryPath);

    /************************************************************************************************************************/

    private static readonly string NormalizedDataPath = Application.dataPath.NormalizeSlashes();

    public static bool IsInsideThisProject(string path)
    {
        path = path.NormalizeSlashes();

        return Path.IsPathRooted(path)
            ? path.StartsWith(NormalizedDataPath)
            : path.StartsWith("Assets/");
    }

    /************************************************************************************************************************/

    public static bool ValidateExternalDirectories(IList<string> paths, bool logWarnings)
    {
        for (int i = 0; i < paths.Count; i++)
            if (!ValidateExternalDirectory(paths[i], logWarnings))
                return false;

        return true;
    }

    public static bool ValidateExternalDirectory(string path, bool logWarnings)
    {
        if (string.IsNullOrEmpty(path) ||
            (!Exists(path)))
            return false;

        if (Environment.CurrentDirectory.StartsWith(RelativeToAbsolute(path)))
        {
            if (logWarnings)
                Debug.LogWarning(
                    $"Invalid External Directory '{path}': must not be a parent directory of the current project.");
            return false;
        }

        if (IsInsideThisProject(path))
        {
            if (logWarnings)
                Debug.LogWarning(
                    $"Invalid External Directory '{path}': must be outside the current project.");
            return false;
        }

        return true;
    }

    /************************************************************************************************************************/

    public const char Slash = '/';

    /// <summary>Convert all forward slashes back slashes.</summary>
    public static string NormalizeSlashes(this string str)
        => str?.Replace('\\', Slash);

    public static string RemoveTrailingSlashes(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        var end = str.Length - 1;
        while (str[end] == Slash)
            end--;

        return str.Substring(0, end + 1);
    }

    /************************************************************************************************************************/

    public static bool BrowseForRelativeDirectory(string title, ref string path)
    {
        var folder = EditorUtility.OpenFolderPanel(title, path, "");
        if (string.IsNullOrEmpty(folder))
            return false;

        path = folder;
        return true;
    }

    public static bool BrowseForRelativeFile(string title, ref string path)
    {
        string folder = EditorUtility.OpenFilePanel(title, RelativeToAbsolute(path), "");
        if (string.IsNullOrEmpty(folder))
            return false;

        path = AbsoluteToRelative(folder);
        return true;
    }

    /************************************************************************************************************************/

    public static T[] FindAssetsOfType<T>() where T : Object
    {
        var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        var assets = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (string.IsNullOrEmpty(path))
                continue;

            assets[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return assets;
    }

    public static T FindAssetOfType<T>() where T : Object
    {
        var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        for (int i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (string.IsNullOrEmpty(path))
                continue;

            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                return asset;
        }

        return null;
    }

    /************************************************************************************************************************/
    #region Path Conversion
    /************************************************************************************************************************/

    private static readonly Dictionary<string, string>
        RelativePaths = new Dictionary<string, string>(),
        AbsolutePaths = new Dictionary<string, string>();

    private static string
        _CurrentDirectory,
        _PathRoot;

    /************************************************************************************************************************/

    public static string AbsoluteToRelative(string path)
    {
        if (string.IsNullOrEmpty(path))
            return "";

        path = path.NormalizeSlashes();

        if (RelativePaths.TryGetValue(path, out string relativePath))
            return relativePath;

        if (!Path.IsPathRooted(path))
        {
            RelativePaths.Add(path, path);
            return path;
        }

        if (_CurrentDirectory == null)
        {
            _CurrentDirectory = Environment.CurrentDirectory.NormalizeSlashes();
            _PathRoot = Path.GetPathRoot(_CurrentDirectory);
        }

        var root = Path.GetPathRoot(path);
        if (root != _PathRoot)
            return path;

        relativePath = path;

        // Trim any identical folders from the start.
        int index = 0;
        while (true)
        {
            var slash = relativePath.IndexOf(Slash, index);
            if (slash < 0) slash = relativePath.Length;

            if (string.Compare(relativePath, index, _CurrentDirectory, index, slash - index) != 0)
            {
                if (index == 0)
                    return relativePath;

                relativePath = relativePath.Substring(index, relativePath.Length - index);
                break;
            }
            else
            {
                index = slash + 1;
                if (index >= relativePath.Length)
                {
                    Debug.LogWarning($"Invalid path: {relativePath} is a direct parent of the current project.");
                    return "";
                }
                else if (index >= _CurrentDirectory.Length)
                {
                    return relativePath.Substring(index, relativePath.Length - index);
                }
            }
        }

        // Prefix the path with ../ for each folder level it needs to go up.
        do
        {
            relativePath = "../" + relativePath;
            index = _CurrentDirectory.IndexOf(Slash, index + 1);
        }
        while (index >= 0 && index < _CurrentDirectory.Length);

        RelativePaths.Add(path, relativePath);
        return relativePath;
    }

    /************************************************************************************************************************/

    public static string RelativeToAbsolute(string path)
        => string.IsNullOrEmpty(path) ? path : Path.GetFullPath(path);

    /************************************************************************************************************************/
    #endregion
    /************************************************************************************************************************/
}

#endif