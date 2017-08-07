using System;
using System.Reflection;
using fastCSharp;

namespace fastCSharp.config
{
    /// <summary>
    /// SQL相关参数
    /// </summary>
    public sealed class sql : database
    {
        /// <summary>
        /// 是否忽略 fastCSharp 程序集（仅用于 Demo）
        /// </summary>
        internal bool IgnoreConnectionFastCSharp = false;
        /// <summary>
        /// 检测链接类型集合
        /// </summary>
        private string[] checkConnection = null;
        /// <summary>
        /// 检测链接类型集合
        /// </summary>
        public string[] CheckConnection
        {
            get
            {
                return checkConnection ?? nullValue<string>.Array;
            }
        }
        /// <summary>
        /// 数据更新客户端链接类型集合
        /// </summary>
        private string[] checkQueueClient = null;
        /// <summary>
        /// 数据更新客户端链接类型集合
        /// </summary>
        public string[] CheckQueueClient
        {
            get
            {
                return checkQueueClient ?? nullValue<string>.Array;
            }
        }
        /// <summary>
        /// 缓存默认最大容器大小
        /// </summary>
        private int cacheMaxCount = 1 << 10;
        /// <summary>
        /// 缓存默认最大容器大小
        /// </summary>
        public int CacheMaxCount
        {
            get { return cacheMaxCount; }
        }
        /// <summary>
        /// 默认更新队列容器大小
        /// </summary>
        private int defaultUpdateQueueSize = 100000;
        /// <summary>
        /// 默认更新队列容器大小
        /// </summary>
        public int DefaultUpdateQueueSize
        {
            get { return defaultUpdateQueueSize <= 4 ? 100000 : defaultUpdateQueueSize; }
        }
        /// <summary>
        /// 更新队列超时
        /// </summary>
        private int updateQueueTimeoutSeconds = 60;
        /// <summary>
        /// 更新队列超时
        /// </summary>
        public int UpdateQueueTimeoutSeconds
        {
            get { return updateQueueTimeoutSeconds; }
        }
        /// <summary>
        /// SQL表格名称前缀集合
        /// </summary>
        private string[] tableNamePrefixs;
        /// <summary>
        /// SQL表格名称前缀集合
        /// </summary>
        public string[] TableNamePrefixs
        {
            get
            {
                if (tableNamePrefixs == null) tableNamePrefixs = nullValue<string>.Array;
                return tableNamePrefixs;
            }
        }
        /// <summary>
        /// SQL表格名称缺省前缀深度
        /// </summary>
        private int tableNameDepth = 2;
        /// <summary>
        /// SQL表格名称缺省前缀深度
        /// </summary>
        public int TableNameDepth
        {
            get { return tableNameDepth; }
        }
        /// <summary>
        /// 每批导入记录数量
        /// </summary>
        private int importBatchSize = 10000;
        /// <summary>
        /// 每批导入记录数量
        /// </summary>
        public int ImportBatchSize
        {
            get { return importBatchSize > 0 ? importBatchSize : 10000; }
        }
        /// <summary>
        /// 日志流数量
        /// </summary>
        private int logStreamSize = 1 << 16;
        /// <summary>
        /// 日志流数量
        /// </summary>
        public int LogStreamSize
        {
            get { return logStreamSize; }
        }
        /// <summary>
        /// 数据加载超时检测秒数
        /// </summary>
        private int tableLoadCheckTimeout = 10;
        /// <summary>
        /// 数据加载超时检测秒数
        /// </summary>
        public int TableLoadCheckTimeout
        {
            get { return tableLoadCheckTimeout <= 0 ? 10 : tableLoadCheckTimeout; }
        }
        /// <summary>
        /// SQL相关参数
        /// </summary>
        private sql()
        {
            pub.LoadConfig(this);
            if (checkConnection.length() != 0) fastCSharp.log.Default.Add("数据库链接处理类型: " + checkConnection.joinString(',', value => value.ToString()), new System.Diagnostics.StackFrame(), false);
        }
        /// <summary>
        /// 默认SQL相关参数
        /// </summary>
        public static readonly sql Default = new sql();
    }
}
