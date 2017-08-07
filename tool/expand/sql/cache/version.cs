using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.sql.cache
{
    /// <summary>
    /// 数据版本号
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public struct version<valueType>
    {
        /// <summary>
        /// 版本号
        /// </summary>
        internal volatile uint Version;
        /// <summary>
        /// 数据类型
        /// </summary>
        public valueType Value { get; internal set; }
        /// <summary>
        /// 判断版本号是否有效
        /// </summary>
        /// <param name="version"></param>
        /// <returns>0表示有效</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal uint IsVersion(uint version)
        {
            return (version ^ Version) | (version & 1);
        }
        /// <summary>
        /// 等待数据修改完毕
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void Wait()
        {
            while ((Version & 1) != 0)
            {
                Thread.Yield();
                if ((Version & 1) == 0) return;
                Thread.Sleep(0);
            }
        }
        /// <summary>
        /// 设置数据并设置为修改状态
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void Set(valueType value)
        {
            ++Version;
            Value = value;
        }
        /// <summary>
        /// 设置数据并升级版本号
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void SetVersion(valueType value)
        {
            Value = value;
            Version += 2;
        }
        /// <summary>
        /// 获取数据+版本号
        /// </summary>
        /// <param name="value"></param>
        /// <param name="version"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void Set(valueType value, uint version)
        {
            Value = value;
            Version = version;
        }
        /// <summary>
        /// 获取数据+版本号
        /// </summary>
        /// <param name="value"></param>
        internal void Get(ref version<valueType> value)
        {
            uint version = Version;
            while ((version & 1) != 0)
            {
                Thread.Yield();
                if (((version = Version) & 1) == 0) break;
                Thread.Sleep(0);
            }
            value.Set(Value, version);
        }
    }
}
