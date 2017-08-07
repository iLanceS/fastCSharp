using System;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// 域名信息
    /// </summary>
    [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
    public sealed class domain : IEquatable<domain>
    {
        /// <summary>
        /// 域名
        /// </summary>
        internal byte[] DomainData;
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain
        {
            set { DomainData = value.getBytes(); }
        }
        /// <summary>
        /// TCP服务端口信息
        /// </summary>
        public host Host;
        /// <summary>
        /// 安全连接服务端口信息
        /// </summary>
        public host SslHost;
        /// <summary>
        /// HASH值
        /// </summary>
        [fastCSharp.code.ignore]
        private int hashCode;
        /// <summary>
        /// 域名是否全名,否则表示泛域名后缀
        /// </summary>
        public bool IsFullName = true;
        /// <summary>
        /// 是否仅用于内网IP映射
        /// </summary>
        public bool IsOnlyHost;
        ///// <summary>
        ///// 域名信息
        ///// </summary>
        //public domain() { }
        ///// <summary>
        ///// 域名信息
        ///// </summary>
        ///// <param name="domain">域名</param>
        ///// <param name="host">TCP服务端口信息</param>
        ///// <param name="isFullName">域名是否全名,否则表示泛域名后缀</param>
        //public domain(string domain, host host, bool isFullName = true)
        //{
        //    DomainData = domain.getBytes();
        //    Host = host;
        //    IsFullName = isFullName;
        //}
        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            if (hashCode == 0)
            {
                hashCode = Host.GetHashCode() ^ SslHost.GetHashCode() ^ algorithm.hashCode.GetHashCode(DomainData);
                if (IsFullName) hashCode ^= 1 << 30;
                if (IsOnlyHost) hashCode ^= 1 << 29;
                if (hashCode == 0) hashCode = int.MaxValue;
            }
            return hashCode;
        }
        /// <summary>
        /// 判断是否TCP服务端口信息
        /// </summary>
        /// <param name="other">TCP服务端口信息</param>
        /// <returns>是否同一TCP服务端口信息</returns>
        public override bool Equals(object other)
        {
            return Equals((domain)other);
        }
        /// <summary>
        /// 判断是否TCP服务端口信息
        /// </summary>
        /// <param name="other">TCP服务端口信息</param>
        /// <returns>是否同一TCP服务端口信息</returns>
        public bool Equals(domain other)
        {
            return other != null && GetHashCode() == other.GetHashCode()
                && (IsFullName ? other.IsFullName : !other.IsFullName)
                && (IsOnlyHost ? other.IsOnlyHost : !other.IsOnlyHost)
                && DomainData.equal(other.DomainData) && Host.Equals(other.Host) && SslHost.Equals(other.SslHost);
        }
    }
}
