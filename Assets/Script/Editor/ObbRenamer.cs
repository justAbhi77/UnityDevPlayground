using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Android;
using UnityEngine;

public class OBBRenamer
{
    [PostProcessBuildAttribute]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltApk)
    {
        // Only run for Android builds
        if (target != BuildTarget.Android)
            return;

        // Determine the OBB file path
        // Unity typically names the OBB the same as the APK but with .obb extension
        var apkDirectory = Path.GetDirectoryName(pathToBuiltApk);
        var obbsPath = Directory.GetFiles(apkDirectory, "*.obb");

        if (obbsPath.Length == 0)
        {
            Debug.LogWarning($"No OBB file found in: {apkDirectory}. Ensure 'Split Application Binary' is enabled.");
            return;
        }

        var obbPath = obbsPath[0];

        // Get Version Code and Package Name from Player Settings
        // Note: BundleVersionCode is an integer, BundleVersion is the string version
        string versionCode = PlayerSettings.Android.bundleVersionCode.ToString();
        string packageName = PlayerSettings.applicationIdentifier;

        // Construct the new filename: main.[versionCode].[packageName].obb
        string newFileName = $"main.{versionCode}.{packageName}.obb";
        string newObbPath = Path.Combine(apkDirectory, newFileName);

        // Rename the file
        try
        {
            File.Move(obbPath, newObbPath);
            Debug.Log($"Successfully renamed OBB to: {newFileName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to rename OBB file: {e.Message}");
        }
    }
}