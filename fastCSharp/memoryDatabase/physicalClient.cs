using System;
using System.Threading;
using fastCSharp.threading;

namespace fastCSharp.memoryDatabase
{
    /// <summary>
    /// 数据库物理层客户端接口
    /// </summary>
    public interface IPhysicalClient
    {
        /// <summary>
        /// 获取数据库物理层客户端
        /// </summary>
        /// <returns>数据库物理层客户端</returns>
        fastCSharp.tcpClient.memoryDatabasePhysical GetClient();
    }
    /// <summary>
    /// 数据库物理层客户端
    /// </summary>
    /// <typeparam name="clientType">客户端类型</typeparam>
    public class physicalClient<clientType> : IPhysicalClient, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<fastCSharp.tcpClient.memoryDatabasePhysical>
    {
        /// <summary>
        /// 数据库物理层客户端
        /// </summary>
        private static fastCSharp.tcpClient.memoryDatabasePhysical client;
        /// <summary>
        /// 数据库物理层客户端访问锁
        /// </summary>
        private static readonly object clientLock = new object();
        /// <summary>
        /// 获取数据库物理层客户端
        /// </summary>
        /// <returns>数据库物理层客户端</returns>
        protected virtual fastCSharp.tcpClient.memoryDatabasePhysical getClient()
        {
            return new fastCSharp.tcpClient.memoryDatabasePhysical(null, this);
        }
        /// <summary>
        /// 获取数据库物理层客户端
        /// </summary>
        /// <returns>数据库物理层客户端</returns>
        public fastCSharp.tcpClient.memoryDatabasePhysical GetClient()
        {
            if (client == null)
            {
                Monitor.Enter(clientLock);
                try
                {
                    if (client == null) client = getClient();
                }
                finally { Monitor.Exit(clientLock); }
            }
            return client;
        }
        /// <summary>
        /// TCP客户端验证函数
        /// </summary>
        /// <param name="client">内存数据库物理层客户端</param>
        /// <returns>是否通过验证</returns>
        public bool Verify(fastCSharp.tcpClient.memoryDatabasePhysical client)
        {
            return client.verify(fastCSharp.config.memoryDatabase.Default.PhysicalVerify).Value;
        }
        static physicalClient()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
