using System;
using System.Runtime.CompilerServices;

namespace fastCSharp.net
{
    /// <summary>
    /// 索引标识
    /// </summary>
    [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
    public struct indexIdentity
    {
        /// <summary>
        /// 错误索引
        /// </summary>
        public const int ErrorIndex = int.MinValue;
        /// <summary>
        /// 索引位置
        /// </summary>
        public int Index;
        /// <summary>
        /// 索引编号
        /// </summary>
        public int Identity;
        /// <summary>
        /// 索引是否有效
        /// </summary>
        public bool IsValid
        {
            get { return Index != ErrorIndex; }
        }
        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="value">比较值</param>
        /// <returns>0表示相等</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int Equals(indexIdentity value)
        {
            return (Index ^ value.Index) | (Identity ^ value.Identity);
        }
        /// <summary>
        /// 索引无效是设置索引
        /// </summary>
        /// <param name="value">目标值</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool SetIfNull(indexIdentity value)
        {
            if (Index == ErrorIndex)
            {
                Index = value.Index;
                Identity = value.Identity;
                return true;
            }
            return false;
        }
    }
}
