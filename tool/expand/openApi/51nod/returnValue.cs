using System;
#pragma warning disable

namespace fastCSharp.openApi._51nod
{
    /// <summary>
    /// API请求返回值
    /// </summary>
    public sealed class returnValue : IValue
    {
        /// <summary>
        /// 返回值
        /// </summary>
        public struct value
        {
            /// <summary>
            /// 返回码
            /// </summary>
            public int Code;
            /// <summary>
            /// 数据是否有效
            /// </summary>
            [fastCSharp.code.ignore]
            public returnCode ReturnCode
            {
                get { return (returnCode)Code; }
                set { Code = (int)value; }
            }
        }
        /// <summary>
        /// 返回值
        /// </summary>
        public value Return;
        /// <summary>
        /// 数据是否有效
        /// </summary>
        public bool IsValue
        {
            get { return Return.Code == (int)returnCode.Success; }
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message
        {
            get { return ((returnCode)Return.Code).ToString(); }
        }
    }
    /// <summary>
    /// API请求返回值
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public sealed class returnValue<valueType> : IValue
    {
        /// <summary>
        /// 返回值
        /// </summary>
        public struct value
        {
            /// <summary>
            /// 返回码
            /// </summary>
            public int Code;
            /// <summary>
            /// 数据是否有效
            /// </summary>
            [fastCSharp.code.ignore]
            public returnCode ReturnCode
            {
                get { return (returnCode)Code; }
                set { Code = (int)value; }
            }
            /// <summary>
            /// 返回值
            /// </summary>
            public valueType Value;
        }
        /// <summary>
        /// 返回值
        /// </summary>
        public value Return;
        /// <summary>
        /// 数据是否有效
        /// </summary>
        public bool IsValue
        {
            get { return Return.Code == (int)returnCode.Success; }
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message
        {
            get { return ((returnCode)Return.Code).ToString(); }
        }
        /// <summary>
        /// 返回值
        /// </summary>
        public valueType Value
        {
            get { return Return.Value; }
        }
    }
}
