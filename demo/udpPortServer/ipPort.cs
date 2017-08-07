using System;
using System.Net;

namespace fastCSharp.demo.udpPortServer
{
    /// <summary>
    /// IP端口信息
    /// </summary>
    [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
    public struct ipPort : IEquatable<ipPort>
    {
        /// <summary>
        /// IPv4地址
        /// </summary>
        public uint Ip;
        /// <summary>
        /// 端口地址
        /// </summary>
        public int Port;
        /// <summary>
        /// 是否为空数据
        /// </summary>
        public bool IsNull
        {
            get { return ((int)Ip | Port) == 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)Ip ^ Port;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ipPort other)
        {
            return (((int)Ip ^ (int)other.Ip) | (Port ^ other.Port)) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals((ipPort)obj);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="ip">IP端口信息</param>
        /// <returns>IP端口信息</returns>
        public static implicit operator ipPort(IPEndPoint ip)
        {
#pragma warning disable 618
            return new ipPort { Ip = (uint)ip.Address.Address, Port = ip.Port };
#pragma warning restore 618
        }
    }
}
