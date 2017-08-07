using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 先进先出优先队列
    /// </summary>
    /// <typeparam name="keyType">键值类型</typeparam>
    /// <typeparam name="valueType">数据类型</typeparam>
    public sealed class fifoPriorityQueue<keyType, valueType> where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 数据节点
        /// </summary>
        private sealed class node
        {
            /// <summary>
            /// 前一个节点
            /// </summary>
            public node Previous;
            /// <summary>
            /// 后一个节点
            /// </summary>
            public node Next;
            /// <summary>
            /// 键值
            /// </summary>
            public keyType Key;
            /// <summary>
            /// 数据
            /// </summary>
            public valueType Value;
        }
        /// <summary>
        /// 数据集合
        /// </summary>
        private Dictionary<keyType, node> values = dictionary<keyType>.Create<node>();
        /// <summary>
        /// 数据数量
        /// </summary>
        public int Count
        {
            get { return values.Count; }
        }
        /// <summary>
        /// 头节点
        /// </summary>
        private node header;
        /// <summary>
        /// 尾节点
        /// </summary>
        private node end;
        /// <summary>
        /// 数据对象
        /// </summary>
        /// <param name="key">查询键值</param>
        /// <returns>数据对象</returns>
        public valueType this[keyType key]
        {
            get
            {
                node node = getNode(ref key);
                return node != null ? node.Value : default(valueType);
            }
            set { Set(key, value); }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>数据对象</returns>
        private node getNode(ref keyType key)
        {
            node node;
            if (values.TryGetValue(key, out node))
            {
                if (node != end)
                {
                    node previous = node.Previous;
                    if (previous == null) (header = node.Next).Previous = null;
                    else (previous.Next = node.Next).Previous = previous;
                    end.Next = node;
                    node.Previous = end;
                    node.Next = null;
                    end = node;
                }
            }
            return node;
        }
        /// <summary>
        /// 判断是否存在键值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>是否存在键值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool ContainsKey(keyType key)
        {
            return values.ContainsKey(key);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="nullValue">失败空值</param>
        /// <returns>数据对象</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Get(keyType key, valueType nullValue)
        {
            node node = getNode(ref key);
            return node != null ? node.Value : nullValue;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="nullValue">失败空值</param>
        /// <returns>数据对象</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Get(ref keyType key, valueType nullValue)
        {
            node node = getNode(ref key);
            return node != null ? node.Value : nullValue;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value"></param>
        /// <returns>数据对象</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool TryGet(keyType key, out valueType value)
        {
            node node = getNode(ref key);
            if (node != null)
            {
                value = node.Value;
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取数据(不调整位置)
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value"></param>
        /// <returns>数据对象</returns>
        public bool TryGetOnly(keyType key, out valueType value)
        {
            node node;
            if (values.TryGetValue(key, out node))
            {
                value = node.Value;
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取数据(不调整位置)
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value"></param>
        /// <returns>数据对象</returns>
        internal bool TryGetOnlyCheck(ref keyType key, out valueType value)
        {
            node node;
            if (values.TryGetValue(key, out node) && key.Equals(node.Key))
            {
                value = node.Value;
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">数据对象</param>
        /// <returns>被替换的数据对象,没有返回default(valueType)</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Set(keyType key, valueType value)
        {
            return Set(ref key, value);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">数据对象</param>
        /// <returns>被替换的数据对象,没有返回default(valueType)</returns>
        public valueType Set(ref keyType key, valueType value)
        {
            node node = getNode(ref key);
            if (node != null)
            {
                valueType oldValue = node.Value;
                node.Value = value;
                return oldValue;
            }
            else
            {
                values.Add(key, node = new node { Value = value, Key = key, Previous = end });
                if (end == null) header = end = node;
                else
                {
                    end.Next = node;
                    end = node;
                }
                return default(valueType);
            }
        }
        /// <summary>
        /// 设置数据(不调整位置)
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">数据对象</param>
        public void SetOnly(keyType key, valueType value)
        {
            node node;
            if (values.TryGetValue(key, out node)) node.Value = value;
        }
        /// <summary>
        /// 弹出一个键值对
        /// </summary>
        /// <returns>键值对</returns>
        public keyValue<keyType, valueType> Pop()
        {
            if (header != null) return UnsafePop();
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return default(keyValue<keyType, valueType>);
        }
        /// <summary>
        /// 弹出一个键值对
        /// </summary>
        /// <returns>键值对</returns>
        public keyValue<keyType, valueType> UnsafePop()
        {
            node node = header;
            if ((header = header.Next) == null) end = null;
            else header.Previous = null;
            values.Remove(node.Key);
            return new keyValue<keyType, valueType>(node.Key, node.Value);
        }
        /// <summary>
        /// 弹出一个值
        /// </summary>
        /// <returns>值</returns>
        public valueType UnsafePopValue()
        {
            node node = header;
            if ((header = header.Next) == null) end = null;
            else header.Previous = null;
            values.Remove(node.Key);
            return node.Value;
        }
        /// <summary>
        /// 清除数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Clear()
        {
            values.Clear();
            header = end = null;
        }
        /// <summary>
        /// 删除一个数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">被删除数据对象</param>
        /// <returns>是否删除了数据对象</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Remove(keyType key, out valueType value)
        {
            return Remove(ref key, out value);
        }
        /// <summary>
        /// 删除一个数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">被删除数据对象</param>
        /// <returns>是否删除了数据对象</returns>
        public bool Remove(ref keyType key, out valueType value)
        {
            node node;
            if (values.TryGetValue(key, out node))
            {
                values.Remove(key);
                if (node.Previous == null)
                {
                    header = node.Next;
                    if (header == null) end = null;
                    else header.Previous = null;
                }
                else if (node.Next == null) (end = node.Previous).Next = null;
                else
                {
                    node.Previous.Next = node.Next;
                    node.Next.Previous = node.Previous;
                }
                value = node.Value;
                return true;
            }
            value = default(valueType);
            return false;
        }
    }
}