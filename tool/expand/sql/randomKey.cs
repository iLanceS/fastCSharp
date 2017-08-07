using System;

namespace fastCSharp.sql
{
    /// <summary>
    /// 随机防HASH构造关键字
    /// </summary>
    /// <typeparam name="keyType"></typeparam>
    public struct randomKey<keyType> : IEquatable<randomKey<keyType>>
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public keyType Key;
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator randomKey<keyType>(keyType value)
        {
            return new randomKey<keyType> { Key = value };
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator keyType(randomKey<keyType> value)
        {
            return value.Key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ random.Hash;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(randomKey<keyType> other)
        {
            return Key.Equals(other.Key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(keyType other)
        {
            return Key.Equals(other);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals((randomKey<keyType>)obj);
        }
    }
}
