using System;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using fastCSharp.threading;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// 时间验证服务
    /// </summary>
    public abstract class timeVerifyServer
    {
        /// <summary>
        /// 时间验证客户端
        /// </summary>
        public interface ITimeVerifyClient
        {
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            fastCSharp.net.tcp.commandClient TcpCommandClient { get; }
            /// <summary>
            /// 时间验证函数
            /// </summary>
            /// <param name="randomPrefix"></param>
            /// <param name="md5Data"></param>
            /// <param name="ticks"></param>
            /// <returns>是否验证成功</returns>
            fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks);
        }
        /// <summary>
        /// 输入参数
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
        internal struct input
        {
            /// <summary>
            /// 随机前缀
            /// </summary>
            public ulong randomPrefix;
            /// <summary>
            /// 输入参数MD5
            /// </summary>
            public byte[] md5Data;
            /// <summary>
            /// 验证时间
            /// </summary>
            public long ticks;
            /// <summary>
            /// 输入参数MD5
            /// </summary>
            /// <param name="verifyString"></param>
            public void MD5(string verifyString)
            {
                md5Data = Md5(verifyString, randomPrefix, ticks);
            }
        }
        /// <summary>
        /// 输出参数
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
        [fastCSharp.emit.boxSerialize]
        internal struct output
