using System;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 内存检测
    /// </summary>
    public static class checkMemory
    {
        /// <summary>
        /// 检测类型集合
        /// </summary>
        private static arrayPool<Type> pool = arrayPool<Type>.Create();
        /// <summary>
        /// 添加类型
        /// </summary>
        /// <param name="type"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void Add(Type type)
        {
            pool.Push(type);
        }
        /// <summary>
        /// 获取检测类型集合
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<Type> GetTypes()
        {
            int count = pool.Count;
            return subArray<Type>.Unsafe(pool.Array, 0, count);
        }
    }
}
