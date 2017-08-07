using System;
using System.Diagnostics;
using System.Threading;
using fastCSharp.memoryDatabase.cache;
using fastCSharp.emit;
using System.Reflection;

namespace fastCSharp.demo.memoryDatabase
{
    /// <summary>
    /// 自增表格示例
    /// </summary>
    internal class identity : data
    {
        /// <summary>
        /// 自增字段
        /// </summary>
        public int Id;

        /// <summary>
        /// 数据缓存
        /// </summary>
        private static identityArray<identity> cache;
        /// <summary>
        /// 内存数据库表格操作工具
        /// </summary>
        private static memoryDatabaseModelTable<identity>.remote table;

        /// <summary>
        /// 单次测试添加记录数量
        /// </summary>
        private static int countPerLoop = 100000;
        /// <summary>
        /// 是否异步回调模式[异步模式下线程数量基本没有意义，同步模式线程数在阀值之内越大越好]
        /// </summary>
        private static bool isCallback = true;
        /// <summary>
        /// 并发线程数量
        /// </summary>
        private static int threadCount = 1;
        /// <summary>
        /// 异步回调处理
        /// </summary>
        private class insertCallback
        {
            /// <summary>
            /// 线程计数
            /// </summary>
            private fastCSharp.threading.waitCount waitCount;
            /// <summary>
            /// 当前线程分配处理数量
            /// </summary>
            private int count;
            /// <summary>
            /// 异步回调处理
            /// </summary>
            /// <param name="waitCount">线程计数</param>
            /// <param name="count">当前线程分配处理数量</param>
            public insertCallback(fastCSharp.threading.waitCount waitCount, int count)
            {
                this.waitCount = waitCount;
                this.count = count;
            }
            /// <summary>
            /// 异步回调处理
            /// </summary>
            /// <param name="value"></param>
            private void onReturn(identity value)
            {
                if (value == null)
                {
                    Console.WriteLine("ERROR");
                }
                if (Interlocked.Decrement(ref count) == 0) waitCount.Decrement();
            }
            /// <summary>
            /// 添加记录
            /// </summary>
            public void Insert()
            {
                Action<identity> onInsered = onReturn;
                for (int index = count; index != 0; --index)
                {
                    table.Insert(new identity { Int = index }, onInsered, false, false);
                }
            }
        }
        /// <summary>
        /// 同步模式处理
        /// </summary>
        private static void task()
        {
            try
            {
                //insertCallback insertCallback = new insertCallback(new threading.waitCount(count));
                for (int index = countPerLoop / threadCount; index != 0; --index)
                {
                    if (table.Insert(new identity { Int = index }, false) == null)
                    {
                        Console.WriteLine("ERROR");
                    }
                    //value.Int = index;
                    //if (db.Update(value, updateMember, false) == null)
                    //{
                    //    Console.WriteLine("ERROR");
                    //}
                }
                //insertCallback.WaitCount.Wait();
            }
            catch (Exception error)
            {
                log.Error.Real(error, null, false);
            }
        }
        /// <summary>
        /// 性能测试
        /// </summary>
        public static void Test()
        {
            typeof(fastCSharp.config.pub).GetProperty("IsDebug", BindingFlags.Instance | BindingFlags.Public).SetValue(fastCSharp.config.pub.Default, true, null);
            fastCSharp.code.cSharp.tcpServer tcpServer = fastCSharp.code.cSharp.tcpServer.GetConfig("memoryDatabasePhysical", typeof(fastCSharp.memoryDatabase.physicalServer));
            tcpServer.Host = "127.0.0.1";
            tcpServer.Port = 12345;
            //tcpServer.SendBufferSize = 128 << 10;
            //tcpServer.ShareMemorySize = 128;
            //tcpServer.IsOnlyShareMemoryClient = true;
            using (fastCSharp.memoryDatabase.physicalServer.tcpServer server = new fastCSharp.memoryDatabase.physicalServer.tcpServer(tcpServer))
            {
                if (server.Start())
                {
                    using (fastCSharp.memoryDatabase.physicalServer.tcpClient client = new fastCSharp.memoryDatabase.physicalServer.tcpClient(tcpServer))
                    {
                        Console.WriteLine("开始加载数据...");
                        Stopwatch time = new Stopwatch();
                        time.Start();
                        using (table = new memoryDatabaseModelTable<identity>.remote(client, cache = new identityArray<identity>()))
                        {
                            cache.WaitLoad();
                            time.Stop();
                            if (table.IsDisposed) Console.WriteLine("数据加载失败");
                            else
                            {
                                Console.WriteLine("数据加载完成[" + cache.Count.toString() + "] " + time.ElapsedMilliseconds.toString() + "ms");
                                Stopwatch allTime = new Stopwatch();
                                using (fastCSharp.threading.task task = new threading.task(threadCount, true, fastCSharp.threading.threadPool.TinyPool))
                                {
                                    allTime.Start();
                                    for (int loop = 1; loop <= 10; ++loop)//循环测试10次
                                    {
                                        int length = 0;
                                        time.Restart();
                                        if (isCallback)
                                        {
                                            fastCSharp.threading.waitCount waitCount = new threading.waitCount(threadCount);
                                            for (int threadIndex = 0; threadIndex != threadCount; ++threadIndex)
                                            {
                                                new insertCallback(waitCount, countPerLoop / threadCount).Insert();
                                            }
                                            waitCount.Wait();
                                        }
                                        else
                                        {
                                            if (threadCount == 1) identity.task();
                                            else
                                            {
                                                for (int threadIndex = 0; threadIndex != threadCount; ++threadIndex) task.Add(identity.task);
                                                task.WaitFree();
                                            }
                                        }
                                        table.Flush(false);
                                        time.Stop();
                                        Console.WriteLine(((loop * countPerLoop) / 10000).toString() + "W[" + length.toString() + "] Thread[" + threadCount.toString() + "] Callback[" + isCallback.ToString() + "] " + time.ElapsedMilliseconds.toString() + "ms");
                                    }
                                    allTime.Stop();
                                }
                                Console.WriteLine(cache.Count.toString() + " " + allTime.ElapsedMilliseconds.toString() + "ms");
                                Console.WriteLine("测试完毕.");
                                GC.Collect();
                            }
                        }
                    }
                }
                else Console.WriteLine("数据库服务端启动失败.");
            }
            Console.ReadKey();
        }
    }
}
