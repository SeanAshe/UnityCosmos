using System;

namespace Cosmos.DI
{
    /// <summary>
    /// 标记一个类以自动注册到 GlobalEntryScope 容器中。
    /// 编译后 Editor 会自动扫描此 Attribute，将注册代码写入 GlobalEntryScope.cs。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GlobalEntryAttribute : Attribute { }
}
