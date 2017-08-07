using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.threading;
using fastCSharp.code.cSharp;
using System.Runtime.CompilerServices;
using fastCSharp.net;

namespace fastCSharp.memoryDatabase
{
    /// <summary>
    /// 数据库物理层集合
    /// </summary>
    internal sealed class physicalSet : IDisposable
    {
        /// <summary>
        /// 数据库物理层集合索引编号
        /// </summary>
        private struct physicalInfo
        {
            /// <summary>
            /// 数据文件名
            /// </summary>
            public hashString FileName;
            /// <summary>
            /// 数据库物理层
            /// </summary>
            public physical Physical;
            /// <summary>
            /// 索引编号
            /// </summary>
            public int Identity;
            /// <summary>
            /// 设置数据库物理层
            /// </summary>
            /// <param name="fileName">数据文件名</param>
            /// <param name="physical">数据库物理层</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(string fileName, physical physical)
            {
                FileName = fileName;
                Physical = physical;
            }
            /// <summary>
            /// 清除数据库物理层
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Clear()
            {
                Physical = null;
                FileName.Null();
                ++Identity;
            }
            /// <summary>
            /// 关闭数据库物理层
            /// </summary>
            /// <param name="isWait">是否等待结束</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Close(bool isWait)
            {
                if (Physical != null)
                {
                    if (isWait) Physical.Dispose();
                    else fastCSharp.threading.task.Tiny.Add(Physical, thread.callType.MemoryDatabasePhysicalDispose);
                }
            }
        }
        /// <summary>
        /// 数据库物理层池
        /// </summary>
        private indexValuePool<physicalInfo> physicalPool = new indexValuePool<physicalInfo>(256);
        ///// <summary>
        ///// 数据库物理层集合
        ///// </summary>
        //private physicalInfo[] physicals = new physicalInfo[255];
        /// <summary>
        /// 数据库物理层文件名与索引集合
        /// </summary>
        private readonly Dictionary<hashString, int> fileNameIndexs = dictionary.CreateHashString<int>();
        ///// <summary>
        ///// 数据库物理层空闲索引集合
        ///// </summary>
        //private subArray<int> freeIndexs;
        ///// <summary>
        ///// 数据库物理层集合访问锁
        ///// </summary>
        //private int physicalLock;
        ///// <summary>
        ///// 数据库物理层最大索引号
        ///// </summary>
        //private int maxIndex;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private int isDisposed;
        /// <summary>
        /// 数据库物理层集合
        /// </summary>
        private physicalSet()
        {
            fastCSharp.domainUnload.Add(this, domainUnload.unloadType.MemoryDatabasePhysicalSetDispose);
        }
        ///// <summary>
        ///// 获取一个可用的集合索引
        ///// </summary>
        ///// <returns>集合索引</returns>
        //private int newIndex()
        //{
        //    if (freeIndexs.Count != 0) return freeIndexs.UnsafePop();
        //    if (maxIndex == physicals.Length)
        //    {
        //        physicalInfo[] newPhysicals = new physicalInfo[maxIndex << 1];
        //        Array.Copy(physicals, 0, newPhysicals, 0, maxIndex);
        //        physicals = newPhysicals;
        //    }
        //    return maxIndex++;
        //}
        /// <summary>
        /// 获取数据库物理层集合唯一标识
        /// </summary>
        /// <param name="fileName">数据文件名</param>
        /// <returns>数据库物理层集合唯一标识</returns>
        internal indexIdentity GetIdentity(string fileName)
        {
            int index;
            indexIdentity identity = new indexIdentity { Index = indexIdentity.ErrorIndex };
            hashString key = fileName;
            if (fileNameIndexs.TryGetValue(key, out index))
            {
                identity.Identity = physicalPool.Pool[index].Identity;
                //identity.Identity = physicals[index].Identity;
                int nextIndex;
                if (fileNameIndexs.TryGetValue(key, out nextIndex) && index == nextIndex) identity.Index = index;
            }
            return identity;
        }
        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="fileName">数据文件名</param>
        /// <returns>数据库物理层初始化信息</returns>
        internal physicalServer.physicalIdentity Open(string fileName)
        {
            physicalServer.physicalIdentity physicalInfo = new physicalServer.physicalIdentity { Identity = new physicalServer.timeIdentity { TimeTick = 0, Index = -1 } };
            if (isDisposed == 0)
            {
                hashString key = fileName;
                if (physicalPool.Enter())
                {
                    if (fileNameIndexs.ContainsKey(key)) physicalPool.Exit();
                    else
                    {
                        try
                        {
                            fileNameIndexs.Add(key, physicalInfo.Identity.Index = physicalPool.GetIndexContinue());
                        }
                        finally { physicalPool.Exit(); }
                    }
                    if (physicalInfo.Identity.Index != -1)
                    {
                        try
                        {
                            physical physical = new physical(fileName, false);
                            if (!physical.IsDisposed)
                            {
                                if (physicalPool.Enter())
                                {
                                    physicalPool.Pool[physicalInfo.Identity.Index].Set(fileName, physical);
                                    physicalPool.Exit();
                                }
                                physicalInfo.Identity.Identity = physicalPool.Pool[physicalInfo.Identity.Index].Identity;
                                physicalInfo.Identity.TimeTick = fastCSharp.pub.StartTime.Ticks;
                                physicalInfo.IsLoader = physical.IsLoader;
                            }
                        }
                        finally
                        {
                            if (physicalInfo.Identity.TimeTick == 0 && physicalPool.Enter())
                            {
                                fileNameIndexs.Remove(key);
                                physicalPool.Exit();
                            }
                        }
                    }
                }
            }
            return physicalInfo;
        }
        /// <summary>
        /// 创建数据库文件
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        /// <param name="header">文件头数据</param>
        /// <returns>是否创建成功</returns>
        internal bool Create(indexIdentity identity, ref subArray<byte> header)
        {
            physicalInfo physical = physicalPool.Pool[identity.Index];
            //physicalInfo physical = physicals[identity.Index];
            if (physical.Identity == identity.Identity)
            {
                if (physical.Physical.Create(ref header)) return true;
                Close(identity, false);
            }
            return false;
        }
        /// <summary>
        /// 关闭数据库文件
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        /// <param name="isWait">是否等待关闭</param>
        internal void Close(indexIdentity identity, bool isWait)
        {
            if (physicalPool.Enter())
            {
                physicalInfo physical = physicalPool.Pool[identity.Index];
                if (physical.Identity == identity.Identity)
                {
                    physicalPool.Pool[identity.Index].Clear();
                    fileNameIndexs.Remove(physical.FileName);
                    physicalPool.FreeExit(identity.Index);
                    physical.Close(isWait);
                }
                else physicalPool.Exit();
            }
        }
        /// <summary>
        /// 数据库文件头数据加载
        /// </summary>
        /// <param name="identity"></param>
        /// <returns>文件数据,null表示失败</returns>
        internal subArray<byte> LoadHeader(indexIdentity identity)
        {
            physicalInfo physical = physicalPool.Pool[identity.Index];
            //physicalInfo physical = physicals[identity.Index];
            if (physical.Identity == identity.Identity)
            {
                subArray<byte> data = physical.Physical.LoadHeader();
                if (data.array != null) return data;
                Close(identity, false);
            }
            return default(subArray<byte>);
        }
        /// <summary>
        /// 数据库文件数据加载
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        /// <returns>文件数据,空数组表示结束,null表示失败</returns>
        internal subArray<byte> Load(indexIdentity identity)
        {
            physicalInfo physical = physicalPool.Pool[identity.Index];
            //physicalInfo physical = physicals[identity.Index];
            if (physical.Identity == identity.Identity)
            {
                subArray<byte> data = physical.Physical.Load();
                if (data.array != null) return data;
                Close(identity, false);
            }
            return default(subArray<byte>);
        }
        /// <summary>
        /// 数据库文件加载完毕
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        /// <param name="isLoaded">是否加载成功</param>
        /// <returns>是否加载成功</returns>
        internal bool Loaded(indexIdentity identity, bool isLoaded)
        {
            physicalInfo physical = physicalPool.Pool[identity.Index];
            //physicalInfo physical = physicals[identity.Index];
            if (physical.Identity == identity.Identity)
            {
                if (physical.Physical.Loaded(isLoaded)) return true;
                Close(identity, false);
            }
            return false;
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        /// <param name="data">日志数据</param>
        /// <returns>是否成功写入缓冲区</returns>
        internal int Append(indexIdentity identity, ref subArray<byte> data)
        {
            physicalInfo physical = physicalPool.Pool[identity.Index];
            //physicalInfo physical = physicals[identity.Index];
            if (physical.Identity == identity.Identity)
            {
                int value = physical.Physical.Append(ref data);
                if (value != 0) return value;
                Close(identity, false);
            }
            return 0;
        }
        /// <summary>
        /// 等待缓存写入
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        internal void WaitBuffer(indexIdentity identity)
        {
            physicalInfo physical = physicalPool.Pool[identity.Index];
            //physicalInfo physical = physicals[identity.Index];
            if (physical.Identity == identity.Identity) physical.Physical.WaitBuffer();
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        /// <returns>是否成功</returns>
        internal bool Flush(indexIdentity identity)
        {
            physicalInfo physical = physicalPool.Pool[identity.Index];
            //physicalInfo physical = physicals[identity.Index];
            if (physical.Identity == identity.Identity)
            {
                if (physical.Physical.Flush()) return true;
                Close(identity, false);
            }
            return false;
        }
        /// <summary>
        /// 刷新写入文件缓存区
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        /// <param name="isDiskFile">是否写入到磁盘文件</param>
        /// <returns>是否成功</returns>
        internal bool FlushFile(indexIdentity identity, bool isDiskFile)
        {
            physicalInfo physical = physicalPool.Pool[identity.Index];
            //physicalInfo physical = physicals[identity.Index];
            if (physical.Identity == identity.Identity)
            {
                if (physical.Physical.FlushFile(isDiskFile)) return true;
                Close(identity, false);
            }
            return false;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                fastCSharp.domainUnload.Remove(this, domainUnload.unloadType.MemoryDatabasePhysicalSetDispose, false);
                physicalInfo[] physicals;
                if (physicalPool.Enter())
                {
                    int poolIndex = physicalPool.PoolIndex;
                    try
                    {
                        physicals = physicalPool.Pool.copy();
                    }
                    finally { physicalPool.Exit(); }
                    while (poolIndex != 0) physicals[--poolIndex].Close(false);
                }
            }
        }
        /// <summary>
        /// 数据库物理层集合
        /// </summary>
        public static readonly physicalSet Default = new physicalSet();
        static physicalSet()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
