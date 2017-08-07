using System;
using System.Net;
using fastCSharp.reflection;

namespace fastCSharp.net
{
    /// <summary>
    /// IPv6地址哈希
    /// </summary>
    public struct ipv6Hash : IEquatable<ipv6Hash>
    {
        /// <summary>
        /// IPv6地址
        /// </summary>
        private static readonly Func<IPAddress, ushort[]> ipAddress = fastCSharp.emit.pub.GetField<IPAddress, ushort[]>("m_Numbers");
        /// <summary>
        /// IP地址
        /// </summary>
        internal ushort[] Ip;
        /// <summary>
        /// 哈希值
        /// </summary>
        internal int HashCode;
        /// <summary>
        /// IPv6地址哈希
        /// </summary>
        /// <param name="ip"></param>
        public unsafe ipv6Hash(IPAddress ip)
        {
            if ((Ip = ipAddress(ip)) == null) HashCode = 0;
            else
            {
                fixed (ushort* ipFixed = Ip)
                {
                    HashCode = * (int*)ipFixed ^ *(int*)(ipFixed + 2) ^ *(int*)(ipFixed + 4) ^ *(int*)(ipFixed + 6) ^ random.Hash;
                }
            }
        }
        /// <summary>
        /// IPv6地址哈希隐式转换
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>IPv6地址哈希</returns>
        public static implicit operator ipv6Hash(IPAddress ip)
        {
            return new ipv6Hash(ip);
        }
        /// <summary>
        /// 设置为空值
        /// </summary>
        internal void Null()
        {
            Ip = null;
            HashCode = 0;
        }
        /// <summary>
        /// IPv6地址哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override unsafe int GetHashCode()
        {
            return HashCode;
        }
        /// <summary>
        /// IPv6地址哈希是否相等
        /// </summary>
        /// <param name="obj">IPv6地址哈希</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            return Equals((ipv6Hash)obj);
            //return obj != null & obj.GetType() == typeof(ipv6Hash) && Equals((ipv6Hash)obj);
        }
        /// <summary>
        /// IPv6地址哈希是否相等
        /// </summary>
        /// <param name="other">IPv6地址哈希</param>
        /// <returns>是否相等</returns>
        public unsafe bool Equals(ipv6Hash other)
        {
            fixed (ushort* ipFixed = Ip, otherFixed = other.Ip)
            {
                if (*(int*)ipFixed == *(int*)otherFixed)
                {
                    return ((*(int*)(ipFixed + 2) ^ *(int*)(otherFixed + 2))
                        | (*(int*)(ipFixed + 4) ^ *(int*)(otherFixed + 4))
                        | (*(int*)(ipFixed + 6) ^ *(int*)(otherFixed + 6))) == 0;
                }
            }
            return false;
        }
    }
}
