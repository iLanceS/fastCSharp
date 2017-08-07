using System;
using System.Runtime.CompilerServices;

namespace fastCSharp.emit
{
    /// <summary>
    /// JSON节点
    /// </summary>
    public struct jsonNode
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public enum type : byte
        {
            /// <summary>
            /// 空值
            /// </summary>
            Null,
            /// <summary>
            /// 字符串
            /// </summary>
            String,
            /// <summary>
            /// 未解析字符串
            /// </summary>
            QuoteString,
            /// <summary>
            /// 数字字符串
            /// </summary>
            NumberString,
            /// <summary>
            /// 非数值
            /// </summary>
            NaN,
            /// <summary>
            /// 时间周期值
            /// </summary>
            DateTimeTick,
            /// <summary>
            /// 逻辑值
            /// </summary>
            Bool,
            /// <summary>
            /// 列表
            /// </summary>
            List,
            /// <summary>
            /// 字典
            /// </summary>
            Dictionary,
        }
        /// <summary>
        /// 64位整数值
        /// </summary>
        internal long Int64;
        /// <summary>
        /// 字典
        /// </summary>
        private keyValue<jsonNode, jsonNode>[] dictionary;
        /// <summary>
        /// 字典
        /// </summary>
        internal subArray<keyValue<jsonNode, jsonNode>> Dictionary
        {
            get
            {
                return subArray<keyValue<jsonNode, jsonNode>>.Unsafe(dictionary, 0, (int)Int64);
            }
        }
        /// <summary>
        /// 键值集合
        /// </summary>
        public subArray<keyValue<jsonNode, jsonNode>> UnsafeDictionary
        {
            get
            {
                return Type == type.Dictionary ? subArray<keyValue<jsonNode, jsonNode>>.Unsafe(dictionary, 0, (int)Int64) : default(subArray<keyValue<jsonNode, jsonNode>>);
            }
        }
        /// <summary>
        /// 根据名称获取JSON节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public jsonNode this[string name]
        {
            get
            {
                if (Type == type.Dictionary)
                {
                    for (int index = 0, count = (int)Int64; index != count; ++index)
                    {
                        if (dictionary[index].Key.UnsafeString.Equals(name)) return dictionary[index].Value;
                    }
                }
                return default(jsonNode);
            }
        }
        /// <summary>
        /// 字典名称
        /// </summary>
        public subString UnsafeString
        {
            get
            {
                if (Type == type.QuoteString)
                {
                    jsonParser.ParseQuoteString(ref String, (int)(Int64 >> 32), (char)Int64, (int)Int64 >> 16);
                    Type = type.String;
                }
                return String;
            }
        }
        /// <summary>
        /// 列表
        /// </summary>
        private jsonNode[] list;
        /// <summary>
        /// 列表
        /// </summary>
        public subArray<jsonNode> UnsafeList
        {
            get
            {
                return Type == type.List ? subArray<jsonNode>.Unsafe(list, 0, (int)Int64) : default(subArray<jsonNode>);
            }
        }
        /// <summary>
        /// 字典或列表数据量
        /// </summary>
        public int Count
        {
            get
            {
                return Type == type.Dictionary || Type == type.List ? (int)Int64 : 0;
            }
        }
        /// <summary>
        /// 字符串
        /// </summary>
        internal subString String;
        /// <summary>
        /// 类型
        /// </summary>
        public type Type { get; internal set; }
        /// <summary>
        /// 设置列表
        /// </summary>
        /// <param name="list"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void SetList(ref subArray<jsonNode> list)
        {
            this.list = list.array;
            Int64 = list.length;
            Type = type.List;
        }
        /// <summary>
        /// 设置字典
        /// </summary>
        /// <param name="dictionary"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void SetDictionary(ref subArray<keyValue<jsonNode, jsonNode>> dictionary)
        {
            this.dictionary = dictionary.array;
            Int64 = dictionary.length;
            Type = type.Dictionary;
        }
        /// <summary>
        /// 未解析字符串
        /// </summary>
        /// <param name="escapeIndex">未解析字符串起始位置</param>
        /// <param name="quote">字符串引号</param>
        /// <param name="isTempString"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void SetQuoteString(int escapeIndex, char quote, bool isTempString)
        {
            Type = type.QuoteString;
            Int64 = ((long)escapeIndex << 32) + quote;
            if (isTempString) Int64 += 0x10000;
        }
        /// <summary>
        /// 设置数字字符串
        /// </summary>
        /// <param name="quote"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void SetNumberString(char quote)
        {
            Int64 = quote;
            Type = type.NumberString;
        }
        /// <summary>
        /// 创建字典节点
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static jsonNode CreateDictionary(ref subArray<keyValue<subString, jsonNode>> dictionary)
        {
            jsonNode node = new jsonNode { Type = type.Dictionary };
            subArray<keyValue<jsonNode, jsonNode>> array = dictionary.GetArray(value => new keyValue<jsonNode, jsonNode>(new jsonNode { Type = type.String, String = value.Key }, value.Value)).toSubArray();
            node.SetDictionary(ref array);
            return node;
        }
    }
}
