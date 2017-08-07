using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace fastCSharp
{
    /// <summary>
    /// 类型转换
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct unionType
    {
        /// <summary>
        /// 回调对象
        /// </summary>
        [FieldOffset(0)]
        public object Value;
        /// <summary>
        /// 委托
        /// </summary>
        [FieldOffset(0)]
        public Action Action;
        /// <summary>
        /// 异常委托
        /// </summary>
        [FieldOffset(0)]
        public Action<Exception> ActionException;
        /// <summary>
        /// 委托
        /// </summary>
        [FieldOffset(0)]
        public Action<int> ActionInt;
        /// <summary>
        /// IP终结点
        /// </summary>
        [FieldOffset(0)]
        public System.Net.IPEndPoint IPEndPoint;
        /// <summary>
        /// 程序集
        /// </summary>
        [FieldOffset(0)]
        public System.Reflection.Assembly Assembly;
        /// <summary>
        /// 程序集数组
        /// </summary>
        [FieldOffset(0)]
        public System.Reflection.Assembly[] AssemblyArray;

        /// <summary>
        /// 内存数据库表格操作工具(远程)数据加载
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.emit.memoryDatabaseTable.remoteTableLoader MemoryDatabaseTableRemoteTableLoader;
        /// <summary>
        /// 新建文件监视
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.io.createFlieTimeoutWatcher CreateFlieTimeoutWatcher;
        /// <summary>
        /// 文件分块写入流读取
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.io.fileBlockStream.reader FileBlockStreamReader;
        /// <summary>
        /// 文件流写入器
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.io.fileStreamWriter FileStreamWriter;
        /// <summary>
        /// 日志处理
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.log Log;
        /// <summary>
        /// 日志流
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.logStream LogStream;
        /// <summary>
        /// TCP调用客户端路由创建客户端
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandClient.routerClientCreator TcpCommandClientRouterClientCreator;
        /// <summary>
        /// TCP客户端命令流处理套接字
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandClient.streamCommandSocket TcpCommandClientSocket;
        /// <summary>
        /// TCP客户端命令
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandClient.streamCommandSocket.command TcpCommandClientCommand;
        /// <summary>
        /// TCP客户端命令接收数据
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandClient.streamCommandSocket.commandReceiver TcpCommandClientSocketReceiver;
        /// <summary>
        /// 创建TCP客户端命令流处理套接字
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandClient.streamSocketCreator TcpCommandClientSocketCreator;
        /// <summary>
        /// TCP调用负载均衡服务端
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandLoadBalancingServer TcpCommandLoadBalancingServer;
        /// <summary>
        /// TCP调用负载均衡服务端信息
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandLoadBalancingServer.serverInfo TcpCommandLoadBalancingServerInfo;
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandServer TcpCommandServer;
        /// <summary>
        /// TCP调用套接字
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandServer.socket TcpCommandServerSocket;
        /// <summary>
        /// TCP服务端数据读取器
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandServer.socket.streamReceiver TcpCommandServerSocketStreamReceiver;
        /// <summary>
        /// TCP服务器端调用
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.commandServer.socketCall TcpCommandServerSocketCall;
        /// <summary>
        /// HTTP服务器
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.http.servers HttpServers;
        /// <summary>
        /// HTTP会话标识
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.http.session HttpSession;
        /// <summary>
        /// HTTP套接字
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.http.socketBase HttpSocket;
        /// <summary>
        /// HTTP套接字数据接收器
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.http.socketBase.boundaryIdentityReceiver HttpSocketBoundaryIdentityReceiver;
        /// <summary>
        /// TCP调用服务
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.server TcpServer;
        /// <summary>
        /// TCP注册服务
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.tcpRegister TcpRegister;
        /// <summary>
        /// TCP注册服务客户端
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.tcp.tcpRegister.client TcpRegisterClient;
        /// <summary>
        /// 超时队列
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.threading.timeoutQueue TimeoutQueue;
        /// <summary>
        /// 超时队列
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.timeoutQueue NetTimeoutQueue;
        /// <summary>
        /// 超时检测接口
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.net.timeoutQueue.ICheckTimeout NetICheckTimeout;
        /// <summary>
        /// 内存数据库物理层
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.memoryDatabase.physical MemoryDatabasePhysical;
        /// <summary>
        /// 内存数据库物理层文件读取器
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.memoryDatabase.physical.fileReader MemoryDatabasePhysicalFileReader;
        /// <summary>
        /// 数据库物理层集合
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.memoryDatabase.physicalSet MemoryDatabasePhysicalSet;
        /// <summary>
        /// 任务信息
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.run Run;
        /// <summary>
        /// 单线程读取队列节点
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.threading.queue.singleDequeueNode SingleDequeueNode;
        /// <summary>
        /// 套接字
        /// </summary>
        [FieldOffset(0)]
        public Socket Socket;
        /// <summary>
        /// SQL客户端添加数据
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.sql.client.inserter SqlClientInserter;
        /// <summary>
        /// SQL客户端获取数据
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.sql.client.selector SqlClientSelector;
        /// <summary>
        /// 任务处理类
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.threading.task Task;
        /// <summary>
        /// 任务处理类
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.threading.taskBase TaskBase;
        /// <summary>
        /// 任务队列
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.threading.taskQueue TaskQueue;
        /// <summary>
        /// 线程池
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.threading.threadPool ThreadPool;
        /// <summary>
        /// 定时任务
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.threading.timerTask TimerTask;
        /// <summary>
        /// 可取消定时任务
        /// </summary>
        [FieldOffset(0)]
        public fastCSharp.threading.timerCancelTask TimerCancelTask;
    }
}
