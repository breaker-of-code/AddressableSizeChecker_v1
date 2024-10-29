using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AssetAddressableSizeChecker : EditorWindow
{
    private enum Tab { AssetSize, AddressableSize }
    private Tab selectedTab = Tab.AssetSize;

    private List<Object> selectedAssets = new List<Object>();
    private Object selectedAddressableAsset;
    private string result;

    [MenuItem("Tools/Asset & Addressable Size Checker")]
    public static void ShowWindow()
    {
        GetWindow<AssetAddressableSizeChecker>("Asset & Addressable Size Checker");
    }

    private void OnGUI()
    {
        // Draw Tabs
        selectedTab = (Tab)GUILayout.Toolbar((int)selectedTab, new string[] { "Asset Size", "Addressable Size" });

        // Draw content based on selected tab
        switch (selectedTab)
        {
            case Tab.AssetSize:
                DrawAssetSizeTab();
                break;
            case Tab.AddressableSize:
                DrawAddressableSizeTab();
                break;
        }
    }

    private void DrawAssetSizeTab()
    {
        GUILayout.Label("Select Assets or Folders to Calculate Size in Build", EditorStyles.boldLabel);

        // Display list of assets with the ability to add or remove
        for (int i = 0; i < selectedAssets.Count; i++)
        {
            selectedAssets[i] = EditorGUILayout.ObjectField("Asset/Folder " + (i + 1), selectedAssets[i], typeof(Object), false);
        }

        if (GUILayout.Button("Add Asset/Folder"))
        {
            selectedAssets.Add(null);
        }

        if (selectedAssets.Count > 0 && GUILayout.Button("Remove Last Asset/Folder"))
        {
            selectedAssets.RemoveAt(selectedAssets.Count - 1);
        }

        if (GUILayout.Button("Calculate Total Size"))
        {
            result = CalculateTotalAssetSize();
        }

        if (!string.IsNullOrEmpty(result))
        {
            EditorGUILayout.HelpBox(result, MessageType.Info);
        }
    }

    private void DrawAddressableSizeTab()
    {
        GUILayout.Label("Select an Addressable Asset to Calculate Size", EditorStyles.boldLabel);

        selectedAddressableAsset = EditorGUILayout.ObjectField("Addressable Asset", selectedAddressableAsset, typeof(Object), false);

        if (GUILayout.Button("Calculate Addressable Size"))
        {
            if (selectedAddressableAsset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedAddressableAsset);
                result = CalculateAssetSize(assetPath);
            }
            else
            {
                result = "Please select an addressable asset.";
            }
        }

        if (!string.IsNullOrEmpty(result))
        {
            EditorGUILayout.HelpBox(result, MessageType.Info);
        }
    }

    private string CalculateTotalAssetSize()
    {
        HashSet<string> uniqueDependencies = new HashSet<string>();
        long totalSize = 0;

        // Collect all unique dependencies from selected assets or folders
        foreach (Object asset in selectedAssets)
        {
            if (asset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);

                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    string[] folderAssets = AssetDatabase.FindAssets("", new[] { assetPath });
                    foreach (string guid in folderAssets)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        string[] dependencies = AssetDatabase.GetDependencies(path);
                        foreach (string dependency in dependencies)
                        {
                            uniqueDependencies.Add(dependency);
                        }
                    }
                }
                else
                {
                    string[] dependencies = AssetDatabase.GetDependencies(assetPath);
                    foreach (string dependency in dependencies)
                    {
                        uniqueDependencies.Add(dependency);
                    }
                }
            }
        }

        // Calculate total size for all unique dependencies
        foreach (string dependencyPath in uniqueDependencies)
        {
            string fullPath = Path.Combine(Application.dataPath, dependencyPath.Substring(7)); // Remove "Assets/" from path
            if (File.Exists(fullPath))
            {
                FileInfo fileInfo = new FileInfo(fullPath);
                totalSize += fileInfo.Length;
            }
        }

        return $"Total Size of Selected Assets/Folders: {totalSize / 1024f / 1024f:0.##} MB";
    }

    private string CalculateAssetSize(string assetPath)
    {
        HashSet<string> uniqueDependencies = new HashSet<string>(AssetDatabase.GetDependencies(assetPath));
        long totalSize = 0;

        // Calculate total size for all unique dependencies
        foreach (string dependencyPath in uniqueDependencies)
        {
            string fullPath = Path.Combine(Application.dataPath, dependencyPath.Substring(7)); // Remove "Assets/" from path
            if (File.Exists(fullPath))
            {
                FileInfo fileInfo = new FileInfo(fullPath);
                totalSize += fileInfo.Length;
            }
        }

        return $"Total Size: {totalSize / 1024f / 1024f:0.##} MB";
    }
}
