using System;
using System.Collections;
using System.Collections.Generic;

namespace fastCSharp.code.cSharp.template
{
    /// <summary>
    /// CSharp模板公用模糊类型
    /// </summary>
    internal partial class pub : IEnumerable
    {
        /// <summary>
        /// 成员索引
        /// </summary>
        public const int MemberIndex = 0;
        ///// <summary>
        ///// 成员集合
        ///// </summary>
        public static code.memberInfo[] Members = null;
        /// <summary>
        /// 成员名称
        /// </summary>
        public pub MemberName = null;
        /// <summary>
        /// 枚举糊类型
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return null;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator pub(FullName[] value) { return null; }
        /// <summary>
        /// 成员类型
        /// </summary>
        public class MemberType : pub
        {
        }
        /// <summary>
        /// 空值类型
        /// </summary>
        public class NullType : pub
        {
        }
        /// <summary>
        /// 非空类型
        /// </summary>
        public class NotNullType : pub
        {
        }
        /// <summary>
        /// 枚举参数类型
        /// </summary>
        public class EnumerableArgumentType : pub
        {
        }
        /// <summary>
        /// 类型
        /// </summary>
        public partial class type : pub
        {
        }
        /// <summary>
        /// 类型全名
        /// </summary>
        public partial class FullName : pub
        {
            public FullName() { }
            public FullName(object value) { }
            public int CompareTo(FullName other) { return 0; }
        }
        /// <summary>
        /// 键值对键类型
        /// </summary>
        public class KeyValueKeyType : pub
        {
        }
        /// <summary>
        /// 键值对值类型
        /// </summary>
        public class KeyValueValueType : pub
        {
        }
        /// <summary>
        /// 键值对键类型
        /// </summary>
        public class PairKeyType : KeyValueKeyType
        {
        }
        /// <summary>
        /// 键值对值类型
        /// </summary>
        public class PairValueType : KeyValueKeyType
        {
        }
        /// <summary>
        /// 非可空类型
        /// </summary>
        public struct StructNotNullType
        {
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static implicit operator FullName(StructNotNullType value) { return null; }
        }
        /// <summary>
        /// 字符串转解析值
        /// </summary>
        /// <typeparam name="valueType">解析类型</typeparam>
        /// <param name="stringValue">字符串</param>
        /// <param name="value">解析值</param>
        /// <returns>转换是否成功</returns>
        public static bool TryParse<valueType>(string stringValue, out valueType value)
        {
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 函数调用
        /// </summary>
        /// <param name="value">参数</param>
        /// <returns>返回值</returns>
        public object MethodGenericName(params object[] value)
        {
            return null;
        }
        /// <summary>
        /// 字段/属性
        /// </summary>
        /// <param name="value">参数</param>
        /// <returns>返回值</returns>
        public object PropertyName
        {
            get { return null; }
            set { }
        }
        /// <summary>
        /// 函数调用
        /// </summary>
        /// <param name="value">参数</param>
        /// <returns>返回值</returns>
        public object this[params object[] name]
        {
            get { return null; }
            set { }
        }
        
        /// <summary>
        /// 函数调用
        /// </summary>
        /// <param name="value">参数</param>
        /// <returns>返回值</returns>
        public static object StaticMethodGenericName(params object[] value)
        {
            return null;
        }
        /// <summary>
        /// 函数调用
        /// </summary>
        public static object StaticPropertyName
        {
            get { return null; }
            set { }
        }
    }
}
