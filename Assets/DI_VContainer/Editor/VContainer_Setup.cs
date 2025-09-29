using UnityEditor;
using UnityEngine;
using System.IO;
using Cosmos.System.Tools;
using System.Linq;


namespace Cosmos.DI
{
    public class VContainerSetupEditor : EditorWindow
    {
        static readonly string DIPath = $"{Application.dataPath}/DI/";
        static readonly string GameplayModelPath = $"{Application.dataPath}/Scripts/GameplayModel/";
        const string VContainerDLLPath = "Assets/DI/VContainer.SourceGenerator.dll";

        [MenuItem("DI/VContainer Setup", false, 0)]
        static async void VContainer_Setup()
        {
            if (!Directory.Exists(DIPath)) Directory.CreateDirectory(DIPath);
            if (!File.Exists($"{DIPath}VContainer.SourceGenerator.dll"))
            {
                EditorUtility.DisplayProgressBar("VContainer Setup", $"Downloading VContainer...", 0);
                await GithubReleaseDownload.Download("hadashiA", "VContainer", "VContainer.SourceGenerator.dll", DIPath);
                EditorUtility.ClearProgressBar();
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            EditorUtility.DisplayProgressBar("VContainer Setup", $"Initialize VContainer...", 0);
            //设置VContainer.SourceGenerator.dll
            PluginImporter pluginImporter = AssetImporter.GetAtPath(VContainerDLLPath) as PluginImporter;
            pluginImporter.SetCompatibleWithAnyPlatform(false);
            pluginImporter.SetCompatibleWithEditor(false);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, false);
            pluginImporter.SaveAndReimport();

            var assetObject = AssetDatabase.LoadAssetAtPath<Object>(VContainerDLLPath);
            string[] labels = new string[] { "RoslynAnalyzer" };
            AssetDatabase.SetLabels(assetObject, labels);
            EditorUtility.SetDirty(assetObject);

            if (!Directory.Exists($"{DIPath}Runtime")) Directory.CreateDirectory($"{DIPath}Runtime");
            if (!Directory.Exists($"{DIPath}Editor"))  Directory.CreateDirectory($"{DIPath}Editor");
            if (!Directory.Exists(GameplayModelPath))  Directory.CreateDirectory(GameplayModelPath);
            // 写入RootLifetimeScope.cs
            File.WriteAllText($"{DIPath}Runtime/RootScope.cs", Script_Template.RootScope_cs);
            // 写入GlobalSignalScope.cs
            File.WriteAllText($"{DIPath}Runtime/GlobalSignalScope.cs", Script_Template.GlobalSignalScope_cs);
            // 写入GameRoot.cs
            File.WriteAllText($"{GameplayModelPath}GameplayModel.cs", Script_Template.GameplayModel_cs);
            // 写入AutoInjectHelperEditor.cs
            File.WriteAllText($"{DIPath}Editor/AutoInjectHelperEditor.cs", Script_Template.AutoInjectHelperEditor_cs);

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }


    }
}
