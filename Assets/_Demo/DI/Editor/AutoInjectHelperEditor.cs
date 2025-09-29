using UnityEditor;
using UnityEngine;
using System.IO;
using Cosmos.System;

namespace Cosmos.DI
{
    public class AutoInjectHelperEditor : EditorWindow
    {
        [MenuItem("DI/VContainer Initialize", false, 1)]
        public static void VContainerInitializeEditor()
        {
            var scopeObject = GameObject.Find("RootScope") ?? new GameObject("RootScope");
            scopeObject.transform.SetAsFirstSibling();
            var gamerootObject = GameObject.Find("GameplayModel") ?? new GameObject("GameplayModel");
            gamerootObject.AddComponent<GameplayModel>();
            gamerootObject.transform.SetSiblingIndex(1);

            var globalSignal = GameObject.Find("GlobalSignalScope") ?? new GameObject("GlobalSignalScope");
            globalSignal.AddComponent<GlobalSignalScope>();
            globalSignal.transform.SetSiblingIndex(2);

            var scope = scopeObject.AddComponent<RootLifetimeScope>();
            scope.AddAutoInjectGameObject(gamerootObject);
        }
    }
    public class GenGamePlayModelHelperEditor : EditorWindow
    {
        static readonly string RootScopeFile = Application.dataPath + "/_Demo/DI/Runtime/RootScope.cs";
        static readonly string GameplayModelFile = Application.dataPath + "/_Demo/GameplayModel/GameplayModel.cs";
        static readonly string GameplayModelFolder = Application.dataPath + "/_Demo/GameplayModel/";

        [MenuItem("代码生成/生成 Singleton Model 模板代码", false, 0)]
        public static void ShowGenModelWindow()
        {
            var window = GetWindow(typeof(GenGamePlayModelHelperEditor));
            window.titleContent = new GUIContent("GameplayModel模板代码生成器");
        }
        static string modelClassName = "";
        private void OnGUI()
        {
            modelClassName = EditorGUILayout.TextField("起一个响亮的名字", modelClassName);
            if (modelClassName.IsNullOrEmpty())
            {
                GUILayout.Label("类名不合法");
                return;
            }
            if (GUILayout.Button("生成!"))
            {
                var modelFile = GameplayModel_cs.Replace("{0}", modelClassName);
                File.WriteAllText(GameplayModelFolder + modelClassName + ".cs", modelFile);

                // Register
                var register = File.ReadAllText(RootScopeFile);
                register = register.Insert(register.IndexOf("// @Dont delete - for Register Singleton Model"),
                    $"builder.Register<{modelClassName}>(Lifetime.Singleton).AsImplementedInterfaces();\r\n            ");
                File.WriteAllText(RootScopeFile, register);

                // GameplayModel
                var gameplayModel = File.ReadAllText(GameplayModelFile);
                gameplayModel = gameplayModel.Insert(gameplayModel.IndexOf("// @Dont delete - for Register Singleton Model"),
                    $"[Inject] public I{modelClassName} {modelClassName} {{ get; set; }}\r\n        ");
                gameplayModel = gameplayModel.Insert(gameplayModel.IndexOf("// @Dont delete - Singleton Model Initialize"),
                    $"{modelClassName}.Initialize();\r\n            ");
                File.WriteAllText(GameplayModelFile, gameplayModel);
                AssetDatabase.Refresh();
            }
        }
        const string GameplayModel_cs =
@"using Cosmos.Unity;
public interface I{0}: IGamePlayModel
{
}
public class {0} : I{0}
{
    public void Initialize()
    {
    }
}
";
    }
    public class GenGlobalSignalHelperEditor : EditorWindow
    {
        static readonly string GlobalSignalFolder = Application.dataPath + "/_Demo/GlobalSignals/";
        static readonly string GlobalSignalScopeFile = Application.dataPath + "/_Demo/DI/Runtime/GlobalSignalScope.cs";


        [MenuItem("代码生成/生成 Global Signal 模板代码", false, 1)]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(GenGlobalSignalHelperEditor));
            window.titleContent = new GUIContent("GlobalSignal模板代码生成器");
        }
        static string signalClassName = "";
        private void OnGUI()
        {
            signalClassName = EditorGUILayout.TextField("起一个响亮的名字", signalClassName);
            if (signalClassName.IsNullOrEmpty())
            {
                GUILayout.Label("类名不合法");
                return;
            }
            if (GUILayout.Button("生成!"))
            {
                var modelFile = GlobalSignal_cs.Replace("{0}", signalClassName);
                if (!Directory.Exists(GlobalSignalFolder)) Directory.CreateDirectory(GlobalSignalFolder);
                File.WriteAllText(GlobalSignalFolder + signalClassName + "Signal.cs", modelFile);

                // Register
                var register = File.ReadAllText(GlobalSignalScopeFile);
                register = register.Insert(register.IndexOf("// @Dont delete - for Register Global Signal"),
                    $"builder.Register<{signalClassName}Signal>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();\r\n            builder.Register<{signalClassName}Command>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();\r\n            ");
                File.WriteAllText(GlobalSignalScopeFile, register);
                AssetDatabase.Refresh();
            }
        }
        const string GlobalSignal_cs =
@"using Cosmos.DI;
public class {0}Signal : BaseSingal<int> { }
public class {0}Command : BaseCommand<int>
{
    public override void Execute(int message)
    {
    }
}
";
    }
}