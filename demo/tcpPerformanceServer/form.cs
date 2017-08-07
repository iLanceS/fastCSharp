using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using fastCSharp.reflection;
using fastCSharp.io;

namespace fastCSharp.demo.tcpPerformanceServer
{
    public partial class form : Form
    {
        /// <summary>
        /// UI线程上下文
        /// </summary>
        private SynchronizationContext context;
#if NotFastCSharpCode
#else
        /// <summary>
        /// TCP调用IOCP性能测试服务
        /// </summary>
        private performanceServer.tcpServer performanceServer;
#endif
        /// <summary>
        /// TCP服务调用配置
        /// </summary>
        private fastCSharp.code.cSharp.tcpServer tcpServer;
        /// <summary>
        /// TCP调用IOCP性能测试服务目标对象
        /// </summary>
        private performanceServer serverTarget;

        public form()
        {
            InitializeComponent();

            context = SynchronizationContext.Current;

            tcpServer = fastCSharp.code.cSharp.tcpServer.GetConfig("tcpPerformance", typeof(fastCSharp.demo.tcpPerformanceServer.performanceServer));
            ipTextBox.Text = tcpServer.Host;
            portTextBox.Text = tcpServer.Port.toString();
            asynchronousCheckBox.Checked = tcpServer.IsServerAsynchronousReceive;

            serverTarget = new performanceServer();
            serverTarget.OnStop += onStop;
            serverTarget.OnClient += onClient;

            startButton_Click(null, null);
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            if (performanceServer.IsStart && fastCSharp.config.pub.Default.IsDebug)
            {
#if DEBUG
                string clientFileName = (@"..\..\..\tcpPerformanceClient\bin\Debug\fastCSharp.demo.tcpPerformanceClient.exe").pathSeparator();
#else
                string clientFileName = (@"..\..\..\tcpPerformanceClient\bin\Release\fastCSharp.demo.tcpPerformanceClient.exe").pathSeparator();
#endif
                fastCSharp.diagnostics.process.StartNew(clientFileName, "1");
            }
#endif
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, EventArgs e)
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            try
            {
                tcpServer.Host = ipTextBox.Text.Trim();
                tcpServer.Port = int.Parse(portTextBox.Text.Trim());
                tcpServer.IsServerAsynchronousReceive = asynchronousCheckBox.Checked;

                ipTextBox.Enabled = portTextBox.Enabled = asynchronousCheckBox.Enabled = startButton.Enabled = false;
                stopButton.Enabled = true;

                performanceServer = new performanceServer.tcpServer(tcpServer, null, serverTarget);
                if (performanceServer.Start())
                {
                    addMessage("server start");
                    return;
                }
                addMessage("server error");
            }
            catch (Exception error)
            {
                addMessage(error.ToString());
            }
            stopButton_Click(null, null);
#endif
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            addMessage("server stop");
            pub.Dispose(ref performanceServer);

            stopButton.Enabled = false;
            ipTextBox.Enabled = portTextBox.Enabled = asynchronousCheckBox.Enabled = startButton.Enabled = true;
#endif
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
        /// 客户端计数
        /// </summary>
        /// <param name="clientCount">客户端数量</param>
        private void onClient(int clientCount)
        {
            context.Post(addMessage, "client count " + clientCount.toString());
        }
        /// <summary>
        /// 停止计数
        /// </summary>
        /// <param name="size">接收字节数</param>
        private void onStop(long size)
        {
            context.Post(addMessage, "receive " + size.ToString() + "B");
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
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close(object sender, FormClosedEventArgs e)
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            pub.Dispose(ref performanceServer);
#endif
        }
    }
}
