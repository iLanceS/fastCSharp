using System;

namespace fastCSharp.net
{
    /// <summary>
    /// 异步返回值
    /// </summary>
    public struct returnValue
    {
        /// <summary>
        /// 返回值类型
        /// </summary>
        public enum type : byte
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown,
            /// <summary>
            /// 成功
            /// </summary>
            Success,
            /// <summary>
            /// 版本过期
            /// </summary>
            VersionExpired,
            /// <summary>
            /// 服务器端反序列化错误
            /// </summary>
            ServerDeSerializeError,
            /// <summary>
            /// 服务器端异常
            /// </summary>
            ServerException,
            /// <summary>
            /// 客户端已关闭
            /// </summary>
            ClientDisposed,
            /// <summary>
            /// 客户端没有接收到数据
            /// </summary>
            ClientNullData,
            /// <summary>
            /// 客户端反序列化错误
            /// </summary>
            ClientDeSerializeError,
            /// <summary>
            /// 客户端异常
            /// </summary>
            ClientException,
            /// <summary>
            /// 日志流过期
            /// </summary>
            LogStreamExpired,
        }
        /// <summary>
        /// 返回值类型
        /// </summary>
        public type Type;
        /// <summary>
        /// 是否存在返回值
        /// </summary>
        public bool IsReturn
        {
            get { return Type == type.Success; }
        }
        /// <summary>
        /// 是否存在返回值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator bool(returnValue value)
        {
            return value.IsReturn;
        }
        /// <summary>
        /// 返回值参数名称
        /// </summary>
        public const string ReturnParameterName = "Return";
        /// <summary>
        /// 异步调用失败
        /// </summary>
        internal static readonly Exception[] Exceptions;
        static returnValue()
        {
            Exceptions = new Exception[Enum.GetMaxValue<type>(-1) + 1];
            for (int index = Exceptions.Length; index != 0; Exceptions[--index] = new Exception("异步调用失败" + ((type)(byte)index).ToString())) ;
        }
    }
    /// <summary>
    /// 异步返回值
    /// </summary>
    /// <typeparam name="returnType">返回值类型</typeparam>
    public struct returnValue<returnType>
    {
        /// <summary>
        /// 返回值
        /// </summary>
        public returnType Value;
        /// <summary>
        /// 返回值类型
        /// </summary>
        public returnValue.type Type;
        /// <summary>
        /// 是否存在返回值
        /// </summary>
        public bool IsReturn
        {
            get { return Type == returnValue.type.Success; }
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        public void Null()
        {
            Type = returnValue.type.Unknown;
            Value = default(returnType);
        }
        /// <summary>
        /// 获取返回值
        /// </summary>
        /// <param name="value">异步返回值</param>
        /// <returns>返回值</returns>
        public static implicit operator returnValue<returnType>(returnType value)
        {
            return new returnValue<returnType> { Type = returnValue.type.Success, Value = value };
        }
        /// <summary>
        /// 获取返回值
        /// </summary>
        /// <param name="value">返回值</param>
        /// <returns>异步返回值</returns>
        public static implicit operator returnType(returnValue<returnType> value)
        {
            if (value.Type == returnValue.type.Success) return value.Value;
            throw returnValue.Exceptions[(byte)value.Type];
        }
    }
}
