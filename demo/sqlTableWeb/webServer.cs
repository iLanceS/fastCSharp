using System;
using System.Threading;
using fastCSharp.net.tcp.http;

namespace fastCSharp.demo.sqlTableWeb
{
    /// <summary>
    /// web视图服务
    /// </summary>
    public partial class webServer
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="domains"></param>
        /// <param name="onStop"></param>
        /// <returns></returns>
        public override bool Start(domain[] domains, Action onStop)
        {
            if (base.Start(domains, onStop))
            {
                fastCSharp.threading.threadPool.TinyPool.Start(loadSqlClientCache);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否已经加载数据库客户端缓存
        /// </summary>
        private bool isSqlClientCache;
        /// <summary>
        /// 数据库客户端缓存初始化
        /// </summary>
        private void loadSqlClientCache()
        {
            Monitor.Enter(fastCSharp.demo.sqlTableCacheServer.clientCache.CacheLock);
            try
            {
                if (!isSqlClientCache)
                {
                    string cachePath = fastCSharp.config.pub.Default.CachePath + @"sqlStreamCache\";
                    fastCSharp.demo.sqlTableCacheServer.clientCache.Class = fastCSharp.demo.sqlTableCacheServer.clientCache.Class.CreateNull(typeof(fastCSharp.demo.sqlTableCacheServer.tcpCall.Class), cachePath);
                    fastCSharp.demo.sqlTableCacheServer.clientCache.Student = fastCSharp.demo.sqlTableCacheServer.clientCache.Student.CreateNull(typeof(fastCSharp.demo.sqlTableCacheServer.tcpCall.Student), cachePath);
                    isSqlClientCache = true;
                }
            }
            finally { Monitor.Exit(fastCSharp.demo.sqlTableCacheServer.clientCache.CacheLock); }
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        protected override void stopListen()
        {
            if (isSqlClientCache)
            {
                isSqlClientCache = false;
                fastCSharp.demo.sqlTableCacheServer.clientCache.CheckSave();
            }
        }
    }
}
