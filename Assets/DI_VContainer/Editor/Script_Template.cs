using UnityEngine;

namespace Cosmos.DI
{
    public static class Script_Template
    {
        public const string RootScope_cs =
@"using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;

namespace Cosmos.DI
{
    public class RootScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            // GamePlayModel
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

        public const string GlobalSignalScope_cs =
@"using VContainer;
using VContainer.Unity;
using MessagePipe;

namespace Cosmos.DI
{
    public class GlobalSignalScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            // Global Signal
            // @Dont delete - for Register Global Signal
        }
    }
}";
        public const string GameplayModel_cs =
@"using VContainer;
using VContainer.Unity;
using Cosmos.Unity;

namespace Cosmos.DI
{
    public class GameplayModel : MonoSingleton<GameplayModel>, IStartable
    {
        // @Dont delete - for Register Singleton Model
        public void Start()
        {
            // @Dont delete - Singleton Model Initialize
        }
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
            var scopeObject = GameObject.Find(""RootScope"") ?? new GameObject(""RootScope"");
            scopeObject.transform.SetAsFirstSibling();
            var gamerootObject = GameObject.Find(""GameplayModel"") ?? new GameObject(""GameplayModel"");
            gamerootObject.AddComponent<GameplayModel>();
            gamerootObject.transform.SetSiblingIndex(1);

            var globalSignal = GameObject.Find(""GlobalSignalScope"") ?? new GameObject(""GlobalSignalScope"");
            globalSignal.AddComponent<GlobalSignalScope>();
            globalSignal.transform.SetSiblingIndex(2);

            var scope = scopeObject.AddComponent<RootScope>();
            scope.AddAutoInjectGameObject(gamerootObject);
        }
    }
    public class GenGamePlayModelHelperEditor : EditorWindow
    {
        static readonly string RootScopeFile = Application.dataPath + ""/DI/Runtime/RootScope.cs"";
        static readonly string GameplayModelFile = Application.dataPath + ""/Scripts/GameplayModel/GameplayModel.cs"";
        static readonly string GameplayModelFolder = Application.dataPath + ""/Scripts/GameplayModel/"";

        [MenuItem(""代码生成/生成 Singleton Model 模板代码"", false, 0)]
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
                var register = File.ReadAllText(RootScopeFile);
                register = register.Insert(register.IndexOf(""// @Dont delete - for Register Singleton Model""),
                    $""builder.Register<{className}>(Lifetime.Singleton).AsImplementedInterfaces();\r\n            "");
                File.WriteAllText(RootScopeFile, register);

                // GameplayModel
                var gameplayModel = File.ReadAllText(GameplayModelFile);
                gameplayModel = gameplayModel.Insert(gameplayModel.IndexOf(""// @Dont delete - for Register Singleton Model""),
                    $""[Inject] public I{className} {className} {{ get; set; }}\r\n        "");
                gameplayModel = gameplayModel.Insert(gameplayModel.IndexOf(""// @Dont delete - Singleton Model Initialize""),
                    $""Container.Resolve<{className}>().Initialize();\r\n            "");
                File.WriteAllText(GameplayModelFile, gameplayModel);

                AssetDatabase.Refresh();
            }
        }
        const string GameplayModel_cs =
@""using Cosmos.Unity;
public interface I{0}
{
}
public class {0} : I{0}, IGamePlayModel
{
    public void Initialize()
{
}
}
"";
    }
    public class GenGlobalSignalHelperEditor : EditorWindow
    {
        static readonly string GlobalSignalFolder = Application.dataPath + ""/Scripts/GlobalSignals/"";
        static readonly string GlobalSignalScopeFile = Application.dataPath + ""/DI/Runtime/GlobalSignalScope.cs"";


        [MenuItem(""代码生成/生成 Global Signal 模板代码"", false, 1)]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(GenGlobalSignalHelperEditor));
            window.titleContent = new GUIContent(""GlobalSignal模板代码生成器"");
        }
        static string signalClassName = "";
        private void OnGUI()
        {
            signalClassName = EditorGUILayout.TextField(""起一个响亮的名字"", signalClassName);
            if (signalClassName.IsNullOrEmpty())
            {
                GUILayout.Label(""类名不合法"");
                return;
            }
            if (GUILayout.Button(""生成!""))
            {
                var modelFile = GlobalSignal_cs.Replace(""{0}"", signalClassName);
                if (!Directory.Exists(GlobalSignalFolder)) Directory.CreateDirectory(GlobalSignalFolder);
                File.WriteAllText(GlobalSignalFolder + signalClassName + ""Signal.cs"", modelFile);

                // Register
                var register = File.ReadAllText(GlobalSignalScopeFile);
                register = register.Insert(register.IndexOf(""// @Dont delete - for Register Global Signal""),
                    $""builder.Register<{signalClassName}Signal>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();\r\n            builder.Register<{signalClassName}Command>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();\r\n            "");
                File.WriteAllText(GlobalSignalScopeFile, register);
                AssetDatabase.Refresh();
            }
        }
        const string GlobalSignal_cs =
@""using Cosmos.DI;
public class {0}Signal : BaseSingal<int> { }
public class {0}Command : BaseCommand<int>
{
    public override void Execute(int message)
    {
    }
}
"";
    }
}";

    }
}
