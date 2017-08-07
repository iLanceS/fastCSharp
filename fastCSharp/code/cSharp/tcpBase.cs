using System;
using System.Reflection;
using System.Net;
using System.Threading;
using fastCSharp.reflection;
using fastCSharp.threading;
using fastCSharp.net.tcp;
using System.Text;
using System.IO;
using fastCSharp;
using fastCSharp.net.tcp.http;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using fastCSharp.net;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// TCP调用配置基类
    /// </summary>
    public abstract partial class tcpBase : ignoreMember
    {
        /// <summary>
        /// 泛型类型服务器端调用类型名称
        /// </summary>
        public const string GenericTypeServerName = "tcpServer";
        /// <summary>
        /// TCP客户端验证接口
        /// </summary>
        public interface ITcpClientVerify
        {
            /// <summary>
            /// TCP客户端验证
            /// </summary>
            /// <param name="socket">TCP调用客户端套接字</param>
            /// <returns>是否通过验证</returns>
            bool Verify(fastCSharp.net.tcp.commandClient.socket socket);
        }
        /// <summary>
        /// TCP客户端验证函数接口(tcpCall)
        /// </summary>
        public interface ITcpClientVerifyMethod
        {
            /// <summary>
            /// TCP客户端验证
            /// </summary>
            /// <returns>是否通过验证</returns>
            bool Verify();
        }
#if NOJIT
        /// <summary>
        /// TCP客户端验证函数接口(tcpServer)
        /// </summary>
        public interface ITcpClientVerifyMethodAsObject
        {
            /// <summary>
            /// TCP客户端验证
            /// </summary>
            /// <param name="client">TCP调用客户端</param>
            /// <returns>是否通过验证</returns>
            bool Verify(object client);
        }
#else
        /// <summary>
        /// TCP客户端验证函数接口(tcpServer)
        /// </summary>
        /// <typeparam name="clientType">TCP客户端类型</typeparam>
        public interface ITcpClientVerifyMethod<clientType>
        {
            /// <summary>
            /// TCP客户端验证
            /// </summary>
            /// <param name="client">TCP调用客户端</param>
            /// <returns>是否通过验证</returns>
            bool Verify(clientType client);
        }
#endif
        ///// <summary>
        ///// TCP客户端验证
        ///// </summary>
        ///// <typeparam name="clientType">TCP客户端类型</typeparam>
        ///// <param name="client">TCP客户端</param>
        ///// <param name="verify">TCP客户端验证接口</param>
        ///// <returns>验证是否成功</returns>
        //public static bool Verify<clientType>(clientType client, ITcpClientVerifyMethod<clientType> verify)
        //{
        //    if (verify == null) return true;
        //    try
        //    {
        //        return verify.Verify(client);
        //    }
        //    catch (Exception error)
        //    {
        //        log.Default.Add(error, null, false);
        //    }
        //    return false;
        //}
        /// <summary>
        /// TCP服务器端同步验证客户端接口
        /// </summary>
        public interface ITcpVerify : ITcpClientVerify
        {
            /// <summary>
            /// TCP客户端同步验证
            /// </summary>
            /// <param name="socket">同步套接字</param>
            /// <returns>是否通过验证</returns>
            bool Verify(fastCSharp.net.tcp.commandServer.socket socket);
        }
        /// <summary>
        /// HTTP页面
        /// </summary>
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        public sealed class httpPage : webPage.page
        {
            /// <summary>
            /// HTTP请求头部
            /// </summary>
            public fastCSharp.net.tcp.http.requestHeader RequestHeader
            {
                get { return requestHeader; }
            }
            /// <summary>
            /// HTTP请求表单
            /// </summary>
            public fastCSharp.net.tcp.http.requestForm Form
            {
                get { return form; }
            }
            /// <summary>
            /// WEB页面回收
            /// </summary>
            internal override void PushPool()
            {
                clear();
#if NOJIT
#else
                typePool<httpPage>.PushNotNull(this);
#endif
            }
            /// <summary>
            /// 参数反序列化
            /// </summary>
            /// <typeparam name="valueType">参数类型</typeparam>
            /// <param name="value">参数值</param>
            /// <returns>是否成功</returns>
            public unsafe bool DeSerialize<valueType>(ref valueType value)
            {
                switch (RequestHeader.PostType)
                {
                    case requestHeader.postType.Json:
                        return Socket.ParseForm() && fastCSharp.emit.jsonParser.Parse(form.Text, ref value, jsonParserConfig);
                    case requestHeader.postType.Xml:
                        return Socket.ParseForm() && fastCSharp.emit.xmlParser.Parse(form.Text, ref value, xmlParserConfig);
                    case requestHeader.postType.Data:
                        return Socket.ParseForm(dataToType) && deSerialize(ref value);
                    case requestHeader.postType.Form:
                        return Socket.ParseForm() && deSerialize(ref value);
                    case requestHeader.postType.FormData:
                        return deSerialize(ref value);
                    default:
                        return deSerializeQuery(ref value);
                }
            }
            /// <summary>
            /// 参数反序列化
            /// </summary>
            /// <typeparam name="valueType">参数类型</typeparam>
            /// <param name="value">参数值</param>
            /// <returns>是否成功</returns>
            private bool deSerialize<valueType>(ref valueType value)
            {
                switch (form.TextQueryChar)
                {
                    case fastCSharp.config.web.QueryJsonName:
                        return fastCSharp.emit.jsonParser.Parse(form.Text, ref value, jsonParserConfig);
                    case fastCSharp.config.web.QueryXmlName:
                        return fastCSharp.emit.xmlParser.Parse(form.Text, ref value, xmlParserConfig);
                    default:
                        return deSerializeQuery(ref value);
                }
            }
            /// <summary>
            /// 参数反序列化
            /// </summary>
            /// <typeparam name="valueType">参数类型</typeparam>
            /// <param name="value">参数值</param>
            /// <returns>是否成功</returns>
            private bool deSerializeQuery<valueType>(ref valueType value)
            {
                if (requestHeader.ParseQuery(ref value))
                {
                    subString queryJson = requestHeader.QueryJson;
                    if (queryJson.Length != 0) return fastCSharp.emit.jsonParser.Parse(ref queryJson, ref value);
                    subString queryXml = requestHeader.QueryXml;
                    if (queryXml.Length != 0) return fastCSharp.emit.xmlParser.Parse(ref queryXml, ref value, xmlParserConfig);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 输出
            /// </summary>
            /// <param name="returnValue">是否调用成功</param>
            /// <returns>是否输出成功</returns>
            public new bool Response(fastCSharp.net.returnValue returnValue)
            {
                socketBase socket = Socket;
                response response = null;
                long identity = SocketIdentity;
                try
                {
                    base.Response = (response = fastCSharp.net.tcp.http.response.GetCookie(base.Response));
                    base.response(returnValue.Type == returnValue.type.Success ? fastCSharp.web.ajax.Object : fastCSharp.web.ajax.Null);
                    if (responseEnd(ref response)) return true;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                finally { response.Push(ref response); }
                if (socket.ResponseError(identity, net.tcp.http.response.state.ServerError500)) PushPool();
                return false;
            }
            /// <summary>
            /// 输出
            /// </summary>
            /// <typeparam name="outputParameter">输出数据类型</typeparam>
            /// <param name="returnValue">输出数据</param>
            /// <returns>是否输出成功</returns>
            public new unsafe bool Response<outputParameter>(fastCSharp.net.returnValue<outputParameter> returnValue)
            {
                socketBase socket = Socket;
                response response = null;
                long identity = SocketIdentity;
                try
                {
                    base.Response = (response = fastCSharp.net.tcp.http.response.GetCookie(base.Response));
                    if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        if (returnValue.Value == null) base.response(fastCSharp.web.ajax.Object);
                        else
                        {
                            pointer buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
                            try
                            {
                                using (charStream jsonStream = response.ResetJsonStream(buffer.Data, fastCSharp.unmanagedPool.StreamBuffers.Size))
                                {
                                    if (responseEncoding.CodePage == Encoding.Unicode.CodePage)
                                    {
                                        fastCSharp.emit.jsonSerializer.Serialize(returnValue.Value, jsonStream, response.BodyStream, null);
                                    }
                                    else
                                    {
                                        fastCSharp.emit.jsonSerializer.ToJson(returnValue.Value, jsonStream);
                                        base.response(fastCSharp.web.ajax.FormatJavascript(jsonStream));
                                    }
                                }
                            }
                            finally { fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer); }
                        }
                    }
                    else base.response(fastCSharp.web.ajax.Null);
                    if (responseEnd(ref response)) return true;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                finally { response.Push(ref response); }
                if (socket.ResponseError(identity, net.tcp.http.response.state.ServerError500)) PushPool();
                return false;
            }
            /// <summary>
            /// 获取HTTP页面
            /// </summary>
            /// <param name="socket">HTTP套接字接口设置</param>
            /// <param name="domainServer">域名服务</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="requestHeader">HTTP请求头部信息</param>
            /// <param name="form">HTTP表单</param>
            /// <returns>HTTP页面</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Set(fastCSharp.net.tcp.http.socketBase socket, domainServer domainServer
                , long socketIdentity, requestHeader requestHeader, requestForm form)
            {
                Socket = socket;
                DomainServer = domainServer;
                SocketIdentity = socketIdentity;
                this.requestHeader = requestHeader;
                this.form = form;
                responseEncoding = socket.TcpCommandSocket.HttpEncoding ?? domainServer.ResponseEncoding;
            }
        }
        /// <summary>
        /// 泛型函数信息
        /// </summary>
        public struct genericMethod : IEquatable<genericMethod>
        {
            /// <summary>
            /// 泛型参数数量
            /// </summary>
            public int ArgumentCount;
            /// <summary>
            /// 函数名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 参数名称集合
            /// </summary>
            public string[] ParameterTypeNames;
            /// <summary>
            /// 哈希值
            /// </summary>
            public int HashCode;
            /// <summary>
            /// 泛型函数信息
            /// </summary>
            /// <param name="method">泛型函数信息</param>
            public genericMethod(MethodInfo method)
            {
                Name = method.Name;
                Type[] genericParameters = method.GetGenericArguments();
                ArgumentCount = genericParameters.Length;
                ParameterTypeNames = parameterInfo.Get(method, genericParameters).getArray(value => value.ParameterRef + value.ParameterType.FullName);
                HashCode = Name.GetHashCode() ^ ArgumentCount;
                setHashCode();
            }
            /// <summary>
            /// 泛型函数信息
            /// </summary>
            /// <param name="name">函数名称</param>
            /// <param name="argumentCount">泛型参数数量</param>
            /// <param name="typeNames">参数名称集合</param>
            public genericMethod(string name, int argumentCount, params string[] typeNames)
            {
                Name = name;
                ArgumentCount = argumentCount;
                ParameterTypeNames = typeNames;
                HashCode = Name.GetHashCode() ^ ArgumentCount;
                setHashCode();
            }
            /// <summary>
            /// 计算哈希值
            /// </summary>
            private void setHashCode()
            {
                foreach (string name in ParameterTypeNames) HashCode ^= name.GetHashCode();
            }
            /// <summary>
            /// 哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public override int GetHashCode()
            {
                return HashCode;
            }
            /// <summary>
            /// 比较是否相等
            /// </summary>
            /// <param name="other">比较对象</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object other)
            {
                return Equals((genericMethod)other);
                //return other != null && other.GetType() == typeof(genericMethod) && Equals((genericMethod)other);
            }
            /// <summary>
            /// 比较是否相等
            /// </summary>
            /// <param name="other">比较对象</param>
            /// <returns>是否相等</returns>
            public bool Equals(genericMethod other)
            {
                if (HashCode == other.HashCode && Name == other.Name
                    && ParameterTypeNames.Length == other.ParameterTypeNames.Length)
                {
                    int index = 0;
                    foreach (string name in other.ParameterTypeNames)
                    {
                        if (ParameterTypeNames[index++] != name) return false;
                    }
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 泛型回调委托
        /// </summary>
        /// <typeparam name="valueType">返回值类型</typeparam>
        private sealed class genericParameterCallback<valueType>
        {
            /// <summary>
            /// 回调处理
            /// </summary>
            private Func<fastCSharp.net.returnValue<object>, bool> callback;
            /// <summary>
            /// 回调处理
            /// </summary>
            /// <param name="value">返回值</param>
            /// <returns></returns>
            private bool onReturn(fastCSharp.net.returnValue<valueType> value)
            {
                if (value.Type == returnValue.type.Success) return callback(new fastCSharp.net.returnValue<object> { Type = value.Type, Value = (object)value.Value });
                return callback(new fastCSharp.net.returnValue<object> { Type = value.Type });
            }
            /// <summary>
            /// 泛型回调委托
            /// </summary>
            /// <param name="callback">回调处理</param>
            /// <returns>泛型回调委托</returns>
            public static Func<fastCSharp.net.returnValue<valueType>, bool> Create(Func<fastCSharp.net.returnValue<object>, bool> callback)
            {
                return new genericParameterCallback<valueType> { callback = callback }.onReturn;
            }
        }
        /// <summary>
        /// 负载均衡回调
        /// </summary>
        /// <typeparam name="returnType">返回值类型</typeparam>
        public abstract class loadBalancingCallback<returnType>
        {
            /// <summary>
            /// 回调委托
            /// </summary>
            protected Action<fastCSharp.net.returnValue<returnType>> _onReturn_;
            /// <summary>
            /// 回调委托
            /// </summary>
            protected Action<fastCSharp.net.returnValue<returnType>> _onReturnHandle_;
            /// <summary>
            /// 错误尝试次数
            /// </summary>
            protected int _tryCount_;
            /// <summary>
            /// 负载均衡回调
            /// </summary>
            protected loadBalancingCallback()
            {
                _onReturnHandle_ = onReturnValue;
            }
            /// <summary>
            /// TCP客户端回调
            /// </summary>
            /// <param name="returnValue">返回值</param>
            private void onReturnValue(fastCSharp.net.returnValue<returnType> returnValue)
            {
                if (returnValue.Type == fastCSharp.net.returnValue.type.Success || --_tryCount_ <= 0) _push_(returnValue);
                else
                {
                    System.Threading.Thread.Sleep(1);
                    _call_();
                }
            }
            /// <summary>
            /// TCP客户端调用
            /// </summary>
            protected abstract void _call_();
            /// <summary>
            /// 添加到回调池负载均衡回调
            /// </summary>
            /// <param name="returnValue">返回值</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void _push_(fastCSharp.net.returnValue<returnType> returnValue)
            {
                Action<fastCSharp.net.returnValue<returnType>> onReturn = this._onReturn_;
                this._onReturn_ = null;
                _push_(returnValue.Type);
                onReturn(returnValue);
            }
            /// <summary>
            /// 添加到回调池负载均衡回调
            /// </summary>
            /// <param name="returnType">是否回调成功</param>
            protected abstract void _push_(fastCSharp.net.returnValue.type returnType);
        }
        /// <summary>
        /// 负载均衡回调
        /// </summary>
        public abstract class loadBalancingCallback
        {
            /// <summary>
            /// 回调委托
            /// </summary>
            protected Action<fastCSharp.net.returnValue> _onReturn_;
            /// <summary>
            /// 回调委托
            /// </summary>
            protected Action<fastCSharp.net.returnValue> _onReturnHandle_;
            /// <summary>
            /// 错误尝试次数
            /// </summary>
            protected int _tryCount_;
            /// <summary>
            /// 负载均衡回调
            /// </summary>
            protected loadBalancingCallback()
            {
                _onReturnHandle_ = onReturnValue;
            }
            /// <summary>
            /// TCP客户端回调
            /// </summary>
            /// <param name="returnValue">返回值</param>
            private void onReturnValue(fastCSharp.net.returnValue returnValue)
            {
                if (returnValue.Type == returnValue.type.Success || --_tryCount_ <= 0) _push_(returnValue);
                else
                {
                    System.Threading.Thread.Sleep(1);
                    _call_();
                }
            }
            /// <summary>
            /// TCP客户端调用
            /// </summary>
            protected abstract void _call_();
            /// <summary>
            /// 添加到回调池负载均衡回调
            /// </summary>
            /// <param name="returnValue">返回值</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void _push_(fastCSharp.net.returnValue returnValue)
            {
                Action<fastCSharp.net.returnValue> onReturn = this._onReturn_;
                this._onReturn_ = null;
                _push_(returnValue.Type);
                onReturn(returnValue);
            }
            /// <summary>
            /// 添加到回调池负载均衡回调
            /// </summary>
            /// <param name="returnType">是否回调成功</param>
            protected abstract void _push_(fastCSharp.net.returnValue.type returnType);
        }
        /// <summary>
        /// 参数序列化标识
        /// </summary>
        private const uint parameterSerializeValue = 0x10030000;
        /// <summary>
        /// TCP参数流
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public struct tcpStream
        {
            /// <summary>
            /// 客户端索引
            /// </summary>
            public int ClientIndex;
            /// <summary>
            /// 客户端序号
            /// </summary>
            public int ClientIdentity;
            /// <summary>
            /// 否支持读取
            /// </summary>
            public bool CanRead;
            /// <summary>
            /// 否支持查找
            /// </summary>
            public bool CanSeek;
            /// <summary>
            /// 是否可以超时
            /// </summary>
            public bool CanTimeout;
            /// <summary>
            /// 否支持写入
            /// </summary>
            public bool CanWrite;
            /// <summary>
            /// 是否有效
            /// </summary>
            public bool IsStream;
        }
        /// <summary>
        /// 字节数组缓冲区+反序列/反序列化事件
        /// </summary>
        public struct subByteArrayEvent
        {
            /// <summary>
            /// 字节数组缓冲区
            /// </summary>
            public subArray<byte> Buffer;
            /// <summary>
            /// 序列化事件
            /// </summary>
            public memoryPool SerializeEvent;
            /// <summary>
            /// 反序列化事件
            /// </summary>
            public Action<subArray<byte>> DeSerializeEvent;
            /// <summary>
            /// 序列化事件
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void callSerializeEvent()
            {
                if (SerializeEvent != null) SerializeEvent.Push(ref Buffer.array);
            }
            /// <summary>
            /// 反序列化事件
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void callDeSerializeEvent()
            {
                if (DeSerializeEvent != null) DeSerializeEvent(Buffer);
            }
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="buffer">字节数组缓冲区</param>
            /// <returns>内存数据流</returns>
            public static implicit operator subByteArrayEvent(subArray<byte> buffer) { return new subByteArrayEvent { Buffer = buffer }; }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="deSerializer">序列化数据</param>
            /// <param name="value"></param>
            [fastCSharp.emit.dataSerialize.custom]
            private static void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref subByteArrayEvent value)
            {
                deSerializer.DeSerialize(ref value.Buffer);
                value.callDeSerializeEvent();
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="value"></param>
            [fastCSharp.emit.dataSerialize.custom]
            private static void serialize(fastCSharp.emit.dataSerializer serializer, subByteArrayEvent value)
            {
                if (value.Buffer.array == null) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                else
                {
                    fastCSharp.emit.binarySerializer.Serialize(serializer.Stream, ref value.Buffer);
                    value.callSerializeEvent();
                }
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="toJsoner"></param>
            /// <param name="value"></param>
            [fastCSharp.emit.jsonSerialize.custom]
            private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, subByteArrayEvent value)
            {
                using (unmanagedStream dataStream = new unmanagedStream(toJsoner.CharStream))
                {
                    try
                    {
                        if (value.Buffer.array == null) dataStream.Write(fastCSharp.emit.binarySerializer.NullValue);
                        else
                        {
                            fastCSharp.emit.binarySerializer.Serialize(dataStream, ref value.Buffer);
                            value.callSerializeEvent();
                        }
                    }
                    finally { toJsoner.CharStream.From(dataStream); }
                }
            }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">参数</param>
            [fastCSharp.emit.jsonParse.custom]
            private unsafe static void parseJson(fastCSharp.emit.jsonParser parser, ref subByteArrayEvent value)
            {
                byte* read = emit.binaryDeSerializer.DeSerialize((byte*)parser.Current, (byte*)parser.End, parser.Buffer, ref value.Buffer);
                if (read == null) parser.Error(emit.jsonParser.parseState.CrashEnd);
                else
                {
                    parser.Current = (char*)read;
                    value.callDeSerializeEvent();
                }
            }
        }
        /// <summary>
        /// 字节数组缓冲区(反序列化数据必须立即使用,否则可能脏数据)
        /// </summary>
        public struct subByteArrayBuffer
        {
            /// <summary>
            /// 字节数组缓冲区
            /// </summary>
            public subArray<byte> Buffer;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="buffer">字节数组缓冲区</param>
            /// <returns>内存数据流</returns>
            public static implicit operator subByteArrayBuffer(subArray<byte> buffer) { return new subByteArrayBuffer { Buffer = buffer }; }
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="buffer">内存数据流</param>
            /// <returns>字节数组缓冲区</returns>
            public static implicit operator subArray<byte>(subByteArrayBuffer buffer) { return buffer.Buffer; }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="deSerializer">序列化数据</param>
            /// <param name="value">字节数组缓冲区</param>
            [fastCSharp.emit.dataSerialize.custom]
            private static void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref subByteArrayBuffer value)
            {
                deSerializer.DeSerialize(ref value.Buffer);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="value">字节数组缓冲区</param>
            [fastCSharp.emit.dataSerialize.custom]
            private static void serialize(fastCSharp.emit.dataSerializer serializer, subByteArrayBuffer value)
            {
                if (value.Buffer.array == null) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                else fastCSharp.emit.binarySerializer.Serialize(serializer.Stream, ref value.Buffer);
            }
            /// <summary>
            /// 对象转换成JSON字符串
            /// </summary>
            /// <param name="toJsoner">对象转换成JSON字符串</param>
            /// <param name="value">参数</param>
            [fastCSharp.emit.jsonSerialize.custom]
            private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, subByteArrayBuffer value)
            {
                fastCSharp.unsafer.memory.ToJson(toJsoner.CharStream, ref value.Buffer);
            }
            /// <summary>
            /// 对象转换成JSON字符串
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">参数</param>
            [fastCSharp.emit.jsonParse.custom]
            private unsafe static void parseJson(fastCSharp.emit.jsonParser parser, ref subByteArrayBuffer value)
            {
                byte[] buffer = null;
                fastCSharp.emit.jsonParser.typeParser<byte>.Array(parser, ref buffer);
                if (buffer == null) value.Buffer.Null();
                else value.Buffer.UnsafeSet(buffer, 0, buffer.Length);
            }
        }
        /// <summary>
        /// 内存数据流(序列化输入流,反序列化字节数组)(反序列化数据必须立即使用,否则可能脏数据)
        /// </summary>
        public struct subByteUnmanagedStream
        {
            /// <summary>
            /// 内存数据流
            /// </summary>
            public unmanagedStream Stream;
            /// <summary>
            /// 内存数据流
            /// </summary>
            public subArray<byte> Buffer;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="stream">内存数据流</param>
            /// <returns>内存数据流</returns>
            public static implicit operator subByteUnmanagedStream(unmanagedStream stream) { return new subByteUnmanagedStream { Stream = stream }; }
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="stream">内存数据流</param>
            /// <returns>内存数据流</returns>
            public static implicit operator subArray<byte>(subByteUnmanagedStream stream) { return stream.Buffer; }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="deSerializer">序列化数据</param>
            /// <param name="value"></param>
            [fastCSharp.emit.dataSerialize.custom]
            private static void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref subByteUnmanagedStream value)
            {
                deSerializer.DeSerialize(ref value.Buffer);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="value"></param>
            [fastCSharp.emit.dataSerialize.custom]
            private static unsafe void serialize(fastCSharp.emit.dataSerializer serializer, subByteUnmanagedStream value)
            {
                if (value.Stream == null) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                else
                {
                    unmanagedStream stream = serializer.Stream;
                    int streamLength = value.Stream.Length, length = ((streamLength + 3) & (int.MaxValue - 3));
                    stream.PrepLength(length + sizeof(int));
                    byte* data = stream.CurrentData;
                    *(int*)data = streamLength;
                    unsafer.memory.Copy(value.Stream.data.data, data + sizeof(int), length);
                    stream.UnsafeAddLength(length + sizeof(int));
                }
            }
            /// <summary>
            /// 对象转换成JSON字符串
            /// </summary>
            /// <param name="toJsoner">对象转换成JSON字符串</param>
            /// <param name="value">参数</param>
            [fastCSharp.emit.jsonSerialize.custom]
            private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, subByteUnmanagedStream value)
            {
                fastCSharp.unsafer.memory.ToJson(toJsoner.CharStream, ref value.Buffer);
            }
            /// <summary>
            /// 对象转换成JSON字符串
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">参数</param>
            [fastCSharp.emit.jsonParse.custom]
            private unsafe static void parseJson(fastCSharp.emit.jsonParser parser, ref subByteUnmanagedStream value)
            {
                byte[] buffer = null;
                fastCSharp.emit.jsonParser.typeParser<byte>.Array(parser, ref buffer);
                if (buffer == null) value.Buffer.Null();
                else value.Buffer.UnsafeSet(buffer, 0, buffer.Length);
            }
        }
        /// <summary>
        /// JSON参数
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        public struct parameterJsonToSerialize<valueType>
        {
            /// <summary>
            /// 参数值
            /// </summary>
            public valueType Return;
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="deSerializer"></param>
            /// <param name="value">目标数据对象</param>
            [fastCSharp.emit.dataSerialize.custom]
            private static unsafe void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref parameterJsonToSerialize<valueType> value)
            {
                int length = *(int*)deSerializer.Read;
                if (length == fastCSharp.emit.binarySerializer.NullValue)
                {
                    value.Return = default(valueType);
                    return;
                }
                if ((length & 1) == 0 && length > 0)
                {
                    byte* start = deSerializer.Read;
                    if (deSerializer.VerifyRead((length + (2 + sizeof(int))) & (int.MaxValue - 3))
                        && fastCSharp.emit.jsonParser.UnsafeParse((char*)(start + sizeof(int)), length >> 1, ref value.Return, null, deSerializer.Buffer))
                    {
                        return;
                    }
                }
                deSerializer.Error(emit.binaryDeSerializer.deSerializeState.IndexOutOfRange);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="value">数据对象</param>
            [fastCSharp.emit.dataSerialize.custom]
            private static unsafe void serialize(fastCSharp.emit.dataSerializer serializer, parameterJsonToSerialize<valueType> value)
            {
                if (value.Return == null) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                else
                {
                    pointer buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
                    try
                    {
                        using (charStream jsonStream = serializer.ResetJsonStream(buffer.Data, fastCSharp.unmanagedPool.StreamBuffers.Size))
                        {
                            fastCSharp.emit.jsonSerializer.Serialize(value.Return, jsonStream, serializer.Stream, null);
                        }
                    }
                    finally { fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer); }
                }
            }
        }
        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="value">目标对象</param>
        /// <param name="data">序列化数据</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool JsonDeSerialize<valueType>(ref valueType value, ref subArray<byte> data)
        {
            parameterJsonToSerialize<valueType> json = new parameterJsonToSerialize<valueType> { Return = value };
            if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref json))
            {
                value = json.Return;
                return true;
            }
            return false;
        }
        /// <summary>
        /// JSON转换序列化
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="value">目标对象</param>
        /// <param name="jsonValue">序列化对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void JsonToSerialize<valueType>(ref fastCSharp.net.returnValue<valueType> value, ref fastCSharp.net.returnValue<parameterJsonToSerialize<valueType>> jsonValue)
        {
            jsonValue.Type = value.Type;
            jsonValue.Value.Return = value.Value;
        }
        /// <summary>
        /// 成员是否匹配自定义属性类型，默认为 true 表示代码生成仅选择申明了 fastCSharp.code.cSharp.tcpMethod 的函数，否则选择所有函数。对于 tcpCall 有效域为当前 class。
        /// </summary>
        public bool IsAttribute = true;
        /// <summary>
        /// 指定是否搜索该成员的继承链以查找这些特性，参考System.Reflection.MemberInfo.GetCustomAttributes(bool inherit)。对于 tcpCall 有效域为当前 class。
        /// </summary>
        public bool IsBaseTypeAttribute;
        /// <summary>
        /// 成员匹配自定义属性是否可继承，默认为 true 表示允许申明 fastCSharp.code.cSharp.tcpMethod 的派生类型并且选择继承深度最小的申明配置。对于 tcpCall 有效域为当前 class。
        /// </summary>
        public bool IsInheritAttribute = true;
        /// <summary>
        /// 服务名称的唯一标识，默认匹配的配置文件 Key 的后缀（code.cSharp.tcpServer.Service），TCP 注册服务中注册的当前服务名称。对于 tcpCall 必填，并且必须可以作为合法的 C# 类型名称使用。
        /// </summary>
        public string Service;
        /// <summary>
        /// 注册当前服务的 TCP 注册服务名称。
        /// </summary>
        public string TcpRegister;
        /// <summary>
        /// 注册当前服务的 TCP 注册服务名称。
        /// </summary>
        public virtual string TcpRegisterName
        {
            get { return TcpRegister; }
        }
        /// <summary>
        /// 默认为 true 表示只允许注册一个 TCP 服务实例（单例服务，其它服务的注册将失败），但 false 并不代表支持负载均衡（仅仅是在客户端访问某个服务端失败时可以切换到其他服务端连接）。
        /// </summary>
        public bool IsSingleRegister = true;
        /// <summary>
        /// IsSingleRegister = true 时有效，默认为 true 表示当存在其它注册服务时仅仅当成预备服务注册，等另一个当前服务退出时自动将这个服务切换为当前服务，用于存在初始化代价的服务（比如数据服务）实现基本无缝的快速切换。
        /// </summary>
        public bool IsPerpleRegister = true;
        /// <summary>
        /// 服务器端验证客户端的处理类型，用于建立连接时的客户端验证，必须实现接口 fastCSharp.code.cSharp.tcpBase.ITcpVerify。一般情况下建议使用申明验证函数的方式替代这种操作 Socket 的方式。
        /// </summary>
        [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
        public Type VerifyType;
        /// <summary>
        /// 客户端建立连接时建立验证的处理类型，对于 tcpServer 需要实现接口 fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod&lt;clientType&gt;，对于 tcpCall 需要实现接口 fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod。一般情况下建议使用申明验证函数的方式替代这种操作 Socket 的方式。
        /// </summary>
        [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
        public Type VerifyMethodType;
        /// <summary>
        /// 客户端访问的主机名称或者 IP 地址，用于需要使用端口映射服务。
        /// </summary>
        public string RegisterHost;
        /// <summary>
        /// 客户端访问的监听端口，用于需要使用端口映射服务。
        /// </summary>
        public int RegisterPort;
        /// <summary>
        /// 服务主机名称或者 IP 地址，无法解析时默认使用 IPAddress.Any，比如 "www.51nod.com" 或者 "127.0.0.1"
        /// </summary>
        public string Host;
        /// <summary>
        /// 服务主机名称或者 IP 地址
        /// </summary>
        private string host;
        /// <summary>
        /// IP地址
        /// </summary>
        private IPAddress ipAddress;
        /// <summary>
        /// IP地址
        /// </summary>
        internal IPAddress IpAddress
        {
            get
            {
                if (ipAddress == null || host != Host)
                {
                    ipAddress = HostToIpAddress(host = Host) ?? IPAddress.Any;
                }
                return ipAddress;
            }
        }
        /// <summary>
        /// 服务监听端口(服务配置)
        /// </summary>
        public int Port;
        /// <summary>
        /// true 表示客户端固定访问本地配置的主机名称或者 IP 地址，也就是说忽略服务端注册的 IP 地址，用于客户端访问 IP 地址与服务主机注册 IP 地址不匹配的情况（因为一个服务端只能注册一个 IP 地址，而不同层级局域网的客户端可能需要访问不同的 IP 地址）。
        /// </summary>
        public bool IsFixedClientHost;
        /// <summary>
        /// 默认为 true 表示注册主机名称或者 IP 地址需要检测冲突，如果已经存在相同的注册返回错误 fastCSharp.net.tcp.tcpRegister,registerState.HostExists（TCP 服务端口信息已存在），否则将忽略这个错误继续注册操作。
        /// </summary>
        public bool IsRegisterCheckHost = true;
        /// <summary>
        /// 默认为 true 表示服务端 Socket 接收数据采用异步模式，客户端较少的内部服务可以考虑配置为同步模式。
        /// </summary>
        public bool IsServerAsynchronousReceive = true;
        /// <summary>
        /// 默认为 false 表示客户端 Socket 接收数据采用同步模式。
        /// </summary>
        public bool IsClientAsynchronousReceive = false;
        /// <summary>
        /// 服务名称
        /// </summary>
        public virtual string ServiceName
        {
            get { return Service; }
        }
        /// <summary>
        /// 服务端批量发送数据时等待数据毫秒数，对于实时性要求不高的需求可以设置适当的值以提高数据批量处理的概率从而减少网络交互次数。
        /// </summary>
        public int ServerSendSleep;
        /// <summary>
        /// 服务端批量发送数据最大数据量，默认为 1024。
        /// </summary>
        public int MaxServerSendCount = fastCSharp.config.tcpCommand.Default.MaxServerSendCount;
        /// <summary>
        /// 客户端批量发送数据时等待数据毫秒数，对于单线程（没有并发的同步请求）的高频（&gt; 500/s）需求应该保持默认值 0，对于实时性要求不高的需求可以设置适当的值以提高数据批量处理的概率从而减少网络交互次数。
        /// </summary>
        public int ClientSendSleep;
        /// <summary>
        /// 客户端批量发送数据最大数据量，默认为 1024。
        /// </summary>
        public int MaxClientSendCount = fastCSharp.config.tcpCommand.Default.MaxClientSendCount;
        /// <summary>
        /// 默认为 false 使用由函数全名称与参数类型名称组合而成的一个长字符串名称标识该远程调用函数，否则表示在通讯中使用数字编号标识该远程调用函数。从性能的角度推荐使用数字编号标识，同时配置 IsRememberIdentityCommand = true 可以记忆上一次生成的代码中数字编号标识与长字符串名称标识的对应关系。
        /// </summary>
        public bool IsIdentityCommand;
        /// <summary>
        /// 默认为 true 表示生成记忆数字编号标识与长字符串名称标识之间对应关系的代码（{项目名称}.remember.fastCSharp.cs），在 IsIdentityCommand = true 时有效。
        /// </summary>
        public bool IsRememberIdentityCommand = true;
        /// <summary>
        /// 由于 fastCSharp 只生成 C# 的客户端代理程序，为了支持其它语言的客户端对于 TCP 服务的调用，所以降级支持客户端以 HTTP 的形式调用服务器端函数。
        /// </summary>
        public bool IsHttpClient;
        /// <summary>
        /// 默认为 false 表示传输原始数据，否则传输简单变换处理后的数据，作用于继承自 fastCSharp.net.tcp.timeVerifyServer 的服务端。
        /// </summary>
        public bool IsMarkData;
        /// <summary>
        /// HTTP 模式的字符串编码名称，默认为 UTF8。
        /// </summary>
        public string HttpEncodingName;
        /// <summary>
        /// 服务默认验证字符串，为了安全不要写在申明中，应该写在配置文件中，fastCSharp.net.tcp.timeVerifyServer 用到了该字符串。
        /// </summary>
        public string VerifyString;
        /// <summary>
        /// 附加验证字符串信息哈希值
        /// </summary>
        [fastCSharp.code.ignore]
        private ulong? verifyHashCode;
        /// <summary>
        /// 附加验证字符串信息哈希值
        /// </summary>
        [fastCSharp.code.ignore]
        public unsafe ulong VerifyHashCode
        {
            get
            {
                if (verifyHashCode == null)
                {
                    fixed (char* verifyFixed = VerifyString) verifyHashCode = (ulong)fastCSharp.algorithm.hashCode.GetHashCode64((byte*)verifyFixed, VerifyString.Length << 1);
                }
                return verifyHashCode.Value;
            }
        }
        /// <summary>
        /// 重置验证字符串
        /// </summary>
        /// <param name="verifyString"></param>
        internal void ResetVerifyString(string verifyString)
        {
            VerifyString = verifyString;
            verifyHashCode = null;
        }
        /// <summary>
        /// 默认HTTP编码
        /// </summary>
        private fastCSharp.net.returnValue<Encoding> httpEncoding;
        /// <summary>
        /// 默认HTTP编码
        /// </summary>
        internal Encoding HttpEncoding
        {
            get
            {
                if (httpEncoding.Type != returnValue.type.Success)
                {
                    if (HttpEncodingName != null)
                    {
                        try
                        {
                            httpEncoding = Encoding.GetEncoding(HttpEncodingName);
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, HttpEncodingName, false);
                        }
                    }
                    httpEncoding.Type = returnValue.type.Success;
                }
                return httpEncoding.Value;
            }
        }
        /// <summary>
        /// 客户端保持连接心跳包间隔时间默认为 50 秒，对于频率稳定可靠的服务类型可以设置为 0 禁用心跳包。
        /// </summary>
        public int ClientCheckSeconds = 50;
        /// <summary>
        /// 服务器端发送数据（包括客户端接受数据）缓冲区初始化字节数，默认为 4KB。
        /// </summary>
        public int SendBufferSize = fastCSharp.config.appSetting.StreamBufferSize;
        /// <summary>
        /// 服务器端接受数据（包括客户端发送数据）缓冲区初始化字节数，默认为 4KB。
        /// </summary>
        public int ReceiveBufferSize = fastCSharp.config.appSetting.StreamBufferSize;
        /// <summary>
        /// 默认使用二进制序列化，适合参数数据类型稳定的服务，或者可以同步部署服务器端与客户端的内部服务。对于数据类型不稳定的互联网服务应该使用 JSON 序列化。
        /// </summary>
        public bool IsJsonSerialize;
        /// <summary>
        /// 使用二进制序列化访问失败时是否尝试 JSON 序列化再次访问。对于数据读取服务可以考虑设置为 true 解决服务切换时由于数据类型被修改产生的访问失败问题。对于非幂等的写服务应该采用默认值，防止在服务端产生多次写操作。
        /// </summary>
        public bool IsTryJsonSerializable;
        /// <summary>
        /// 是否支持HTTP客户端 或者 是否使用JSON序列化
        /// </summary>
        public bool IsHttpClientOrJsonSerialize
        {
            get
            {
                return IsHttpClient || IsJsonSerialize;
            }
        }
        /// <summary>
        /// 当需要将客户端提供给第三方使用的时候，可能不希望 dll 中同时包含服务端，设置为 true 会客户端代码单独剥离出来生成一个代码文件 {项目名称}.tcpServer.服务名称.client.cs，当然你需要将服务中所有参数与返回值及其依赖的数据类型剥离出来。
        /// </summary>
        public bool IsSegmentation;
        /// <summary>
        /// 当 IsSegmentation = true 时，对于剥离出来的客户端代码指定需要复制的目标路径，也就是你的客户端所在的项目路径。
        /// </summary>
        public string ClientSegmentationCopyPath;
        /// <summary>
        /// 客户端验证的默认超时为 20 秒，超时客户端将被当作攻击者被抛弃。
        /// </summary>
        public int VerifySeconds = 20;
        /// <summary>
        /// 服务端某次接受新的数据，从接受到第一份数据开始，在没有接受完成数据的情况下，每秒接收数据的最低数据量要求（单位:KB），不符合要求的客户端将被当作攻击者被抛弃。默认为 0 表示不做要求。
        /// </summary>
        public int MinReceivePerSecond;
        /// <summary>
        /// 服务端接收新的命令数据的默认超时为 1 分钟，超时客户端将被关闭。
        /// </summary>
        public int RecieveCommandMinutes;
        ///// <summary>
        ///// 接收数据超时的秒数(服务配置)
        ///// </summary>
        //public int ReceiveTimeout;
        /// <summary>
        /// 最大客户端数量，超出的客户端将被抛弃，默认为 int.MaxValue。
        /// </summary>
        public int MaxClientCount = int.MaxValue;
        /// <summary>
        /// 每 IP 最大客户端连接数，超出的客户端将被抛弃，默认为 0 表示不限。
        /// </summary>
        public int MaxClientCountPerIpAddress;
        /// <summary>
        /// 每 IP 最大活动客户端连接数，超出的客户端将被放到队列中等待处理，默认为 0 表示不限。
        /// </summary>
        public int MaxActiveClientCountIpAddress;
        /// <summary>
        /// 服务端接收请求线程数量（一般可以根据 CPU 核心数量调整）。需要说明的是这是一个测试性参数，由于 Socket.Accept 存在严重的阻塞问题（异步模式在高并发连接环境下表现更差），多线程并不能解决高并发连接拒绝服务问题，效果可能还不如队列模式。
        /// </summary>
        public int AcceptThreadCount;
        /// <summary>
        /// 默认使用单个队列处理客户端连接，否则使用 256 个队列处理客户端连接，对于高并发连接处理应该设置为 true 降低线程间的阻塞概率。
        /// </summary>
        public bool IsIpSocketQueues;
        /// <summary>
        /// 默认为 false 表示不压缩数据（适合内网服务），压缩数据需要消耗一定的 CPU 资源降低带宽使用。
        /// </summary>
        public bool IsCompress;
        ///// <summary>
        ///// 是否输出调试信息
        ///// </summary>
        //public bool IsOutputDebug;
        /// <summary>
        /// 复制TCP服务配置成员位图
        /// </summary>
        private static memberMap copyMemberMap = new fastCSharp.code.memberMap<tcpBase>.builder(fastCSharp.code.memberMap<tcpBase>.NewFull()).Clear(value => value.Service).Clear(value => value.IsAttribute).Clear(value => value.IsBaseTypeAttribute).Clear(value => value.IsInheritAttribute);
        /// <summary>
        /// 复制TCP服务配置
        /// </summary>
        /// <param name="value">TCP服务配置</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void CopyFrom(tcpBase value)
        {
            fastCSharp.emit.memberCopyer<tcpBase>.Copy(this, value, copyMemberMap);
        }
        /// <summary>
        /// 获取泛型参数集合
        /// </summary>
        /// <param name="_"></param>
        /// <param name="types">泛型参数集合</param>
        /// <returns>泛型参数集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static fastCSharp.code.remoteType[] GetGenericParameters(int _, params Type[] types)
        {
            return types.getArray(type => new fastCSharp.code.remoteType(type));
        }
        /// <summary>
        /// 获取泛型回调委托
        /// </summary>
        /// <typeparam name="valueType">返回值类型</typeparam>
        /// <param name="callback">回调委托</param>
        /// <returns>泛型回调委托</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static object getGenericParameterCallback<valueType>(Func<fastCSharp.net.returnValue<object>, bool> callback)
        {
            return callback != null ? genericParameterCallback<valueType>.Create(callback) : null;
        }
        /// <summary>
        /// 获取泛型回调委托函数信息
        /// </summary>
        private static readonly MethodInfo getGenericParameterCallbackMethod = typeof(tcpBase).GetMethod("getGenericParameterCallback", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 获取泛型回调委托
        /// </summary>
        /// <param name="type">返回值类型</param>
        /// <param name="callback">回调委托</param>
        /// <returns>泛型回调委托</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static object GetGenericParameterCallback(ref fastCSharp.code.remoteType type, Func<fastCSharp.net.returnValue<object>, bool> callback)
        {
            return ((Func<Func<fastCSharp.net.returnValue<object>, bool>, object>)Delegate.CreateDelegate(typeof(Func<Func<fastCSharp.net.returnValue<object>, bool>, object>), getGenericParameterCallbackMethod.MakeGenericMethod(type.Type)))(callback);
        }
        /// <summary>
        /// 主机名称转换成IP地址
        /// </summary>
        /// <param name="host">主机名称</param>
        /// <returns>IP地址</returns>
        internal static IPAddress HostToIpAddress(string host)
        {
            if (!string.IsNullOrEmpty(host))
            {
                IPAddress ipAddress;
                if (!IPAddress.TryParse(host, out ipAddress))
                {
                    try
                    {
                        ipAddress = Dns.GetHostEntry(host).AddressList[0];
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, host, true);
                    }
                }
                return ipAddress;
            }
            return null;
        }
    }
}
namespace fastCSharp.code
{
    /// <summary>
    /// 参数信息
    /// </summary>
    public sealed partial class parameterInfo
    {
        /// <summary>
        /// 流参数名称
        /// </summary>
        public string StreamParameterName
        {
            get { return ParameterName; }
        }
    }
}
