using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class AssetAddressableSizeChecker : EditorWindow
{
    private readonly List<Object> _assetSelection = new ();
    private readonly List<Object> _addressableSelection = new ();
    
    private Vector2 _scrollPosition;
    private string _resultText = string.Empty;

    [MenuItem("Tools/Asset & Addressable Size Checker")]
    public static void ShowWindow()
    {
        GetWindow<AssetAddressableSizeChecker>("Asset & Addressable Size Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Asset & Addressable Size Checker", EditorStyles.boldLabel);
        GUILayout.Space(10);

        int selectedTab = GUILayout.Toolbar(
            EditorPrefs.GetInt("AssetCheckerTab", 0),
            new[] { "Asset Size", "Addressable Size" }
        );
        EditorPrefs.SetInt("AssetCheckerTab", selectedTab);

        GUILayout.Space(10);

        if (selectedTab == 0)
        {
            DrawAssetSizeTab();
        }
        else
        {
            DrawAddressableSizeTab();
        }

        GUILayout.Space(20);
        GUILayout.Label("Results", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(_resultText, MessageType.Info);
    }

    private void DrawAssetSizeTab()
    {
        GUILayout.Label("Asset Size Estimator", EditorStyles.boldLabel);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(200));
        for (int i = 0; i < _assetSelection.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            _assetSelection[i] = EditorGUILayout.ObjectField(_assetSelection[i], typeof(Object), false);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                _assetSelection.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("Add Asset/Folder"))
        {
            _assetSelection.Add(null);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Calculate Total Size"))
        {
            long totalSize = 0;
            foreach (Object asset in _assetSelection)
            {
                if (asset != null)
                {
                    string path = AssetDatabase.GetAssetPath(asset);
                    totalSize += CalculateSizeForPath(path);
                }
            }

            _resultText = $"Total Asset Size: {FormatBytes(totalSize)}";
        }
    }

    private void DrawAddressableSizeTab()
    {
        GUILayout.Label("Addressable Size Estimator", EditorStyles.boldLabel);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(200));
        for (int i = 0; i < _addressableSelection.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            _addressableSelection[i] = EditorGUILayout.ObjectField(_addressableSelection[i], typeof(Object), false);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                _addressableSelection.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("Add Addressable Asset"))
        {
            _addressableSelection.Add(null);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Calculate Total Addressable Size"))
        {
            long totalSize = 0;
            foreach (Object asset in _addressableSelection)
            {
                if (asset != null && IsAddressableAsset(asset))
                {
                    string path = AssetDatabase.GetAssetPath(asset);
                    totalSize += CalculateSizeForPath(path);
                }
            }

            _resultText = $"Total Addressable Asset Size: {FormatBytes(totalSize)}";
        }
    }

    private bool IsAddressableAsset(Object obj)
    {
        // Get the addressable settings
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(false);
        if (settings == null)
            return false;

        // Check if the object is part of addressable assets
        string path = AssetDatabase.GetAssetPath(obj);
        string guid = AssetDatabase.AssetPathToGUID(path);
        var entry = settings.FindAssetEntry(guid);

        return entry != null;
    }

    private long CalculateSizeForPath(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            string[] assetPaths = AssetDatabase.FindAssets("", new[] { path });
            long folderSize = 0;
            foreach (string guid in assetPaths)
            {
                folderSize += CalculateSizeForPath(AssetDatabase.GUIDToAssetPath(guid));
            }

            return folderSize;
        }
        else
        {
            string filePath = GetFullPath(path);
            if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length;
            }

            return 0;
        }
    }

    private string GetRelativePath(string fullPath)
    {
        string projectPath = Application.dataPath.Replace("Assets", "");
        return fullPath.StartsWith(projectPath) ? fullPath.Substring(projectPath.Length) : fullPath;
    }

    private string GetFullPath(string relativePath)
    {
        return Path.Combine(Application.dataPath.Replace("Assets", ""), relativePath);
    }

    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        while (bytes >= 1024 && order < sizes.Length - 1)
        {
            order++;
            bytes /= 1024;
        }

        return $"{bytes:0.##} {sizes[order]}";
    }
}