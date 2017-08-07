using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using fastCSharp.reflection;
using fastCSharp.code.cSharp;

namespace fastCSharp.demo.tcpPerformanceClient
{
    public partial class form : Form
    {
        /// <summary>
        /// UI线程上下文
        /// </summary>
        private SynchronizationContext context;
        /// <summary>
        /// TCP服务调用配置
        /// </summary>
        private fastCSharp.code.cSharp.tcpServer tcpServer;
        /// <summary>
        /// 默认服务端监听端口
        /// </summary>
        private int defaultPort;

        public form(int isStart)
        {
            InitializeComponent();
            context = SynchronizationContext.Current;

            tcpServer = fastCSharp.code.cSharp.tcpServer.GetConfig("tcpPerformance", typeof(fastCSharp.demo.tcpPerformanceServer.performanceServer));
            ipTextBox.Text = tcpServer.Host;
            portTextBox.Text = (defaultPort = tcpServer.Port).toString();
            asynchronousCheckBox.Checked = tcpServer.IsClientAsynchronousReceive;

            if (isStart != 0) start(null, null);
        }
        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="message">信息</param>
        private void addMessage(string message)
        {
            if (messageTextBox.Text.Length == 0) messageTextBox.Text = message;
            else
            {
                messageTextBox.AppendText(@"
" + message);
                messageTextBox.ScrollToCaret();
            }
        }
        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="message">信息</param>
        private void addMessage(object message)
        {
            addMessage((string)message);
        }
        /// <summary>
        /// 发送数据计数
        /// </summary>
        private int times;
        /// <summary>
        /// 套接字计数
        /// </summary>
        private int socket;
        /// <summary>
        /// 线程计数
        /// </summary>
        private int thread;
        /// <summary>
        /// 数据包字节数
        /// </summary>
        private int packet;
        /// <summary>
        /// 是否随机包
        /// </summary>
        private bool isRandomPacket;
        /// <summary>
        /// 参数检测
        /// </summary>
        /// <returns>参数是否可用</returns>
        private bool parameter()
        {
            tcpServer.Host = ipTextBox.Text.Trim();
            tcpServer.Port = parse(portTextBox.Text, defaultPort);
            tcpServer.IsClientAsynchronousReceive = asynchronousCheckBox.Checked;

            times = parse(timesTextBox.Text, 10000);
            socket = parse(socketTextBox.Text, 100);
            thread = parse(threadTextBox.Text, 1);
            packet = parse(packetTextBox.Text, 1000);
            isRandomPacket = randomCheckBox.Checked;
            if (times <= 0)
            {
                addMessage("times error");
                return false;
            }
            if (socket <= 0)
            {
                addMessage("socket error");
                return false;
            }
            if (thread <= 0)
            {
                addMessage("thread error");
                return false;
            }
            if (packet <= 0)
            {
                addMessage("packet error");
                return false;
            }
            if ((long)times * socket >= int.MaxValue - 1)
            {
                addMessage("times * socket error");
                return false;
            }
            timesTextBox.Enabled = socketTextBox.Enabled = threadTextBox.Enabled = packetTextBox.Enabled = startButton.Enabled = false;
            ipTextBox.Enabled = portTextBox.Enabled = asynchronousCheckBox.Enabled = false;
            timesTextBox.Text = times.toString();
            socketTextBox.Text = socket.toString();
            threadTextBox.Text = thread.toString();
            packetTextBox.Text = packet.toString();
            portTextBox.Text = tcpServer.Port.toString();
            sendSize = receiveSize = isStop = 0;
            stopButton.Enabled = true;
            return true;
        }
        /// <summary>
        /// 发送数据字节数
        /// </summary>
        private long sendSize;
        /// <summary>
        /// 接收数据字节数
        /// </summary>
        private long receiveSize;
        /// <summary>
        /// 等待线程数量
        /// </summary>
        private int waitThread;
        /// <summary>
        /// 线程同步事件
        /// </summary>
        private EventWaitHandle threadWait = new EventWaitHandle(false, EventResetMode.ManualReset);
        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch stopwatch = new Stopwatch();
        /// <summary>
        /// 是否停止
        /// </summary>
        private int isStop;
        /// <summary>
        /// 发送数据次数计数
        /// </summary>
        private int currentTimes;
        /// <summary>
        /// 发送数据总数
        /// </summary>
        private int checkTimes;
        /// <summary>
        /// 开始测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start(object sender, EventArgs e)
        {
            if (parameter())
            {
                currentTimes = times;
                checkTimes = 1;
                try
                {
                    addMessage("create socket");
                    clients = new testClient[socket];
                    for (int index = 0; index != socket; clients[index++] = new testClient(this)) ;
                    threadWait.Reset();
                    waitThread = thread;
                    addMessage("create thread");
                    for (int index = 0; index != thread; ++index)
                    {
                        Interlocked.Increment(ref checkTimes);
                        fastCSharp.threading.threadPool.TinyPool.Start(testThread);
                    }
                }
                catch (Exception error)
                {
                    isStop = 1;
                    addMessage(error.ToString());
                }
                finally
                {
                    checkFinally();
                }
            }
        }
        /// <summary>
        /// 测试线程
        /// </summary>
        private void testThread()
        {
            try
            {
                byte[] buffer = new byte[packet];
                for (int index = buffer.Length; index != 0; buffer[--index] = (byte)index) ;
                fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer parameter = new fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer { Buffer = subArray<byte>.Unsafe(buffer, 0, packet) };
                int randomPacket = packet;
                fastCSharp.random random = fastCSharp.random.Default;
                start();
                while (Interlocked.Decrement(ref currentTimes) >= 0 && isStop == 0)
                {
                    foreach (testClient client in clients)
                    {
                        Interlocked.Increment(ref checkTimes);
                        if (isRandomPacket)
                        {
                            randomPacket = random.Next(packet);
                            parameter.Buffer.UnsafeSetLength(randomPacket);
                        }
                        client.Client.serverSynchronous(parameter, client.OnSend);
                        //client.CommandClient.WaitFree();
                        Interlocked.Add(ref sendSize, randomPacket);
                    }
                }
            }
            catch (Exception error)
            {
                isStop = 1;
                context.Post(addMessage, error.ToString());
            }
            finally { checkFinally(); }
        }
        /// <summary>
        /// 同步线程
        /// </summary>
        private void start()
        {
            if (Interlocked.Decrement(ref waitThread) == 0)
            {
                Thread.Sleep(1000);
                context.Post(addMessage, "start send");
                stopwatch.Restart();
                threadWait.Set();
            }
            else if (isStop == 0) threadWait.WaitOne();
        }
        /// <summary>
        /// 检测测试结束
        /// </summary>
        private void checkFinally()
        {
            if (Interlocked.Decrement(ref checkTimes) == 0) stop(null, null);
        }
        /// <summary>
        /// 通讯错误
        /// </summary>
        /// <param name="_"></param>
        private void error(object _)
        {
            addMessage("receive error");
            stop(null, null);
        }
        /// <summary>
        /// 通讯错误
        /// </summary>
        private void error()
        {
            if (Interlocked.Increment(ref isStop) == 1) context.Post(error, null);
        }
        /// <summary>
        /// 停止客户端
        /// </summary>
        /// <param name="clients"></param>
        private void stop(testClient[] clients)
        {
            foreach (testClient client in clients)
            {
                client.CommandClient = null;
                client.Client.Dispose();
            }
            context.Post(onStop, null);
        }
        /// <summary>
        /// 停止计数
        /// </summary>
        private void onStop(object _)
        {
            stopButton.Enabled = false;
            addMessage("send " + sendSize.toString() + "B receive " + receiveSize.toString() + "B " + stopwatch.ElapsedMilliseconds.toString() + "ms");
            timesTextBox.Enabled = socketTextBox.Enabled = threadTextBox.Enabled = packetTextBox.Enabled = startButton.Enabled = true;
            ipTextBox.Enabled = portTextBox.Enabled = asynchronousCheckBox.Enabled = true;
        }
        /// <summary>
        /// 停止测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stop(object sender, EventArgs e)
        {
            stopwatch.Stop();
            isStop = 1;
            if (sender != null) stopButton.Enabled = false;
            testClient[] clients = Interlocked.Exchange(ref this.clients, null);
            if (clients != null)
            {
                currentTimes = 0;
                foreach (testClient client in clients)
                {
                    if (client != null) receiveSize += client.ReceiveSize;
                }
                fastCSharp.threading.task.Tiny.Add(stop, clients);
            }
        }
        /// <summary>
        /// 测试客户端集合
        /// </summary>
        private testClient[] clients;
        /// <summary>
        /// 测试客户端
        /// </summary>
        class testClient
        {
            /// <summary>
            /// 
            /// </summary>
            private form form;
            /// <summary>
            /// TCP调用IOCP性能测试客户端
            /// </summary>
            internal tcpPerformanceServer.performanceServer.tcpClient Client;
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            internal fastCSharp.net.tcp.commandClient CommandClient;
            /// <summary>
            /// 接收数据处理
            /// </summary>
            internal Action<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer>> OnSend;
            /// <summary>
            /// 接收数据字节数
            /// </summary>
            internal int ReceiveSize;
            /// <summary>
            /// 测试线程
            /// </summary>
            /// <param name="form"></param>
            public testClient(form form)
            {
                this.form = form;
                OnSend = onSend;
                Client = new tcpPerformanceServer.performanceServer.tcpClient(form.tcpServer, null);
                if (Client.TcpCommandClient.StreamSocket != null) CommandClient = Client.TcpCommandClient;
                else
                {
                    pub.Dispose(ref Client);
                    throw new Exception();
                }
            }
            /// <summary>
            /// 接收数据处理
            /// </summary>
            /// <param name="buffer">接收数据</param>
            private void onSend(fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer> buffer)
            {
                if (buffer.Type == fastCSharp.net.returnValue.type.Success) Interlocked.Add(ref ReceiveSize, buffer.Value.Buffer.Count);
                else form.error();
                form.checkFinally();
            }
        }
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static int parse(string stringValue, int defaultValue)
        {
            int value;
            return int.TryParse(stringValue, out value) ? value : defaultValue;
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearMemoryPoolButton_Click(object sender, EventArgs e)
        {
            messageTextBox.Text = string.Empty;
            unmanagedPool.ClearPool();
            memoryPool.ClearPool();
            typePool.ClearPool();
            GC.Collect();
        }
        /// <summary>
        /// 停止计数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close(object sender, FormClosedEventArgs e)
        {
            isStop = 1;
            while (waitThread != 0) Thread.Sleep(1);
        }
    }
}
