using System;

namespace fastCSharp.reflection
{
    /// <summary>
    /// 类型扩展操作
    /// </summary>
    internal static class type
    {
        /// <summary>
        /// 访问控制符
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>访问控制符</returns>
        internal static string getAccessDefinition(this Type type)
        {
            if (type != null)
            {
                if (type.IsNested)
                {
                    if (type.IsNestedPublic) return "public";
                    if (type.IsNestedPrivate) return "private";
                    if (type.IsNestedAssembly) return "internal";
                }
                else return type.IsPublic ? "public" : "internal";
            }
            return null;
        }
    }
}
