using System;

namespace fastCSharp
{
    /// <summary>
    /// 用于HASH的字节数组
    /// </summary>
    public struct hashBytes : IEquatable<hashBytes>, IEquatable<subArray<byte>>, IEquatable<byte[]>
    {
        /// <summary>
        /// 字节数组
        /// </summary>
        private subArray<byte> data;
        /// <summary>
        /// 数组长度
        /// </summary>
        public int Length
        {
            get { return data.length; }
        }
        /// <summary>
        /// HASH值
        /// </summary>
        private int hashCode;
        /// <summary>
        /// HASH字节数组隐式转换
        /// </summary>
        /// <param name="data">字节数组</param>
        /// <returns>HASH字节数组</returns>
        public static implicit operator hashBytes(subArray<byte> data) { return new hashBytes(ref data); }
        /// <summary>
        /// HASH字节数组隐式转换
        /// </summary>
        /// <param name="data">字节数组</param>
        /// <returns>HASH字节数组</returns>
        public static implicit operator hashBytes(byte[] data)
        {
            return new hashBytes(data != null ? subArray<byte>.Unsafe(data, 0, data.Length) : default(subArray<byte>));
        }
        /// <summary>
        /// HASH字节数组隐式转换
        /// </summary>
        /// <param name="value">HASH字节数组</param>
        /// <returns>字节数组</returns>
        public static implicit operator subArray<byte>(hashBytes value) { return value.data; }
        /// <summary>
        /// 字节数组HASH
        /// </summary>
        /// <param name="data">字节数组</param>
        public unsafe hashBytes(subArray<byte> data) : this(ref data) { }
        /// <summary>
        /// 字节数组HASH
        /// </summary>
        /// <param name="data">字节数组</param>
        public unsafe hashBytes(ref subArray<byte> data)
        {
            this.data = data;
            if (data.length == 0) hashCode = 0;
            else
            {
                fixed (byte* dataFixed = data.array)
                {
                    hashCode = algorithm.hashCode.GetHashCode(dataFixed + data.startIndex, data.length) ^ random.Hash;
                }
            }
        }
        /// <summary>
        /// 获取HASH值
        /// </summary>
        /// <returns>HASH值</returns>
        public override int GetHashCode()
        {
            return hashCode;
        }
        /// <summary>
        /// 比较字节数组是否相等
        /// </summary>
        /// <param name="other">字节数组HASH</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object other)
        {
            return Equals((hashBytes)other);
            //return other != null && other.GetType() == typeof(hashBytes) && Equals((hashBytes)other);
        }
        /// <summary>
        /// 比较字节数组是否相等
        /// </summary>
        /// <param name="other">用于HASH的字节数组</param>
        /// <returns>是否相等</returns>
        public unsafe bool Equals(hashBytes other)
        {
            if (data.array == null) return other.data.array == null;
            if (other.data.array != null && ((hashCode ^ other.hashCode) | (data.length ^ other.data.length)) == 0)
            {
                if (data.array == other.data.array && data.startIndex == other.data.startIndex) return true;
                fixed (byte* dataFixed = data.array, otherDataFixed = other.data.array)
                {
                    return unsafer.memory.Equal(dataFixed + data.startIndex, otherDataFixed + other.data.startIndex, data.length);
                }
            }
            return false;
        }
        /// <summary>
        /// 比较字节数组是否相等
        /// </summary>
        /// <param name="other">用于HASH的字节数组</param>
        /// <returns>是否相等</returns>
        public unsafe bool Equals(subArray<byte> other)
        {
            if (data.array == null) return other.array == null;
            if (other.array != null && data.length == other.length)
            {
                if (data.array == other.array && data.startIndex == other.startIndex) return true;
                fixed (byte* dataFixed = data.array, otherDataFixed = other.array)
                {
                    return unsafer.memory.Equal(dataFixed + data.startIndex, otherDataFixed + other.startIndex, data.length);
                }
            }
            return false;
        }
        /// <summary>
        /// 比较字节数组是否相等
        /// </summary>
        /// <param name="other">用于HASH的字节数组</param>
        /// <returns>是否相等</returns>
        public unsafe bool Equals(byte[] other)
        {
            if (data.array == null) return other == null;
            if (other != null && data.length == other.Length)
            {
                if (data.array == other && data.startIndex == 0) return true;
                fixed (byte* dataFixed = data.array) return unsafer.memory.Equal(other, dataFixed + data.startIndex, data.length);
            }
            return false;
        }
        /// <summary>
        /// 复制HASH字节数组
        /// </summary>
        /// <returns>HASH字节数组</returns>
        public hashBytes Copy()
        {
            if (this.data.length == 0)
            {
                return new hashBytes { data = subArray<byte>.Unsafe(nullValue<byte>.Array, 0, 0), hashCode = hashCode };
            }
            byte[] data = new byte[this.data.length];
            Buffer.BlockCopy(this.data.array, this.data.startIndex, data, 0, data.Length);
            return new hashBytes { data = subArray<byte>.Unsafe(data, 0, data.Length), hashCode = hashCode };
        }
    }
}
