using System;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// HTTP响应Cookie
    /// </summary>
    //[fastCSharp.code.cSharp.serialize(IsReferenceMember = false)]
    public sealed partial class cookie
    {
        /// <summary>
        /// 名称
        /// </summary>
        public byte[] Name;
        /// <summary>
        /// 值
        /// </summary>
        public byte[] Value;
        /// <summary>
        /// 有效域名
        /// </summary>
        public subArray<byte> Domain;
        /// <summary>
        /// 有效路径
        /// </summary>
        public byte[] Path;
        /// <summary>
        /// 超时时间
        /// </summary>
        public DateTime Expires = DateTime.MinValue;
        /// <summary>
        /// 是否安全
        /// </summary>
        public bool IsSecure;
        /// <summary>
        /// 是否HTTP Only
        /// </summary>
        public bool IsHttpOnly;
        /// <summary>
        /// HTTP响应Cookie
        /// </summary>
        public cookie() { }
        /// <summary>
        /// HTTP响应Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        public cookie(string name, string value)
        {
            if (!string.IsNullOrEmpty(name)) Name = name.getBytes();
            if (!string.IsNullOrEmpty(value)) Value = value.getBytes();
        }
        /// <summary>
        /// HTTP响应Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="domain">有效域名</param>
        /// <param name="path">有效路径</param>
        /// <param name="isSecure">是否安全</param>
        /// <param name="isHttpOnly">是否HTTP Only</param>
        public cookie(string name, string value, string domain, string path, bool isSecure, bool isHttpOnly)
        {
            if (!string.IsNullOrEmpty(name)) Name = name.getBytes();
            if (!string.IsNullOrEmpty(value)) Value = value.getBytes();
            if (!string.IsNullOrEmpty(domain))
            {
                byte[] data = domain.getBytes();
                Domain = subArray<byte>.Unsafe(data, 0, data.Length);
            }
            if (!string.IsNullOrEmpty(path)) Path = path.getBytes();
            IsSecure = isSecure;
            IsHttpOnly = isHttpOnly;
        }
        /// <summary>
        /// HTTP响应Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="expires">超时时间,DateTime.MinValue表示忽略</param>
        /// <param name="domain">有效域名</param>
        /// <param name="path">有效路径</param>
        /// <param name="isSecure">是否安全</param>
        /// <param name="isHttpOnly">是否HTTP Only</param>
        public cookie(string name, string value, DateTime expires
            , string domain, string path, bool isSecure, bool isHttpOnly)
            : this(name, value, domain, path, isSecure, isHttpOnly)
        {
            Expires = expires;
        }
        /// <summary>
        /// HTTP响应Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="expires">超时时间,DateTime.MinValue表示忽略</param>
        /// <param name="domain">有效域名</param>
        /// <param name="path">有效路径</param>
        /// <param name="isSecure">是否安全</param>
        /// <param name="isHttpOnly">是否HTTP Only</param>
        internal cookie(byte[] name, byte[] value, DateTime expires, subArray<byte> domain, byte[] path, bool isSecure, bool isHttpOnly)
        {
            Name = name;
            Value = value;
            Expires = expires;
            Domain = domain;
            Path = path;
            IsSecure = isSecure;
            IsHttpOnly = isHttpOnly;
        }
    }
}
