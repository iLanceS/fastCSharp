using System;
using fastCSharp.code.cSharp;
using System.Threading;
using fastCSharp.io;
using System.IO;
using fastCSharp.net;

namespace fastCSharp.demo.loadBalancingTcpCommandWeb.ajax
{
    /// <summary>
    /// 负载均衡测试Ajax调用
    /// </summary>
    [fastCSharp.code.cSharp.ajax(IsOnlyPost = false, IsPool = true)]
    internal sealed class loadBalancing : fastCSharp.code.cSharp.ajax.call<loadBalancing>
    {
        /// <summary>
        /// 启动负载均衡服务
        /// </summary>
        /// <returns>负载均衡服务是否启动</returns>
        public int Start()
        {
            if (loadBalancingTcpCommandWeb.loadBalancing.IsServer)
            {
#if MONO
                string path = "demo.loadBalancingTcpCommand";
#else
                string path = "loadBalancingTcpCommand";
#endif
#if DEBUG
                FileInfo tcpCommandFile = new FileInfo((@"..\..\..\" + path + @"\bin\Debug\fastCSharp.demo.loadBalancingTcpCommand.exe").pathSeparator());
#else
                FileInfo tcpCommandFile = new FileInfo((@"..\..\..\" + path + @"\bin\Release\fastCSharp.demo.loadBalancingTcpCommand.exe").pathSeparator());
                if (!tcpCommandFile.Exists) tcpCommandFile = new FileInfo((@"..\..\..\" + path + @"\fastCSharp.demo.loadBalancingTcpCommand.exe").pathSeparator());
#endif
                if (tcpCommandFile.Exists)
                {
#if MONO
                    Console.WriteLine("请打开多个(比如3次)负载均衡测试服务节点 " + tcpCommandFile.FullName);
#else
                    fastCSharp.diagnostics.process.StartNew(tcpCommandFile.FullName, "1");
                    fastCSharp.diagnostics.process.StartNew(tcpCommandFile.FullName, "2");
                    fastCSharp.diagnostics.process.StartNew(tcpCommandFile.FullName, "3");
#endif
                }
                else Console.WriteLine("没有找到负载均衡测试服务节点 " + tcpCommandFile.FullName);
                return 1;
            }
            return 0;
        }
        /// <summary>
        /// 负载均衡测试调用
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.ajax(IsReferer = false, IsOnlyPost = false)]
        public int Add(int left, int right)
        {
#if ONLYWEB
            return left + right;
#else
            returnValue<int> value = loadBalancingTcpCommandWeb.loadBalancing.add(left, right);
            if (value.Type != returnValue.type.Success)
            {
                serverError500();
                Console.WriteLine("TCP Error");
            }
            return value.Value;
#endif
        }
        /// <summary>
        /// 负载均衡测试调用
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.ajax(IsReferer = false, IsOnlyPost = false)]
        public void Xor(int left, int right, Action<returnValue<int>> onReturn)
        {
#if ONLYWEB
            onReturn(left ^ right);
#else
            loadBalancingTcpCommandWeb.loadBalancing.xor(left, right, onReturn);
#endif
        }
    }
}
