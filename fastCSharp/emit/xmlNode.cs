using System;
using System.Runtime.CompilerServices;

namespace fastCSharp.emit
{
    /// <summary>
    /// XML节点
    /// </summary>
    public struct xmlNode
    {        /// <summary>
        /// 节点类型
        /// </summary>
        public enum type : byte
        {
            /// <summary>
            /// 字符串
            /// </summary>
            String,
            /// <summary>
            /// 未解码字符串
            /// </summary>
            EncodeString,
            /// <summary>
            /// 未解码可修改字符串
            /// </summary>
            TempString,
            /// <summary>
            /// 子节点
            /// </summary>
            Node,
            /// <summary>
            /// 字符串解析失败
            /// </summary>
            ErrorString,
        }
        /// <summary>
        /// 字符串
        /// </summary>
        internal subString String;
        /// <summary>
        /// 字符串
        /// </summary>
        public subString UnsafeString
        {
            get
            {
                switch (Type)
                {
                    case type.String:
                        return String;
                    case type.EncodeString:
                        if (xmlParser.ParseEncodeString(ref String)) return String;
                        Type = type.ErrorString;
                        break;
                    case type.TempString:
                        if (xmlParser.ParseTempString(ref String)) return String;
                        Type = type.ErrorString;
                        break;
                }
                if (Type == type.ErrorString) log.Error.Throw("XML字符串解析失败 " + String, new System.Diagnostics.StackFrame(), false);
                return default(subString);
            }
        }
        /// <summary>
        /// 属性集合
        /// </summary>
        private keyValue<xmlParser.attributeIndex, xmlParser.attributeIndex>[] attributes;
        /// <summary>
        /// 子节点集合
        /// </summary>
        private keyValue<subString, xmlNode>[] nodes;
        /// <summary>
        /// 子节点集合
        /// </summary>
        internal subArray<keyValue<subString, xmlNode>> Nodes
        {
            get { return subArray<keyValue<subString, xmlNode>>.Unsafe(nodes, 0, String.Length); }
        }
        /// <summary>
        /// 根据名称获取XML节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public xmlNode this[string name]
        {
            get
            {
                if (Type == type.Node)
                {
                    int count = String.Length;
                    if (count != 0)
                    {
                        foreach (keyValue<subString, xmlNode> node in nodes)
                        {
                            if (node.Key.Equals(name)) return node.Value;
                            if (--count == 0) break;
                        }
                    }
                }
                return default(xmlNode);
            }
        }
        /// <summary>
        /// 子节点集合数据
        /// </summary>
        public int Count
        {
            get
            {
                return Type == type.Node ? String.Length : 0;
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public type Type { get; internal set; }
        /// <summary>
        /// 设置字符串
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void SetString(string value)
        {
            String.UnsafeSet(value, 0, value.Length);
            Type = type.String;
        }
        /// <summary>
        /// 设置字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void SetString(string value, int startIndex, int length)
        {
            String.UnsafeSet(value, startIndex, length);
            Type = type.String;
        }
        /// <summary>
        /// 设置子节点集合
        /// </summary>
        /// <param name="nodes"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void SetNode(ref subArray<keyValue<subString, xmlNode>> nodes)
        {
            this.nodes = nodes.array;
            String.UnsafeSetLength(nodes.length);
            Type = type.Node;
        }
        /// <summary>
        /// 属性集合
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="attributes"></param>
        internal void SetAttribute(string xml, keyValue<xmlParser.attributeIndex, xmlParser.attributeIndex>[] attributes)
        {
            String.value = xml;
            this.attributes = attributes;
        }
        /// <summary>
        /// 获取属性索引位置
        /// </summary>
        /// <param name="nameStart"></param>
        /// <param name="nameSize"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal unsafe bool GetAttribute(char* nameStart, int nameSize, ref xmlParser.attributeIndex index)
        {
            if (attributes != null)
            {
                foreach (keyValue<xmlParser.attributeIndex, xmlParser.attributeIndex> attribute in attributes)
                {
                    if (attribute.Key.Length == nameSize)
                    {
                        fixed (char* xmlFixed = String.value)
                        {
                            if (fastCSharp.unsafer.memory.SimpleEqual((byte*)(xmlFixed + attribute.Key.StartIndex), (byte*)nameStart, nameSize << 1))
                            {
                                index = attribute.Value;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
