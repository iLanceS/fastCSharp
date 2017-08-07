using System;
using System.Net;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// TCP服务端口信息
    /// </summary>
    [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
    public struct host : IEquatable<host>
    {
        /// <summary>
        /// 主机名称或者IP地址
        /// </summary>
        public string Host;
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port;
        /// <summary>
        /// TCP服务端口信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        internal void Set(string host, int port)
        {
            Host = host;
            Port = port;
        }
        /// <summary>
        /// 主机名称转换成IP地址
        /// </summary>
        /// <returns>是否转换成功</returns>
        public bool HostToIpAddress()
        {
            IPAddress ipAddress = fastCSharp.code.cSharp.tcpBase.HostToIpAddress(Host);
            if (ipAddress == null) return false;
            Host = ipAddress.ToString();
            return true;
        }
        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return Host == null ? Port : (Host.GetHashCode() ^ Port);
        }
        /// <summary>
        /// 判断是否TCP服务端口信息
        /// </summary>
        /// <param name="other">TCP服务端口信息</param>
        /// <returns>是否同一TCP服务端口信息</returns>
        public override bool Equals(object other)
        {
            return Equals((host)other);
            //return other != null && other.GetType() == typeof(host) && Equals((host)other);
        }
        /// <summary>
        /// 判断是否TCP服务端口信息
        /// </summary>
        /// <param name="other">TCP服务端口信息</param>
        /// <returns>是否同一TCP服务端口信息</returns>
        public bool Equals(host other)
        {
            return Host == other.Host && Port == other.Port;
        }
        /// <summary>
        /// 判断是否TCP服务端口信息
        /// </summary>
        /// <param name="other">TCP服务端口信息</param>
        /// <returns>是否同一TCP服务端口信息</returns>
        public bool Equals(ref host other)
        {
            return Host == other.Host && Port == other.Port;
        }
        /// <summary>
        /// TCP服务端口信息
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static host FromDomain(string domain)
        {
            int index = domain.LastIndexOf(':');
            if (index != -1)
            {
                ushort port;
                if (ushort.TryParse(domain.Substring(index + 1), out port)) return new host { Host = domain.Substring(0, index), Port = (int)(uint)port };
            }
            return new host { Host = domain, Port = 80 };
        }
    }
}
