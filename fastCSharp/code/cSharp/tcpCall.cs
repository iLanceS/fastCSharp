using System;
using System.Collections.Generic;
using fastCSharp.net;
using fastCSharp.reflection;
using System.Reflection;
using System.Threading;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// TCP调用配置，必须是 partial class（因为要和生成的代码共用一个类型）。
    /// 这种服务不需要定义一个单独的整合类型，远程调用函数（只支持 static 静态函数）可以分布在同一个程序集的各个 class 中，用于整合大量零碎的远程调用函数，典型的应用比如 WEB 后端的 数据服务。
    /// </summary>
    public partial class tcpCall : tcpBase
    {
        /// <summary>
        /// 获取TCP调用泛型函数集合
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>TCP调用泛型函数集合</returns>
        public static Dictionary<genericMethod, MethodInfo> GetGenericMethods(Type type)
        {
            if (type != null)
            {
                tcpCall tcpCall = fastCSharp.code.typeAttribute.GetAttribute<tcpCall>(type, false, true);//cSharp.Default.IsInheritAttribute
                if (tcpCall != null && tcpCall.IsSetup)
                {
                    Dictionary<genericMethod, MethodInfo> values = dictionary.Create<genericMethod, MethodInfo>();
                    foreach (code.methodInfo method in code.methodInfo.GetMethods<tcpCall>(type, tcpCall.MemberFilter, false, tcpCall.IsAttribute, tcpCall.IsBaseTypeAttribute, tcpCall.IsInheritAttribute))
                    {
                        if (method.Method.IsGenericMethod) values.Add(new genericMethod(method.Method), method.Method);
                    }
                    return values;
                }
            }
            return null;
        }
        /// <summary>
        /// 泛型类型函数调用缓存
        /// </summary>
        private static readonly Dictionary<Type, keyValue<Type, fastCSharp.stateSearcher.ascii<MethodInfo>>> genericTypeMethods = dictionary.CreateOnly<Type, keyValue<Type, fastCSharp.stateSearcher.ascii<MethodInfo>>>();
        /// <summary>
        /// 泛型类型函数调用缓存 访问锁
        /// </summary>
        private static readonly object genericTypeMethodLock = new object();
        /// <summary>
        /// 泛型类型函数调用缓存 版本
        /// </summary>
        private static int genericTypeMethodVersion;
        /// <summary>
        /// 泛型类型函数调用
        /// </summary>
        /// <param name="remoteType">调用代理类型</param>
        /// <param name="methodName">调用函数名称</param>
        /// <param name="methodGenericTypes">方法泛型参数集合</param>
        /// <param name="parameters">调用参数</param>
        /// <returns>函数返回值</returns>
        public static object InvokeGenericTypeMethod(ref fastCSharp.code.remoteType remoteType, string methodName, fastCSharp.code.remoteType[] methodGenericTypes, params object[] parameters)
        {
            return getGenericTypeMethod(ref remoteType, methodName).MakeGenericMethod(methodGenericTypes.getArray(value => value.Type)).Invoke(null, parameters);
        }
        /// <summary>
        /// 泛型类型函数调用
        /// </summary>
        /// <param name="remoteType">调用代理类型</param>
        /// <param name="methodName">调用函数名称</param>
        /// <param name="parameters">调用参数</param>
        /// <returns>函数返回值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static object InvokeGenericTypeMethod(ref fastCSharp.code.remoteType remoteType, string methodName, params object[] parameters)
        {
            return getGenericTypeMethod(ref remoteType, methodName).Invoke(null, parameters);
        }
        /// <summary>
        /// 获取泛型类型函数信息
        /// </summary>
        /// <param name="remoteType">调用代理类型</param>
        /// <param name="methodName">调用函数名称</param>
        /// <returns>泛型类型函数信息</returns>
        private static MethodInfo getGenericTypeMethod(ref fastCSharp.code.remoteType remoteType, string methodName)
        {
            Type type = remoteType.Type;
            if (type.Name == GenericTypeServerName && type.DeclaringType.IsGenericType)
            {
                tcpCall tcpCall = fastCSharp.code.typeAttribute.GetAttribute<tcpCall>(type, false, false);
                if (tcpCall != null && tcpCall.IsGenericTypeServerMethod)
                {
                    tcpCall = fastCSharp.code.typeAttribute.GetAttribute<tcpCall>(type.DeclaringType, false, true);//cSharp.Default.IsInheritAttribute
                    if (tcpCall != null && tcpCall.IsSetup)
                    {
                        keyValue<Type, fastCSharp.stateSearcher.ascii<MethodInfo>> methods;
                        int version = genericTypeMethodVersion;
                        if (!genericTypeMethods.TryGetValue(type, out methods) || methods.Key != type)
                        {
                            Monitor.Enter(genericTypeMethodLock);
                            try
                            {
                                if (version == genericTypeMethodVersion || !genericTypeMethods.TryGetValue(type, out methods))
                                {
                                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                                    methods = new keyValue<Type, fastCSharp.stateSearcher.ascii<MethodInfo>>(type, new fastCSharp.stateSearcher.ascii<MethodInfo>(methodInfos.getArray(value => value.Name), methodInfos, true));
                                    genericTypeMethods.Add(type, methods);
                                    ++genericTypeMethodVersion;
                                }
                            }
                            finally { Monitor.Exit(genericTypeMethodLock); }
                        }
                        return methods.Value.Get(methodName);
                    }
                }
            }
            log.Error.Throw(type.fullName() + " 不符合泛型类型服务器端调用", new System.Diagnostics.StackFrame(), false);
            return null;
        }
        /// <summary>
        /// 泛型方法调用
        /// </summary>
        /// <param name="method">泛型方法信息</param>
        /// <param name="types">泛型参数类型集合</param>
        /// <param name="parameters">调用参数</param>
        /// <returns>返回值</returns>
        public static object InvokeGenericMethod(MethodInfo method, fastCSharp.code.remoteType[] types, params object[] parameters)
        {
            if (method == null) fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.Null);
            return method.MakeGenericMethod(types.getArray(value => value.Type)).Invoke(null, parameters);
        }
        /// <summary>
        /// 成员选择类型，为了防止调用者混淆了远程函数与本地函数在某些情况下产生误调用，默认只选择受保护的方法（包括 private / protected / internal）生成相关代码。
        /// </summary>
        public code.memberFilters Filter = code.memberFilters.NonPublicStatic;
        /// <summary>
        /// 成员选择类型
        /// </summary>
        public code.memberFilters MemberFilter
        {
            get { return Filter & code.memberFilters.Static; }
        }
        /// <summary>
        /// 是否泛型方法服务器端代理,用于代码生成,请不要手动设置此属性,否则可能产生严重的安全问题
        /// </summary>
        public bool IsGenericTypeServerMethod;
        /// <summary>
        /// 是否支持抽象类
        /// </summary>
        public bool IsAbstract;
        /// <summary>
        /// 是否TCP服务配置。一个跨类型单例服务只能存在一个 class 配置 IsServer = true，并且必须指定 Service，用于这个服务名称绑定 TCP 服务配置。
        /// </summary>
        public bool IsServer;
    }
}
namespace fastCSharp.code
{
    /// <summary>
    /// 成员类型
    /// </summary>
    public partial class memberType
    {
        /// <summary>
        /// 泛型参数类型
        /// </summary>
        public memberType GenericParameterType
        {
            get
            {
                return Type.IsGenericParameter ? (memberType)typeof(object) : this;
            }
        }
    }
}
