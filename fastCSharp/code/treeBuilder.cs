using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp.code
{
    /// <summary>
    /// 建树器
    /// </summary>
    /// <typeparam name="nodeType">树节点类型</typeparam>
    /// <typeparam name="tagType">树节点标识类型</typeparam>
    public sealed class treeBuilder<nodeType, tagType>
        where nodeType : treeBuilder<nodeType, tagType>.INode
        where tagType : IEquatable<tagType>
    {
        /// <summary>
        /// 节点接口
        /// </summary>
        public interface INode
        {
            /// <summary>
            /// 树节点标识
            /// </summary>
            tagType Tag { get; }
            /// <summary>
            /// 设置子节点集合
            /// </summary>
            /// <param name="childs">子节点集合</param>
            void SetChilds(nodeType[] childs);
        }
        /// <summary>
        /// 当前节点集合
        /// </summary>
        private list<keyValue<nodeType, bool>> nodes = new list<keyValue<nodeType, bool>>();
        ///// <summary>
        ///// 树节点回合检测器,必须回合返回true
        ///// </summary>
        //private func<nodeType, bool> checkRound;
        /// <summary>
        /// 建树器
        /// </summary>
        public treeBuilder() { }
        ///// <summary>
        ///// 建树器
        ///// </summary>
        ///// <param name="checkRound">树节点回合检测器,必须回合返回true</param>
        //public treeBuilder(func<nodeType, bool> checkRound)
        //{
        //    this.checkRound = checkRound;
        //}
        /// <summary>
        /// 清除节点
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Empty()
        {
            nodes.Empty();
        }
        /// <summary>
        /// 追加新节点
        /// </summary>
        /// <param name="node">新节点</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Append(nodeType node)
        {
            nodes.Add(new keyValue<nodeType, bool>(node, true));
        }
        /// <summary>
        /// 追加新节点
        /// </summary>
        /// <param name="node">新节点</param>
        /// <param name="isRound">是否需要判断回合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Append(nodeType node, bool isRound)
        {
            nodes.Add(new keyValue<nodeType, bool>(node, isRound));
        }
        /// <summary>
        /// 检测节点回合状态
        /// </summary>
        private enum checkType
        {
            /// <summary>
            /// 节点回合成功
            /// </summary>
            Ok,
            /// <summary>
            /// 缺少回合节点
            /// </summary>
            LessRound,
            /// <summary>
            /// 未知的回合节点
            /// </summary>
            UnknownRound,
        }
        /// <summary>
        /// 节点回合
        /// </summary>
        /// <param name="tagName">树节点标识</param>
        /// <param name="isAny"></param>
        /// <returns>节点回合状态</returns>
        private checkType round(tagType tagName, bool isAny)
        {
            keyValue<nodeType, bool>[] array = nodes.array;
            for (int index = nodes.length; index != 0; )
            {
                if (array[--index].Value)
                {
                    nodeType node = array[index].Key;
                    if (tagName.Equals(node.Tag))
                    {
                        array[index].Set(node, false);
                        node.SetChilds(array.sub(++index, nodes.length - index).GetArray(value => value.Key));
                        nodes.UnsafeAddLength(index - nodes.length);
                        return checkType.Ok;
                    }
                    else if (!isAny) return checkType.LessRound;
                    //else if (checkRound != null && checkRound(node)) return checkType.LessRound;
                }
            }
            return checkType.UnknownRound;
        }
        /// <summary>
        /// 节点回合
        /// </summary>
        /// <param name="tagName">树节点标识</param>
        public void Round(tagType tagName)
        {
            checkType check = round(tagName, false);
            switch (check)
            {
                case checkType.LessRound:
                    log.Error.Throw("缺少回合节点 : " + tagName.ToString() + @"
" + nodes.JoinString(@"
", value => value.Key.Tag.ToString()), new System.Diagnostics.StackFrame(), false);
                    break;
                case checkType.UnknownRound:
                    log.Error.Throw("未知的回合节点 : " + tagName.ToString() + @"
" + nodes.JoinString(@"
", value => value.Key.Tag.ToString()), new System.Diagnostics.StackFrame(), false);
                    break;
            }
        }
        /// <summary>
        /// 节点回合
        /// </summary>
        /// <param name="tagName">树节点标识</param>
        /// <param name="isAny">是否匹配任意索引位置,否则只能匹配最后一个索引位置</param>
        /// <returns>节点回合是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool IsRound(tagType tagName, bool isAny)
        {
            return round(tagName, isAny) == checkType.Ok;
        }
        /// <summary>
        /// 建树结束
        /// </summary>
        /// <returns>根节点集合</returns>
        public nodeType[] End()
        {
            //if (checkRound != null)
            //{
                keyValue<nodeType, bool>[] array = nodes.array;
                for (int index = nodes.length; index != 0; )
                {
                    //if (array[--index].Value && checkRound(array[index].Key))
                    if (array[--index].Value)
                    {
                        log.Error.Throw("缺少回合节点 : " + nodes.JoinString(@"
", value => value.Key.Tag.ToString()), new System.Diagnostics.StackFrame(), false);
                    }
                }
            //}
            return nodes.GetArray(value => value.Key);
        }
    }
}
