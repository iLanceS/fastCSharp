using System;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 字符串Hash
    /// </summary>
    public struct hashString : IEquatable<hashString>, IEquatable<subString>, IEquatable<string>
    {
        /// <summary>
        /// 字符串
        /// </summary>
        private subString value;
        /// <summary>
        /// 字符串长度
        /// </summary>
        internal int Length
        {
            get { return value.Length; }
        }
        /// <summary>
        /// 哈希值
        /// </summary>
        private int hashCode;
        /// <summary>
        /// 清空数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void Null()
        {
            value.Null();
            hashCode = 0;
        }
        /// <summary>
        /// 字符串Hash
        /// </summary>
        /// <param name="value"></param>
        public hashString(subString value)
            : this(ref value)
        { }
        /// <summary>
        /// 字符串Hash
        /// </summary>
        /// <param name="value"></param>
        public hashString(ref subString value)
        {
            this.value = value;
            hashCode = value == null ? 0 : (value.GetHashCode() ^ random.Hash);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>字符串</returns>
        public static implicit operator hashString(string value) { return new hashString(value); }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>字符串</returns>
        public static implicit operator hashString(subString value) { return new hashString(ref value); }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>字符串</returns>
        public static implicit operator subString(hashString value) { return value.value; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Equals(hashString other)
        {
            return hashCode == other.hashCode && value.Equals(other.value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Equals(subString other)
        {
            return value.Equals(other);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Equals(string other)
        {
            return value.Equals(other);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return hashCode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals((hashString)obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return value == null ? null : value.ToString();
        }
    }
}
