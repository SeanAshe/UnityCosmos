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
        static readonly string RootScopeFile = Application.dataPath + "/_Demo/DI/Runtime/RootScope.cs";
        static readonly string GameplayModelFile = Application.dataPath + "/_Demo/GameplayModel/GameplayModel.cs";
        static readonly string GameplayModelFolder = Application.dataPath + "/_Demo/GameplayModel/";

        [MenuItem("DI/生成 Singleton Model 模板代码", false, 2)]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(AutoInjectHelperEditor));
            window.titleContent = new GUIContent("GameplayModel模板代码生成器");
        }
        static string className = "";
        private void OnGUI()
        {
            className = EditorGUILayout.TextField("起一个响亮的名字", className);
            if (className.IsNullOrEmpty())
            {
                GUILayout.Label("类名不合法");
                return;
            }
            if (GUILayout.Button("生成!"))
            {
                var modelFile = GameplayModel_cs.Replace("{0}", className);
                File.WriteAllText(GameplayModelFolder + className + ".cs", modelFile);

                // Register
                var register = File.ReadAllText(RootScopeFile);
                register = register.Insert(register.IndexOf("// @Dont delete - for Register Singleton Model"),
                    $"builder.Register<{className}>(Lifetime.Singleton).AsImplementedInterfaces();\r\n            ");
                File.WriteAllText(RootScopeFile, register);

                // GameplayModel
                var gameplayModel = File.ReadAllText(GameplayModelFile);
                gameplayModel = gameplayModel.Insert(gameplayModel.IndexOf("// @Dont delete - for Register Singleton Model"),
                    $"[Inject] public I{className} {className} {{ get; set; }}\r\n        ");
                gameplayModel = gameplayModel.Insert(gameplayModel.IndexOf("// @Dont delete - Singleton Model Initialize"),
                    $"Container.Resolve<{className}>().Initialize();\r\n            ");
                File.WriteAllText(GameplayModelFile, gameplayModel);
                AssetDatabase.Refresh();
            }
        }
        const string GameplayModel_cs =
@"using Cosmos.Unity;
public interface I{0}
{
}
public class {0} : I{0}, IGamePlayModel
{
    public void Initialize()
    {
    }
}
";
    }
}