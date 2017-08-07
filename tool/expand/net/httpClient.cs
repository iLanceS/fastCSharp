using System;
using System.Net;
using fastCSharp.threading;
using System.Threading;

namespace fastCSharp.net
{
    /// <summary>
    /// HTTP客户端
    /// </summary>
    internal static class httpClient
    {
        /// <summary>
        /// IP地址信息
        /// </summary>
        private struct ipAddress
        {
            /// <summary>
            /// 超时时间
            /// </summary>
            public DateTime Timeout;
            /// <summary>
            /// IP地址
            /// </summary>
            public IPAddress[] Ips;
            /// <summary>
            /// 域名字符串
            /// </summary>
            public string Domain;
        }
        /// <summary>
        /// 域名转换IP地址集合
        /// </summary>
        private static readonly fifoPriorityQueue<hashBytes, ipAddress> domainIps = new fifoPriorityQueue<hashBytes, ipAddress>();
        /// <summary>
        /// 域名转换IP地址访问锁
        /// </summary>
        private static readonly object domainIpLock = new object();
        /// <summary>
        /// 域名转IP地址缓存超时时钟周期
        /// </summary>
        private static readonly long domainIpTimeoutTicks = new TimeSpan(0, config.http.Default.IpAddressTimeoutMinutes, 0).Ticks;
        /// <summary>
        /// 清除域名转换IP地址集合
        /// </summary>
        public static void ClearDomainIPAddress()
        {
            Monitor.Enter(domainIpLock);
            domainIps.Clear();
            Monitor.Exit(domainIpLock);
        }
        /// <summary>
        /// 设置域名转换IP地址
        /// </summary>
        /// <param name="key">域名</param>
        /// <param name="ipAddress">IP地址</param>
        private static void setDomainIp(hashBytes key, ref ipAddress ipAddress)
        {
            Monitor.Enter(domainIpLock);
            try
            {
                domainIps.Set(key, ipAddress);
                if (domainIps.Count == config.http.Default.IpAddressCacheCount) domainIps.UnsafePopValue();
            }
            finally { Monitor.Exit(domainIpLock); }
        }
        /// <summary>
        /// 根据域名获取IP地址
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>IP地址,失败返回null</returns>
        internal unsafe static IPAddress[] GetIPAddress(ref subArray<byte> domain)
        {
            try
            {
                fixed (byte* domainFixed = domain.UnsafeArray)
                {
                    byte* domainStart = domainFixed + domain.StartIndex;
                    unsafer.memory.ToLower(domainStart, domainStart + domain.Count);
                    hashBytes key = domain;
                    ipAddress value;
                    Monitor.Enter(domainIpLock);
                    try
                    {
                        value = domainIps.Get(key, default(ipAddress));
                        if (value.Ips != null && value.Timeout < date.NowSecond)
                        {
                            domainIps.Remove(key, out value);
                            value.Ips = null;
                        }
                    }
                    finally { Monitor.Exit(domainIpLock); }
                    if (value.Ips == null)
                    {
                        if (value.Domain == null) value.Domain = String.UnsafeDeSerialize(domainStart, -domain.Count);
                        IPAddress ip;
                        if (IPAddress.TryParse(value.Domain, out ip))
                        {
                            value.Timeout = DateTime.MaxValue;
                            value.Domain = null;
                            setDomainIp(key.Copy(), ref value);
                            return value.Ips = new IPAddress[] { ip };
                        }
                        value.Ips = Dns.GetHostEntry(value.Domain).AddressList;
                        if (value.Ips.Length != 0)
                        {
                            value.Timeout = date.NowSecond.AddTicks(domainIpTimeoutTicks);
                            setDomainIp(key.Copy(), ref value);
                            return value.Ips;
                        }
                    }
                    else return value.Ips;
                }
            }
            catch (Exception error)
            {
                log.Default.Add(error, null, false);
            }
            return null;
        }
        static httpClient()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
