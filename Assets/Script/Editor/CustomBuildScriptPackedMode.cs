using System.IO;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;

[UnityEngine.CreateAssetMenu(fileName = "CustomBuildScriptPacked", menuName = "Addressables/Content Builders/Custom Build Script")]
public class CustomBuildScriptPackedMode : BuildScriptPackedMode
{
    protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput builderInput)
    {
        TResult result = base.BuildDataImplementation<TResult>(builderInput);

        CopyCatalogFiles();

        return result;
    }

    private void CopyCatalogFiles()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var profileId = settings.activeProfileId;

        // Your custom profile variable
        string bundleOutputPath = settings.profileSettings.GetValueByName(profileId, "Persistent" + ".BuildPath");
        UnityEngine.Debug.Log($"{bundleOutputPath}");

        bundleOutputPath = settings.profileSettings.EvaluateString(profileId, bundleOutputPath);
        UnityEngine.Debug.Log($"{bundleOutputPath}");

        Directory.CreateDirectory(bundleOutputPath);

        // Addressables puts the catalog in aa/<platform>
        string aaFolder = Addressables.BuildPath;

        if (!Directory.Exists(aaFolder))
        {
            UnityEngine.Debug.LogError($"Addressables output folder not found: {aaFolder}");
            return;
        }

        foreach (string file in Directory.GetFiles(aaFolder))
        {
            string fileName = Path.GetFileName(file);

            if (fileName.StartsWith("catalog"))
            {
                File.Copy(file, Path.Combine(bundleOutputPath, fileName), true);

                UnityEngine.Debug.Log($"Copied {fileName} from {aaFolder} -> {bundleOutputPath}");
            }
        }
    }
}