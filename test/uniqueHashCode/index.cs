using System;
using fastCSharp;

namespace fastCSharp.test.uniqueHashCode
{
    /// <summary>
    /// 数据位置信息
    /// </summary>
    struct index
    {
        /// <summary>
        /// 位置类型
        /// </summary>
        public enum type
        {
            /// <summary>
            /// 左侧往后第几个
            /// </summary>
            Left = 0,
            /// <summary>
            /// 右侧往前第几个
            /// </summary>
            Right = 1,
            /// <summary>
            /// 位置二进制右移
            /// </summary>
            Shift = 2
        }
        /// <summary>
        /// 数据位置
        /// </summary>
        public int Index;
        /// <summary>
        /// 位置类型
        /// </summary>
        public type Type;
        /// <summary>
        /// 根据关键字获取字符
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public char GetChar(string key)
        {
            switch(Type)
            {
                case type.Left: return key[Index];
                case type.Right: return key[key.Length - Index - 1];
                default: return key[key.Length >> Index];
            }
        }
        /// <summary>
        /// 数据位置信息转换成代码片段
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Type == type.Left) return Index.toString();
            if (Type == type.Right) return "-" + (Index + 1).toString();
            return ">>" + Index.toString();
        }
    }
}
