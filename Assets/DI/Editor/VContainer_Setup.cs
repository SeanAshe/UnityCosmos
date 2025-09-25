using UnityEditor;
using UnityEngine;
using System.IO;
using Cosmos.System.Tools;
using System.Linq;


namespace Cosmos.DI
{
    public class VContainerSetupEditor : EditorWindow
    {
        static readonly string DIPath = Application.dataPath + "/DIRefrences/";
        static readonly string VContainerDLLPath = "Assets/DIRefrences/VContainer.SourceGenerator.dll";
        [MenuItem("DI/VContainer Setup")]
        static async void VContainer_Setup()
        {
            if (!File.Exists(DIPath + "VContainer.SourceGenerator.dll"))
            {
                EditorUtility.DisplayProgressBar("VContainer_Setup", $"Downloading VContainer...", 0);
                if (!Directory.Exists(DIPath)) Directory.CreateDirectory(DIPath);
                await GithubReleaseDownload.Download("hadashiA", "VContainer", "VContainer.SourceGenerator.dll", DIPath);
                EditorUtility.ClearProgressBar();
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            PluginImporter pluginImporter = AssetImporter.GetAtPath(VContainerDLLPath) as PluginImporter;
            pluginImporter.SetCompatibleWithAnyPlatform(false);
            pluginImporter.SetCompatibleWithEditor(false);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, false);
            pluginImporter.SaveAndReimport();

            var assetObject = AssetDatabase.LoadAssetAtPath<Object>(VContainerDLLPath);
            string[] labels = AssetDatabase.GetLabels(assetObject);
            labels = labels.Append("RoslynAnalyzer").ToArray();
            AssetDatabase.SetLabels(assetObject, labels);
            EditorUtility.SetDirty(assetObject);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        public class GitHubRelease
        {
            public GitHubAsset[] assets { get; set; }
        }
        public class GitHubAsset
        {
            public string url { get; set; }
            public string name { get; set; }
        }
    }
}
