using System;
using fastCSharp.reflection;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql
{
    /// <summary>
    /// 加载表格缓存数据
    /// </summary>
    public static class tableLoadChecker
    {
        /// <summary>
        /// 超时检测队列
        /// </summary>
        private static readonly fastCSharp.threading.timeoutQueue timeoutQueue = fastCSharp.threading.timeoutQueue.Get(fastCSharp.config.sql.Default.TableLoadCheckTimeout);
        /// <summary>
        /// 当前未完成检测数量
        /// </summary>
        private static int checkCount;
        /// <summary>
        /// 当前未完成检测数量
        /// </summary>
        public static int CheckCount
        {
            get { return checkCount; }
        }
        /// <summary>
        /// 加载缓存数据集合
        /// </summary>
        public static subArray<object> Loaded = new subArray<object>();
        /// <summary>
        /// 添加新的检测
        /// </summary>
        /// <param name="check"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void Add(Action check)
        {
            Interlocked.Increment(ref checkCount);
            timeoutQueue.Add(check);
        }
        /// <summary>
        /// 添加新的检测
        /// </summary>
        /// <param name="check"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void ReCheck(Action check)
        {
            timeoutQueue.Add(check);
        }
        /// <summary>
        /// 检测完成
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void End()
        {
            Interlocked.Decrement(ref checkCount);
        }
    }
    /// <summary>
    /// 加载表格缓存数据
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public sealed class tableLoadChecker<valueType>
    {
        /// <summary>
        /// 是否加载完成
        /// </summary>
        private bool isLoaded;
        /// <summary>
        /// 是否检测失败输出过日志
        /// </summary>
        private bool isOutput;
        /// <summary>
        /// 创建表格对象检测初始化
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void New()
        {
            tableLoadChecker.Add(check);
            tableLoadChecker.Loaded.Add(fastCSharp.emit.constructor<valueType>.New());
            isLoaded = true;
            tableLoadChecker.End();
        }
        /// <summary>
        /// 超时检测
        /// </summary>
        private void check()
        {
            if (isLoaded)
            {
                if (isOutput)
                {
                    fastCSharp.log.Default.Add(typeof(valueType).fullName() + " loaded " + tableLoadChecker.CheckCount.toString() + " waiting", new System.Diagnostics.StackFrame(), false);
                }
            }
            else
            {
                tableLoadChecker.ReCheck(check);
                if (!isOutput)
                {
                    isOutput = true;
                    fastCSharp.log.Default.Add(typeof(valueType).fullName() + " loading ...", new System.Diagnostics.StackFrame(), false);
                }
            }
        }
    }
}
