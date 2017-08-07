using System;
using System.Threading;
using fastCSharp.threading;
using fastCSharp.reflection;
using System.Text;
using fastCSharp.net.tcp.http;
using System.Runtime.CompilerServices;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// web调用配置
    /// </summary>
    public sealed partial class webCall : webPage
    {
        /// <summary>
        /// 默认为 true 表示代码生成仅选择申明了 fastCSharp.code.cSharp.webCall 的函数，否则选择所有函数，有效域为当前 class。
        /// </summary>
        public bool IsAttribute = true;
        /// <summary>
        /// 指定是否搜索该成员的继承链以查找这些特性，参考System.Reflection.MemberInfo.GetCustomAttributes(bool inherit)，有效域为当前 class。
        /// </summary>
        public bool IsBaseTypeAttribute;
        /// <summary>
        /// 成员匹配自定义属性是否可继承，设置为 true 表示允许申明 fastCSharp.code.cSharp.webCall 的派生类型并且选择继承深度最小的申明配置，有效域为当前 class。
        /// </summary>
        public bool IsInheritAttribute;
        /// <summary>
        /// HTTP 调用全名称，用于替换默认的调用全名称。
        /// </summary>
        public string FullName;
        /// <summary>
        /// 默认为 false 表示支持 GET 请求，否则仅支持 POST 请求，有效域为当前 class。
        /// </summary>
        public bool IsOnlyPost;
        /// <summary>
        /// 正常情况下输入参数会包装成一个 struct，设置为 true 表示只有一个输入参数时序列化操作忽略外壳类型的处理。
        /// </summary>
        public bool IsSerializeBox;
        /// <summary>
        /// 是否仅仅序列化第一个参数，设置为 true 表示忽略生成的包装 struct
        /// </summary>
        public bool IsFirstParameter;
        /// <summary>
        /// web调用接口
        /// </summary>
        public interface IWebCall : webPage.IWebPage
        {
            /// <summary>
            /// HTTP请求表单设置
            /// </summary>
            fastCSharp.net.tcp.http.requestForm RequestForm { set; }
            /// <summary>
            /// 解析web调用参数
            /// </summary>
            /// <typeparam name="parameterType">web调用参数类型</typeparam>
            /// <param name="parameter">web调用参数</param>
            /// <returns>是否成功</returns>
            bool ParseParameter<parameterType>(ref parameterType parameter)
                where parameterType : struct;
        }
        /// <summary>
        /// web调用池
        /// </summary>
        public abstract class callPool
        {
            /// <summary>
            /// web调用
            /// </summary>
            public abstract bool Call();
        }
        /// <summary>
        /// web调用池
        /// </summary>
        /// <typeparam name="callType">web调用类型</typeparam>
        /// <typeparam name="webType">web调用实例类型</typeparam>
        public abstract class callPool<callType, webType> : callPool
            where callType : callPool<callType, webType>
            where webType : IWebCall
        {
            /// <summary>
            /// web调用
            /// </summary>
            public webType WebCall;
        }
        /// <summary>
        /// web调用池
        /// </summary>
        /// <typeparam name="callType">web调用类型</typeparam>
        /// <typeparam name="webType">web调用实例类型</typeparam>
        /// <typeparam name="parameterType">web调用参数类型</typeparam>
        public abstract class callPool<callType, webType, parameterType> : callPool<callType, webType>
            where callType : callPool<callType, webType, parameterType>
            where webType : IWebCall
            where parameterType : struct
        {
            /// <summary>
            /// web调用参数
            /// </summary>
            public parameterType Parameter;
            /// <summary>
            /// web调用池
            /// </summary>
            protected callPool() : base() { }
        }
        /// <summary>
        /// web调用
        /// </summary>
        public abstract class call : webPage.page
        {
            /// <summary>
            /// HTTP请求表单设置
            /// </summary>
            public fastCSharp.net.tcp.http.requestForm RequestForm
            {
                set
                {
                    if (form == null) form = value;
                    else log.Error.Throw(log.exceptionType.ErrorOperation);
                }
            }
            /// <summary>
            /// 根据HTTP请求表单值获取保存文件全称
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>文件全称</returns>
            public virtual string GetSaveFileName(ref fastCSharp.net.tcp.http.requestForm.value value)
            {
                return null;
            }
            /// <summary>
            /// 创建输出
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void createResponse()
            {
                Response = fastCSharp.net.tcp.http.response.Get();
            }
            /// <summary>
            /// 输出结束
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void responseEnd()
            {
                response response = Response;
                Response = null;
                responseEnd(ref response);
            }
            ///// <summary>
            ///// 
            ///// </summary>
            ///// <param name="response"></param>
            //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            //protected void pushResponse(ref response response)
            //{
            //    Response = null;
            //    fastCSharp.net.tcp.http.response.Push(ref response);
            //}
            /// <summary>
            /// AJAX调用
            /// </summary>
            /// <param name="callIndex"></param>
            /// <param name="loader"></param>
            protected virtual void callAjax(int callIndex, fastCSharp.code.cSharp.ajax.loader loader)
            {
                log.Error.Throw(log.exceptionType.ErrorOperation);
            }
            /// <summary>
            /// AJAX调用
            /// </summary>
            /// <param name="callIndex"></param>
            /// <param name="loader"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void CallAjax(int callIndex, fastCSharp.code.cSharp.ajax.loader loader)
            {
                callAjax(callIndex, loader);
            }
        }
        /// <summary>
        /// web调用
        /// </summary>
        /// <typeparam name="callType">web调用类型</typeparam>
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        public abstract class call<callType> : call, IWebCall where callType : call<callType>
        {
            ///// <summary>
            ///// 是否已经加载HTTP请求头部
            ///// </summary>
            //private int isLoadHeader;
            /// <summary>
            /// 当前web调用
            /// </summary>
            private callType thisCall;
            /// <summary>
            /// 是否使用对象池
            /// </summary>
            private bool isPool;
            /// <summary>
            /// web调用
            /// </summary>
            protected call()
            {
                thisCall = (callType)this;
            }
            /// <summary>
            /// WEB页面回收
            /// </summary>
            internal override void PushPool()
            {
                if (isPool)
                {
                    isPool = false;
                    clear();
#if NOJIT
#else
                    typePool<callType>.PushNotNull(thisCall);
#endif
                }
            }
            ///// <summary>
            ///// WEB页面回收
            ///// </summary>
            //protected static void pushPool(callType call)
            //{
            //    if (call.isLoadHeader != 0)
            //    {
            //        call.clear();
            //        if (Interlocked.CompareExchange(ref call.isLoadHeader, 0, 2) == 2) typePool<callType>.Push(call);
            //        return;
            //    }
            //    log.Default.Add("WEB页面回收", true, true);
            //}
            /// <summary>
            /// HTTP请求头部处理
            /// </summary>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="request">HTTP请求头部</param>
            /// <param name="isPool">是否使用WEB页面池</param>
            /// <returns>是否成功</returns>
            internal override bool LoadHeader(long socketIdentity, fastCSharp.net.tcp.http.requestHeader request, ref bool isPool)
            {
                //if (Interlocked.CompareExchange(ref isLoadHeader, isPool ? 2 : 1, 0) == 0)
                //{
                    SocketIdentity = socketIdentity;
                    requestHeader = request;
                    responseEncoding = DomainServer.ResponseEncoding;//request.IsWebSocket ? Encoding.UTF8 : DomainServer.ResponseEncoding;
                    this.isPool = isPool;
                    isPool = false;
                    return true;
                //}
                //log.Default.Add(typeof(callType).fullName() + " 页面回收错误[" + isLoadHeader.toString() + "]", false, true);
                //return false;
            }
        }
    }
}
