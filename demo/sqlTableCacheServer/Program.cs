using System;

namespace fastCSharp.demo.sqlTableCacheServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (fastCSharp.sql.connection.IsCheckConnection)
            {
                try
                {
                    if (Class.Cache == null) Console.WriteLine("配置文件 fastCSharp.demo.sqlTableCacheServer.config 存在未知错误。");
                    else
                    {
                        using (fastCSharp.demo.sqlTableCacheServer.tcpServer.DataReader dataReaderServer = new fastCSharp.demo.sqlTableCacheServer.tcpServer.DataReader())
                        using (fastCSharp.demo.sqlTableCacheServer.tcpServer.DataLog dataLogServer = new fastCSharp.demo.sqlTableCacheServer.tcpServer.DataLog())
                        {
                            if (dataReaderServer.Start() && dataLogServer.Start())
                            {
                                Console.WriteLine("数据服务启动成功");
                                using (fastCSharp.threading.task task = new fastCSharp.threading.task(2, true, fastCSharp.threading.threadPool.TinyPool))
                                {
                                    new fastCSharp.sql.tableLoadChecker<Class>().New();
                                    new fastCSharp.sql.tableLoadChecker<Student>().New();
                                }
                                Console.WriteLine("Press quit to exit.");
                                while (Console.ReadLine() != "quit") ;
                                return;
                            }
                            Console.WriteLine("数据服务启动失败");
                        }
                    }
                }
                catch (TypeInitializationException error)
                {
                    Console.WriteLine("数据库连接失败，请检测 fastCSharp.demo.sqlTableCacheServer.config 连接字符串配置是否正确。");
                    Console.WriteLine(error.ToString());
                }
            }
            else Console.WriteLine("没有找到可用的数据库连接信息");
            Console.ReadKey();
        }
    }
}
