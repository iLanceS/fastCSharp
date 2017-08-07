using System;
using System.Reflection;

namespace fastCSharp.emit
{
    /// <summary>
    /// 字符串序列化
    /// </summary>
    public unsafe abstract class stringSerializer
    {
        /// <summary>
        /// 字符串输出缓冲区
        /// </summary>
        internal readonly charStream CharStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
        /// <summary>
        /// 字符串输出缓冲区
        /// </summary>
        public charStream UnsafeCharStream
        {
            get { return CharStream; }
        }
        /// <summary>
        /// 祖先节点集合
        /// </summary>
        protected object[] forefather;
        /// <summary>
        /// 祖先节点数量
        /// </summary>
        protected int forefatherCount;
        /// <summary>
        /// 循环检测深度
        /// </summary>
        protected int checkLoopDepth;

        /// <summary>
        /// 获取字符串输出缓冲区属性方法信息
        /// </summary>
        protected static readonly FieldInfo charStreamField = typeof(stringSerializer).GetField("CharStream", BindingFlags.Instance | BindingFlags.NonPublic);
    }
}
