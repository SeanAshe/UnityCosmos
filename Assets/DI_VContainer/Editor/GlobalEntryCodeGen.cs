using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.Callbacks;

namespace Cosmos.DI
{
    /// <summary>
    /// 在脚本编译后自动扫描所有带 [GlobalEntry] 的类型，
    /// 并将注册代码写入 GlobalEntryScope.cs 源文件。
    /// 运行时零反射，所有注册代码都是编译期生成的。
    /// </summary>
    public static class GlobalEntryCodeGen
    {
        const string MarkerStart = "// [AutoGen] GlobalEntry - Start";
        const string MarkerEnd = "// [AutoGen] GlobalEntry - End";

        [DidReloadScripts]
        static void OnScriptsReloaded()
        {
            // 查找 GlobalEntryScope.cs 文件（排除 Editor 目录和模板）
            var guids = AssetDatabase.FindAssets("GlobalEntryScope");
            string scopeFilePath = null;
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith("GlobalEntryScope.cs")
                    && !path.Contains("/Editor/")
                    && !path.Contains("DI_VContainer"))
                {
                    scopeFilePath = path;
                    break;
                }
            }
            if (scopeFilePath == null) return;

            var fullPath = Path.GetFullPath(scopeFilePath);
            var content = File.ReadAllText(fullPath);

            // 确保文件包含标记
            var startIdx = content.IndexOf(MarkerStart);
            var endIdx = content.IndexOf(MarkerEnd);
            if (startIdx < 0 || endIdx < 0) return;

            // 获取缩进（取 Start 标记所在行的缩进）
            var lineStart = content.LastIndexOf('\n', startIdx) + 1;
            var indent = content.Substring(lineStart, startIdx - lineStart);

            // 获取所有带 [GlobalEntry] 的类型
            var types = TypeCache.GetTypesWithAttribute<GlobalEntryAttribute>();

            // 生成注册代码
            var sb = new StringBuilder();
            sb.Append(MarkerStart);
            sb.AppendLine();
            foreach (var type in types.OrderBy(t => t.FullName))
            {
                var typeName = string.IsNullOrEmpty(type.Namespace) ? type.Name : $"{type.Namespace}.{type.Name}";
                sb.Append(indent);
                sb.Append($"builder.Register<{typeName}>(Lifetime.Singleton).AsSelf();");
                sb.AppendLine();
            }
            sb.Append(indent);
            sb.Append(MarkerEnd);

            // 拼接新内容
            var beforeMarker = content.Substring(0, startIdx);
            var afterMarker = content.Substring(endIdx + MarkerEnd.Length);
            var newContent = beforeMarker + sb.ToString() + afterMarker;

            // 只在内容变化时写入，避免无限编译循环
            if (content == newContent) return;

            File.WriteAllText(fullPath, newContent);
            AssetDatabase.Refresh();
        }
    }
}
