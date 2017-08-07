using System;

namespace fastCSharp.config
{
    /// <summary>
    /// 内存数据库相关参数
    /// </summary>
    public sealed class memoryDatabase : database
    {
        /// <summary>
        /// 数据库文件刷新超时秒数
        /// </summary>
        private int refreshTimeOutSeconds = 30;
        /// <summary>
        /// 数据库文件刷新超时周期
        /// </summary>
        public long RefreshTimeOutTicks
        {
            get { return new TimeSpan(0, 0, 0, refreshTimeOutSeconds <= 0 ? 30 : refreshTimeOutSeconds).Ticks; }
        }
        ///// <summary>
        ///// 数据库日志文件最小刷新尺寸(单位:KB)
        ///// </summary>
        //internal const int DefaultMinRefreshSize = 1024;
        ///// <summary>
        ///// 数据库日志文件最小刷新尺寸(单位:KB)
        ///// </summary>
        //private int minRefreshSize = DefaultMinRefreshSize;
        ///// <summary>
        ///// 数据库日志文件最小刷新字节数
        ///// </summary>
        //public int MinRefreshSize
        //{
        //    get { return minRefreshSize <= DefaultMinRefreshSize ? DefaultMinRefreshSize : minRefreshSize; }
        //}
        /// <summary>
        /// 数据库日志文件最大刷新比例(:KB)
        /// </summary>
        private int maxRefreshPerKB = 512;
        /// <summary>
        /// 数据库日志文件最大刷新比例(:KB)
        /// </summary>
        public int MaxRefreshPerKB
        {
            get
            {
                return maxRefreshPerKB > 0 ? maxRefreshPerKB : 512;
            }
        }
        /// <summary>
        /// 缓存默认容器尺寸(单位:2^n)
        /// </summary>
        private byte cacheCapacity = 16;
        /// <summary>
        /// 缓存默认容器尺寸
        /// </summary>
        public int CacheCapacity
        {
            get { return cacheCapacity >= 8 && cacheCapacity <= 30 ? 1 << cacheCapacity : (1 << 16); }
        }
        /// <summary>
        /// 客户端缓存字节数
        /// </summary>
        private int bufferSize = 1 << 10;
        /// <summary>
        /// 客户端缓存字节数
        /// </summary>
        public int BufferSize
        {
            get
            {
                return bufferSize <= 0 ? 1 << 10 : bufferSize;
            }
        }

        /// <summary>
        /// 物理层最小数据缓冲区字节数
        /// </summary>
        internal const int MinPhysicalBufferSize = 1 << 12;
        /// <summary>
        /// 物理层默认数据缓冲区字节数(单位:2^n)
        /// </summary>
        private byte physicalBufferSize = 16;
        /// <summary>
        /// 物理层默认数据缓冲区字节数
        /// </summary>
        public int PhysicalBufferSize
        {
            get { return physicalBufferSize >= 12 && physicalBufferSize <= 30 ? 1 << physicalBufferSize : (1 << 16); }
        }
        /// <summary>
        /// 内存数据库相关参数
        /// </summary>
        private memoryDatabase()
        {
            pub.LoadConfig(this);
        }
        /// <summary>
        /// 默认内存数据库相关参数
        /// </summary>
        public static readonly memoryDatabase Default = new memoryDatabase();
    }
}