#if NOJIT
            : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
        {
            /// <summary>
            /// 验证时间
            /// </summary>
            public long ticks;
            /// <summary>
            /// 返回值
            /// </summary>
            [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
            [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
            public bool Ret;
            /// <summary>
            /// 返回值
            /// </summary>
            public bool Return
            {
                get { return Ret; }
                set { Ret = value; }
            }
#if NOJIT
            /// <summary>
            /// 返回值
            /// </summary>
            public object ReturnObject
            {
                get { return Ret; }
                set { Ret = (bool)value; }
            }
#endif
        }
        /// <summary>
        /// 最后一次验证时间
        /// </summary>
        private long lastVerifyTicks = date.nowTime.UtcNow.Ticks - 1;
        /// <summary>
        /// 最后一次验证时间访问锁
        /// </summary>
        private int lastVerifyTickLock;
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        protected fastCSharp.net.tcp.commandServer server;
        /// <summary>
        /// 设置TCP服务端
        /// </summary>
        /// <param name="tcpServer">TCP服务端</param>
        public virtual void SetCommandServer(fastCSharp.net.tcp.commandServer tcpServer)
        {
            server = tcpServer;
        }
        /// <summary>
        /// 时间验证函数
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="randomPrefix"></param>
        /// <param name="md5Data"></param>
        /// <param name="ticks"></param>
        /// <returns>是否验证成功</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsVerifyMethod = true, IsServerSynchronousTask = false, InputParameterMaxLength = 1024, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        protected virtual bool verify(commandServer.socket socket, ulong randomPrefix, byte[] md5Data, ref long ticks)
        {
            string verify = server.attribute.VerifyString;
            if (verify == null)
            {
                if (fastCSharp.config.pub.Default.IsDebug)
                {
                    log.Error.Add("警告：调试模式未启用服务验证", null, true);
                    return true;
                }
                log.Error.Add("服务验证字符串不能为空", null, true);
            }
            else if (md5Data != null)
            {
                if (ticks <= lastVerifyTicks && ticks != socket.VerifyTimeTicks)
                {
                    if (socket.VerifyTimeTicks == 0)
                    {
                        interlocked.CompareSetYield(ref lastVerifyTickLock);
                        socket.VerifyTimeTicks = ++lastVerifyTicks;
                        lastVerifyTickLock = 0;
                    }
                    ticks = socket.VerifyTimeTicks;
                    return false;
                }
                if (Md5(verify, randomPrefix, ticks).equal(md5Data))
                {
                    if (ticks > lastVerifyTicks)
                    {
                        interlocked.CompareSetYield(ref lastVerifyTickLock);
                        if (ticks > lastVerifyTicks) lastVerifyTicks = ticks;
                        lastVerifyTickLock = 0;
                    }
                    if (server.attribute.IsMarkData) socket.MarkData = server.attribute.VerifyHashCode ^ randomPrefix;
                    return true;
                }
            }
            ticks = 0;
            return false;
        }
        /// <summary>
        /// 设置变换数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="verifyString"></param>
        /// <param name="randomPrefix"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void setMarkData(commandServer.socket socket, string verifyString, ulong randomPrefix)
        {
            server.attribute.ResetVerifyString(verifyString);
            socket.MarkData = server.attribute.VerifyHashCode ^ randomPrefix;
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="value"></param>
        /// <param name="randomPrefix"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static unsafe byte[] Md5(string value, ulong randomPrefix, long ticks)
        {
            byte[] buffer = fastCSharp.memoryPool.TinyBuffers.Get((value.Length << 1) + (sizeof(ulong) + sizeof(long)));
            try
            {
                fixed (char* valueFixed = value)
                fixed (byte* dataFixed = buffer)
                {
                    *(ulong*)dataFixed = randomPrefix;
                    *(long*)(dataFixed + sizeof(ulong)) = ticks;
                    fastCSharp.unsafer.memory.UnsafeSimpleCopy(valueFixed, (char*)(dataFixed + (sizeof(ulong) + sizeof(long))), value.Length);
                }
                using (MD5 md5 = new MD5CryptoServiceProvider()) return md5.ComputeHash(buffer, 0, (value.Length << 1) + (sizeof(ulong) + sizeof(long)));
            }
            finally { fastCSharp.memoryPool.TinyBuffers.PushNotNull(buffer); }
        }
        /// <summary>
        /// 时间验证客户端验证
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public unsafe static bool Verify(ITimeVerifyClient client)
        {
            long ticks;
            fastCSharp.net.tcp.commandClient commandClient = client.TcpCommandClient;
            fastCSharp.code.cSharp.tcpServer attribute = commandClient.Attribute;
            string verifyString = attribute.VerifyString;
            if (verifyString == null)
            {
                ticks = 0;
                return client.verify(0, null, ref ticks).Value;
            }
            ulong markData = 0;
            if (attribute.IsMarkData) markData = attribute.VerifyHashCode;
            ticks = date.UtcNow.Ticks;
            do
            {
                ulong randomPrefix = random.Default.SecureNextULongNotZero();
                while (randomPrefix == markData) randomPrefix = random.Default.SecureNextULongNotZero();
                commandClient.ReceiveMarkData = attribute.IsMarkData ? markData ^ randomPrefix : 0UL;
                commandClient.SendMarkData = 0;
                long lastTicks = ticks;
                fastCSharp.net.returnValue<bool> isVerify = client.verify(randomPrefix, Md5(verifyString, randomPrefix, ticks), ref ticks);
                if (isVerify.Value)
                {
                    commandClient.SendMarkData = commandClient.ReceiveMarkData;
                    return true;
                }
                if (isVerify.Type != fastCSharp.net.returnValue.type.Success || ticks <= lastTicks)
                {
                    log.Error.Add("TCP客户端验证失败 [" + isVerify.Type.ToString() + "] " + ticks.toString() + " <= " + lastTicks.toString(), null, false);
                    return false;
                }
                log.Error.Add("TCP客户端验证时间失败重试 " + ticks.toString() + " - " + lastTicks.toString(), null, false);
                //++ticks;
            }
            while (true);
        }
        /// <summary>
        /// TCP调用模拟服务端
        /// </summary>
        public sealed class tcpCallServer : fastCSharp.net.tcp.commandServer
        {
            /// <summary>
            /// TCP调用模拟服务端
            /// </summary>
            /// <param name="type"></param>
            public tcpCallServer(Type type) : base(type) { }
        }
        /// <summary>
        /// tcpCall客户端验证调用函数
        /// </summary>
        /// <param name="randomPrefix"></param>
        /// <param name="md5Data"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public delegate fastCSharp.net.returnValue<bool> verifyMethod(ulong randomPrefix, byte[] md5Data, ref long ticks);
        /// <summary>
        /// tcpCall时间验证客户端验证
        /// </summary>
        /// <param name="client"></param>
        /// <param name="verifyMethod"></param>
        /// <returns></returns>
        protected static bool verify(fastCSharp.net.tcp.commandClient client, verifyMethod verifyMethod)
        {
            long ticks;
            fastCSharp.code.cSharp.tcpServer attribute = client.Attribute;
            string verifyString = attribute.VerifyString;
            if (verifyString == null)
            {
                ticks = 0;
                return verifyMethod(0, null, ref ticks).Value;
            }
            ulong markData = 0;
            if (attribute.IsMarkData) markData = attribute.VerifyHashCode;
            ticks = date.UtcNow.Ticks;
            do
            {
                ulong randomPrefix = random.Default.SecureNextULongNotZero();
                while (randomPrefix == markData) randomPrefix = random.Default.SecureNextULongNotZero();
                client.ReceiveMarkData = attribute.IsMarkData ? markData ^ randomPrefix : 0;
                client.SendMarkData = 0;
                long lastTicks = ticks;
                fastCSharp.net.returnValue<bool> isVerify = verifyMethod(randomPrefix, Md5(verifyString, randomPrefix, ticks), ref ticks);
                if (isVerify.Value)
                {
                    client.SendMarkData = client.ReceiveMarkData;
                    return true;
                }
                if (isVerify.Type != fastCSharp.net.returnValue.type.Success || ticks <= lastTicks)
                {
                    log.Error.Add("TCP客户端验证失败 [" + isVerify.Type.ToString() + "] " + ticks.toString() + " <= " + lastTicks.toString(), null, false);
                    return false;
                }
                log.Error.Add("TCP客户端验证时间失败重试 " + ticks.toString() + " - " + lastTicks.toString(), null, false);
                //++ticks;
            }
            while (true);
        }
        /// <summary>
        /// 设置接收变换数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="markData"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void SetReceiveMarkData(fastCSharp.net.tcp.commandClient client, ulong markData)
        {
            client.ReceiveMarkData = markData;
            client.SendMarkData = 0;
        }
        /// <summary>
        /// 设置发送变换数据
        /// </summary>
        /// <param name="client"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void SetSendMarkData(fastCSharp.net.tcp.commandClient client)
        {
            client.SendMarkData = client.ReceiveMarkData;
        }
        /// <summary>
        /// TCP调用时间验证服务
        /// </summary>
        /// <typeparam name="verifyType">验证类型</typeparam>
        public abstract class tcpCall<verifyType> : timeVerifyServer
            where verifyType : tcpCall<verifyType>, new()
        {
            /// <summary>
            /// TCP调用模拟服务端
            /// </summary>
            private new static readonly tcpCallServer tcpCallServer;
            /// <summary>
            /// TCP调用时间验证服务
            /// </summary>
            private static readonly timeVerifyServer timeVerify;
            /// <summary>
            /// TCP调用时间验证服务
            /// </summary>
            public tcpCall()
            {
                server = tcpCallServer;
            }
            /// <summary>
            /// 时间验证函数
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="randomPrefix"></param>
            /// <param name="md5Data"></param>
            /// <param name="ticks"></param>
            /// <returns>是否验证成功</returns>
            [fastCSharp.code.cSharp.tcpMethod(IsVerifyMethod = true, IsServerSynchronousTask = false, InputParameterMaxLength = 1024, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
            protected new static bool verify(commandServer.socket socket, ulong randomPrefix, byte[] md5Data, ref long ticks)
            {
                return timeVerify.verify(socket, randomPrefix, md5Data, ref ticks);
            }
            /// <summary>
            /// 时间验证客户端验证
            /// </summary>
            /// <param name="client"></param>
            /// <param name="verifyMethod"></param>
            /// <returns></returns>
            public static bool Verify(fastCSharp.net.tcp.commandClient client, verifyMethod verifyMethod)
            {
                return verify(client, verifyMethod);//tcpCallServer.attribute.VerifyString
            }
            static tcpCall()
            {
                tcpCallServer = new tcpCallServer(typeof(verifyType));
                timeVerify = new verifyType();
            }
        }
    }
}
