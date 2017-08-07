using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.threading;
using System.Collections.Specialized;
using System.Text;
using System.Data.Common;
using System.Reflection.Emit;
using fastCSharp.sql.expression;
using fastCSharp.code;
using System.Threading;
#if __IOS__
#else
using System.Web;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 公共类型
    /// </summary>
    internal static partial class pubExtension
    {
        /// <summary>
        /// 数值转换调用函数信息集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, MethodInfo> numberToStringMethods = new interlocked.dictionary<Type, MethodInfo>();
        /// <summary>
        /// 获取数值转换委托调用函数信息
        /// </summary>
        /// <param name="type">数值类型</param>
        /// <returns>数值转换委托调用函数信息</returns>
        public static MethodInfo GetNumberToStringMethod(Type type)
        {
            MethodInfo method;
            if (numberToStringMethods.TryGetValue(type, out method)) return method;
            method = typeof(fastCSharp.number).GetMethod("toString", BindingFlags.Static | BindingFlags.Public, null, new Type[] { type }, null);
            numberToStringMethods.Set(type, method);
            return method;
        }

        /// <summary>
        /// 获取可空类型是否为空判断函数信息
        /// </summary>
        /// <returns>可空类型是否为空判断函数信息</returns>
        public static readonly Func<Type, MethodInfo> GetNullableHasValue = (Func<Type, MethodInfo>)Delegate.CreateDelegate(typeof(Func<Type, MethodInfo>), typeof(pub).GetMethod("GetNullableHasValue", BindingFlags.Static | BindingFlags.NonPublic));
        /// <summary>
        /// 获取可空类型获取数据函数信息
        /// </summary>
        /// <returns>可空类型获取数据函数信息</returns>
        public static readonly Func<Type, MethodInfo> GetNullableValue = (Func<Type, MethodInfo>)Delegate.CreateDelegate(typeof(Func<Type, MethodInfo>), typeof(pub).GetMethod("GetNullableValue", BindingFlags.Static | BindingFlags.NonPublic));
        /// <summary>
        /// 获取字段成员集合
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="memberAttribute"></typeparam>
        /// <param name="memberFilter"></param>
        /// <param name="isAllMember"></param>
        /// <returns>字段成员集合</returns>
        public static subArray<FieldInfo> GetFields<valueType, memberAttribute>(memberFilters memberFilter, bool isAllMember)
            where memberAttribute : ignoreMember
        {
            return (subArray<FieldInfo>)getFieldsMethod.MakeGenericMethod(typeof(valueType), typeof(memberAttribute)).Invoke(null, new object[] { memberFilter, isAllMember });
        }
        /// <summary>
        /// 获取字段成员集合函数信息
        /// </summary>
        private static readonly MethodInfo getFieldsMethod = typeof(pub).GetMethod("GetFields", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 获取属性成员集合
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="memberAttribute"></typeparam>
        /// <param name="memberFilter"></param>
        /// <param name="isAllMember"></param>
        /// <param name="isGet">是否必须可读</param>
        /// <param name="isSet">是否必须可写</param>
        /// <returns>属性成员集合</returns>
        internal static subArray<PropertyInfo> GetProperties<valueType, memberAttribute>(memberFilters memberFilter, bool isAllMember, bool isGet, bool isSet)
            where memberAttribute : ignoreMember
        {
            return (subArray<PropertyInfo>)getPropertiesMethod.MakeGenericMethod(typeof(valueType), typeof(memberAttribute)).Invoke(null, new object[] { memberFilter, isAllMember, isGet, isSet });
        }
        /// <summary>
        /// 获取属性成员集合函数信息
        /// </summary>
        private static readonly MethodInfo getPropertiesMethod = typeof(pub).GetMethod("GetProperties", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 获取字段成员集合
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="memberFilter"></param>
        /// <returns>字段成员集合</returns>
        public static keyValue<FieldInfo, int>[] GetFieldIndexs<valueType>(memberFilters memberFilter)
        {
            return (keyValue<FieldInfo, int>[])getFieldIndexsMethod.MakeGenericMethod(typeof(valueType)).Invoke(null, new object[] { memberFilter });
        }
        /// <summary>
        /// 获取字段成员集合函数信息
        /// </summary>
        private static readonly MethodInfo getFieldIndexsMethod = typeof(pub).GetMethod("GetFieldIndexs", BindingFlags.Static | BindingFlags.NonPublic);
#if NOJIT
#else
        /// <summary>
        /// 添加表单函数信息
        /// </summary>
        public static readonly MethodInfo NameValueCollectionAddMethod = typeof(NameValueCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(string), typeof(string) }, null);
        /// <summary>
        /// URL编码函数信息
        /// </summary>
        public static readonly MethodInfo UrlEncodeMethod = typeof(HttpUtility).GetMethod("UrlEncode", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(Encoding) }, null);
        /// <summary>
        /// 引用比较函数信息
        /// </summary>
        public static readonly MethodInfo ReferenceEqualsMethod = typeof(Object).GetMethod("ReferenceEquals", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(object), typeof(object) }, null);

        /// <summary>
        /// 字符串转换调用函数信息集合
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> toStringMethods = dictionary.CreateOnly<Type, MethodInfo>();
        /// <summary>
        /// 字符串转换调用函数信息访问锁
        /// </summary>
        private static readonly object toStringMethodLock = new object();
        /// <summary>
        /// 获取字符串转换委托调用函数信息
        /// </summary>
        /// <param name="type">数值类型</param>
        /// <returns>字符串转换委托调用函数信息</returns>
        public static MethodInfo GetToStringMethod(Type type)
        {
            MethodInfo method;
            Monitor.Enter(toStringMethodLock);
            if (toStringMethods.TryGetValue(type, out method))
            {
                Monitor.Exit(toStringMethodLock);
                return method;
            }
            try
            {
                method = type.GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public, null, nullValue<Type>.Array, null);
                toStringMethods.Add(type, method);
            }
            finally { Monitor.Exit(toStringMethodLock); }
            return method;
        }

        /// <summary>
        /// 获取数值转换委托调用函数信息
        /// </summary>
        /// <returns>数值转换委托调用函数信息</returns>
        public static readonly Func<Type, MethodInfo> GetNumberToCharStreamMethod = (Func<Type, MethodInfo>)Delegate.CreateDelegate(typeof(Func<Type, MethodInfo>), typeof(pub).GetMethod("GetNumberToCharStreamMethod", BindingFlags.Static | BindingFlags.NonPublic));
#endif
    }
}
