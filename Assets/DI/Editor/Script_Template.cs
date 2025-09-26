using UnityEngine;

namespace Cosmos.DI
{
    public static class Script_Template
    {
        public const string RootLifetimeScope_cs =
@"using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;

namespace Cosmos.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // @Dont delete - for Register Singleton Model
        }
        public void AddAutoInjectGameObject(GameObject gameObject)
        {
            autoInjectGameObjects ??= new List<GameObject>();
            autoInjectGameObjects.Add(gameObject);
        }
    }
}
";
        public const string GameRoot_cs =
@"using VContainer;
using VContainer.Unity;
using Cosmos.Unity;

namespace Cosmos.DI
{
    public class GameRoot : MonoSingleton<GameRoot>
    {
        // @Dont delete - for Register Singleton Model
    }
}
";
        public const string AutoInjectHelperEditor_cs =
@"using UnityEditor;
using UnityEngine;
using System.IO;
using Cosmos.System;

namespace Cosmos.DI
{
    public class AutoInjectHelperEditor : EditorWindow
    {
        [MenuItem(""DI/VContainer Initialize"", false, 1)]
        public static void VContainerInitializeEditor()
        {
            var scopeObject = GameObject.Find(""RootLifetimeScope"") ?? new GameObject(""RootLifetimeScope"");
            scopeObject.transform.SetAsFirstSibling();
            var gamerootObject = GameObject.Find(""GameRoot"") ?? new GameObject(""GameRoot"");
            gamerootObject.AddComponent<GameRoot>();

            var scope = scopeObject.AddComponent<RootLifetimeScope>();
            scope.AddAutoInjectGameObject(gamerootObject);
        }
        static readonly string RootLifetimeScopeFile = Application.dataPath + ""/DIRefrences/RootLifetimeScope.cs"";
        static readonly string GameRoot = Application.dataPath + ""/DIRefrences/GameRoot.cs"";
        static readonly string GameplayModelFolder = Application.dataPath + ""/_Demo/GameplayModel/"";

        [MenuItem(""DI/生成 Singleton Model 模板代码"", false, 2)]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(AutoInjectHelperEditor));
            window.titleContent = new GUIContent(""GameplayModel模板代码生成器"");
        }
        static string className = """";
        private void OnGUI()
        {
            className = EditorGUILayout.TextField(""起一个响亮的名字"", className);
            if (className.IsNullOrEmpty())
            {
                GUILayout.Label(""类名不合法"");
                return;
            }
            if (GUILayout.Button(""生成!""))
            {
                var modelFile = GameplayModel_cs.Replace(""{0}"", className);
                File.WriteAllText(GameplayModelFolder + className + "".cs"", modelFile);

                // Register
                var register = File.ReadAllText(RootLifetimeScopeFile);
                register = register.Insert(register.IndexOf(""// @Dont delete - for Register Singleton Model""),
                    $""builder.Register<{className}>(Lifetime.Singleton).AsImplementedInterfaces();r\n            ""\);
                File.WriteAllText(RootLifetimeScopeFile, register);

                // GameRoot
                var gameRoot = File.ReadAllText(GameRoot);
                gameRoot = gameRoot.Insert(gameRoot.IndexOf(""// @Dont delete - for Register Singleton Model""),
                    $""[Inject] public I{className} {className} {{ get; set; }}r\n        "");
                File.WriteAllText(GameRoot, gameRoot);

                AssetDatabase.Refresh();
            }
        }
        const string GameplayModel_cs =
@""using Cosmos.Unity;
public interface I {0}
{
}
public class {0} : IGamePlayModel
{
    public void Initialize()
{
}
}
"";
    }
}";


    }
}
