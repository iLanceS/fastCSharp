using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 基于叔侄节点数量比较与旋转概率选择的非严格平衡查找二叉树
    /// </summary>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="valueType">数据类型</typeparam>
    public sealed class searchTree<keyType, valueType> where keyType : IComparable<keyType>
    {
        /// <summary>
        /// 二叉树节点
        /// </summary>
        internal sealed class node
        {
            /// <summary>
            /// 左节点
            /// </summary>
            public node Left;
            /// <summary>
            /// 右节点
            /// </summary>
            public node Right;
            /// <summary>
            /// 节点数量
            /// </summary>
            public int Count;
            /// <summary>
            /// 关键字
            /// </summary>
            public keyType Key;
            /// <summary>
            /// 节点数据
            /// </summary>
            public valueType Value;
            /// <summary>
            /// 二叉树节点
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="value">节点数据</param>
            public node(keyType key, valueType value)
            {
                Key = key;
                Value = value;
                Count = 1;
            }
            /// <summary>
            /// 二叉树节点
            /// </summary>
            /// <param name="count">节点数量</param>
            internal node(int count)
            {
                Count = count;
            }
            /// <summary>
            /// 删除当前节点
            /// </summary>
            public void Remove()
            {
                if (--Count != 0)
                {
                    if (Left != null)
                    {
                        if (Right != null)
                        {
                            if (Right.Count > Left.Count)
                            {
                                if (Right.Left != null)
                                {
                                    node father = Right, node = Right.Left;
                                    for (--Right.Count; node.Left != null; --father.Count) node = (father = node).Left;
                                    Key = node.Key;
                                    Value = node.Value;
                                    if (node.Count == 1) father.Left = null;
                                    else node.Remove();
                                }
                                else
                                {
                                    Key = Right.Key;
                                    Value = Right.Value;
                                    Right = Right.Right;
                                }
                            }
                            else
                            {
                                if (Left.Right != null)
                                {
                                    node father = Left, node = Left.Right;
                                    for (--Left.Count; node.Right != null; --father.Count) node = (father = node).Right;
                                    Key = node.Key;
                                    Value = node.Value;
                                    if (node.Count == 1) father.Right = null;
                                    else node.Remove();
                                }
                                else
                                {
                                    Key = Left.Key;
                                    Value = Left.Value;
                                    Left = Left.Left;
                                }
                            }
                        }
                        else copyForm(Left);
                    }
                    else copyForm(Right);
                }
            }
            /// <summary>
            /// 复制节点数据(不包括Count)
            /// </summary>
            /// <param name="node">被复制节点</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void copyForm(node node)
            {
                Left = node.Left;
                Right = node.Right;
                Key = node.Key;
                Value = node.Value;
            }
            /// <summary>
            /// 节点关键字交换
            /// </summary>
            /// <param name="node">交换接单</param>
            private void changeKey(node node)
            {
                keyType key = Key;
                valueType value = Value;
                Key = node.Key;
                Value = node.Value;
                node.Key = key;
                node.Value = value;
            }
            /// <summary>
            /// 旋转子节点
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void CheckChange()
            {
                if (Count >= 3)
                {
                    if (Left != null)
                    {
                        if (Right != null) checkChange();
                        else leftToNull();
                    }
                    else rightToNull();
                }
            }
            /// <summary>
            /// 旋转子节点
            /// </summary>
            private void checkChange()
            {
                if (Left.Count > Right.Count)
                {
                    if (Left.Left == null || Left.Right == null) Left.CheckChange();
                    else if (Right.Count == 1) leftToRightNull();
                    else
                    {
                        node leftLeft = Left.Left, leftRight = Left.Right;
                        if (leftLeft.Count > leftRight.Count)
                        {
                            if (leftLeft.Count > Right.Count)
                            {
                                if (leftLeft.Left == null || leftLeft.Right == null) leftLeft.CheckChange();
                                leftLeftToRight();
                            }
                        }
                        else if (leftRight.Count > Right.Count)
                        {
                            if (leftRight.Left == null || leftRight.Right == null) leftRight.CheckChange();
                            leftRightToRight();
                        }
                    }
                }
                else
                {
                    if (Right.Right == null || Right.Left == null) Right.CheckChange();
                    else if (Left.Count == 1) rightToLeftNull();
                    else
                    {
                        node rightRight = Right.Right, rightLeft = Right.Left;
                        if (rightRight.Count > rightLeft.Count)
                        {
                            if (rightRight.Count > Left.Count)
                            {
                                if (rightRight.Right == null || rightRight.Left == null) rightRight.CheckChange();
                                rightRightToLeft();
                            }
                        }
                        else if (rightLeft.Count > Left.Count)
                        {
                            if (rightLeft.Right == null || rightLeft.Left == null) rightLeft.CheckChange();
                            rightLeftToLeft();
                        }
                    }
                }
            }
            /// <summary>
            /// 旋转子节点
            /// </summary>
            /// <returns>是否旋转子节点</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private bool isCheckChange()
            {
                if (Count >= 3)
                {
                    if (Left != null)
                    {
                        if (Right != null) return isCheckChangeNotNull();
                        leftToNull();
                    }
                    else rightToNull();
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 旋转子节点
            /// </summary>
            /// <returns>是否旋转子节点</returns>
            private bool isCheckChangeNotNull()
            {
                if (Left.Count > Right.Count)
                {
                    if (Left.Left == null || Left.Right == null) return Left.isCheckChange();
                    if (Right.Count == 1)
                    {
                        leftToRightNull();
                        return true;
                    }
                    node leftLeft = Left.Left, leftRight = Left.Right;
                    if (leftLeft.Count > leftRight.Count)
                    {
                        if (leftLeft.Count > Right.Count)
                        {
                            if (leftLeft.Left == null || leftLeft.Right == null) return leftLeft.isCheckChange();
                            leftLeftToRight();
                            return true;
                        }
                    }
                    else if (leftRight.Count > Right.Count)
                    {
                        if (leftRight.Left == null || leftRight.Right == null) return leftRight.isCheckChange();
                        leftRightToRight();
                        return true;
                    }
                }
                else
                {
                    if (Right.Right == null || Right.Left == null) return Right.isCheckChange();
                    if (Left.Count == 1)
                    {
                        rightToLeftNull();
                        return true;
                    }
                    node rightRight = Right.Right, rightLeft = Right.Left;
                    if (rightRight.Count > rightLeft.Count)
                    {
                        if (rightRight.Count > Left.Count)
                        {
                            if (rightRight.Right == null || rightRight.Left == null) return rightRight.isCheckChange();
                            rightRightToLeft();
                            return true;
                        }
                    }
                    else if (rightLeft.Count > Left.Count)
                    {
                        if (rightLeft.Right == null || rightLeft.Left == null) return rightLeft.isCheckChange();
                        rightLeftToLeft();
                        return true;
                    }
                }
                return false;
            }
            /// <summary>
            /// 右节点为null
            /// </summary>
            private void leftToNull()
            {
                if (Left.Left == null)
                {
                    #region 左节点为null
                    node leftRight = Left.Right;
                    changeKey(leftRight);
                    Right = leftRight;
                    Left.Right = leftRight.Left;
                    if (leftRight.Left != null) leftRight.Count -= leftRight.Left.Count;
                    leftRight.Left = leftRight.Right;
                    Left.Count -= leftRight.Count;
                    leftRight.Right = null;
                    (Left.Count > leftRight.Count ? Left : leftRight).CheckChange();
                    #endregion
                }
                else if (Left.Right == null)
                {
                    #region 右节点为null
                    node leftLeft = Left.Left;
                    changeKey(Left);
                    Right = Left;
                    Left = leftLeft;
                    Right.Left = null;
                    Right.Count = 1;
                    checkChange();
                    #endregion
                }
                else
                {
                    #region 分裂左节点
                    node left = Left, leftLeft = Left.Left, leftRight = Left.Right;
                    changeKey(Left);
                    Right = leftRight;
                    Left = leftLeft;
                    left.Left = leftRight.Right;
                    left.Count = leftRight.Right == null ? 1 : (leftRight.Right.Count + 1);
                    left.Right = null;
                    ++leftRight.Count;
                    leftRight.Right = left;
                    left.CheckChange();
                    #endregion
                }
            }
            /// <summary>
            /// 左节点为null
            /// </summary>
            private void rightToNull()
            {
                if (Right.Right == null)
                {
                    #region 右节点为null
                    node rightLeft = Right.Left;
                    changeKey(rightLeft);
                    Left = rightLeft;
                    Right.Left = rightLeft.Right;
                    if (rightLeft.Right != null) rightLeft.Count -= rightLeft.Right.Count;
                    rightLeft.Right = rightLeft.Left;
                    Right.Count -= rightLeft.Count;
                    rightLeft.Left = null;
                    (Right.Count > rightLeft.Count ? Right : rightLeft).CheckChange();
                    #endregion
                }
                else if (Right.Left == null)
                {
                    #region 左节点为null
                    node rightRight = Right.Right;
                    changeKey(Right);
                    Left = Right;
                    Right = rightRight;
                    Left.Right = null;
                    Left.Count = 1;
                    checkChange();
                    #endregion
                }
                else
                {
                    #region 分裂右节点
                    node right = Right, rightRight = Right.Right, rightLeft = Right.Left;
                    changeKey(Right);
                    Left = rightLeft;
                    Right = rightRight;
                    right.Right = rightLeft.Left;
                    right.Count = rightLeft.Left == null ? 1 : (rightLeft.Left.Count + 1);
                    right.Left = null;
                    ++rightLeft.Count;
                    rightLeft.Left = right;
                    right.CheckChange();
                    #endregion
                }
            }
            /// <summary>
            /// 左节点数量大于右节点数量
            /// </summary>
            private void leftToRightNull()
            {
                node leftLeft = Left.Left, leftRight = Left.Right;
                int leftRightRightCount = leftRight.Right != null ? leftRight.Right.Count : 0;
                changeKey(Left);
                int leftRightLeftCount = leftRight.Count - leftRightRightCount - 1;
                Left.Left = leftRight.Right;
                Left.Right = Right;
                Left.Count = leftRightRightCount + 2;
                leftRight.Right = Left;
                leftRight.Count += 2;
                Right = leftRight;
                Left = leftLeft;
                if (leftRightLeftCount > leftRightRightCount)
                {
                    if (!Right.Right.isCheckChange()) Right.checkChange();
                }
                else Right.Right.CheckChange();
            }
            /// <summary>
            /// 右节点数量大于左节点数量
            /// </summary>
            private void rightToLeftNull()
            {
                node rightRight = Right.Right, rightLeft = Right.Left;
                int rightLeftLeftCount = rightLeft.Left != null ? rightLeft.Left.Count : 0;
                changeKey(Right);
                int rightLeftRightCount = rightLeft.Count - rightLeftLeftCount - 1;
                Right.Right = rightLeft.Left;
                Right.Left = Left;
                Right.Count = rightLeftLeftCount + 2;
                rightLeft.Left = Right;
                rightLeft.Count += 2;
                Left = rightLeft;
                Right = rightRight;
                if (rightLeftRightCount > rightLeftLeftCount)
                {
                    if (!Left.Left.isCheckChange()) Left.checkChange();
                }
                else Left.Left.CheckChange();
            }
            /// <summary>
            /// 左节点的左子节点数量大于右节点数量
            /// </summary>
            private void leftLeftToRight()
            {
                node leftRight = Left.Right, leftLeft = Left.Left;
                changeKey(leftRight);
                if (Right.Left != null && Right.Right != null)
                {
                    node rightLeft = Right.Left, rightRight = Right.Right, leftLeftRight = leftLeft.Right;
                    int leftRightLeftCount = leftRight.Left != null ? leftRight.Left.Count : 0;
                    if (leftLeft.Left.Count > leftLeftRight.Count)
                    {
                        if (rightLeft.Count < rightRight.Count)
                        {
                            #region
                            Left.Right = leftRight.Left;
                            int leftRightRightCount = leftRight.Count - leftRightLeftCount - 1;
                            leftLeft.Count += leftRightLeftCount + 1;
                            Right.Left = leftRight;
                            Right.Count += leftRightRightCount + 1;
                            Left.Left = leftLeftRight;
                            leftRight.Count += rightLeft.Count - leftRightLeftCount;
                            Left.Count = leftRightLeftCount + leftLeftRight.Count + 1;
                            leftRight.Left = leftRight.Right;
                            leftLeft.Right = Left;
                            leftRight.Right = rightLeft;
                            Left = leftLeft;
                            if (!leftLeft.isCheckChangeNotNull() && !Right.isCheckChangeNotNull()) leftRight.CheckChange();
                            #endregion
                        }
                        else
                        {
                            #region
                            int rightLeftLeftCount = rightLeft.Left != null ? rightLeft.Left.Count : 0;
                            Right.Left = rightLeft.Right;
                            Left.Right = leftRight.Left;
                            leftLeft.Count += leftRightLeftCount + 1;
                            Left.Left = leftLeftRight;
                            leftRight.Left = leftRight.Right;
                            Right.Count -= rightLeftLeftCount + 1;
                            rightLeft.Right = Right;
                            leftRight.Count += rightLeftLeftCount - leftRightLeftCount;
                            Left.Count = leftRightLeftCount + leftLeftRight.Count + 1;
                            leftRight.Right = rightLeft.Left;
                            rightLeft.Count = leftRight.Count + Right.Count + 1;
                            leftLeft.Right = Left;
                            rightLeft.Left = leftRight;
                            Right = rightLeft;
                            Left = leftLeft;
                            if (!rightLeft.isCheckChangeNotNull() && !Right.Right.isCheckChange() && !leftLeft.isCheckChangeNotNull())
                            {
                                leftRight.CheckChange();
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        int leftLeftRightRightCount = leftLeftRight.Right != null ? leftLeftRight.Right.Count : 0;
                        int leftLeftRightCount = leftLeftRight.Count;
                        if (rightLeft.Count < rightRight.Count)
                        {
                            #region
                            Left.Count = leftLeftRightRightCount + leftRightLeftCount + 1;
                            Left.Left = leftLeftRight.Right;
                            leftRight.Count += rightLeft.Count - leftRightLeftCount;
                            Right.Left = leftRight;
                            leftLeft.Count -= leftLeftRightRightCount + 1;
                            leftLeft.Right = leftLeftRight.Left;
                            Right.Count = leftRight.Count + rightRight.Count + 1;
                            Left.Right = leftRight.Left;
                            leftLeftRight.Count = Left.Count + leftLeft.Count + 1;
                            leftLeftRight.Right = Left;
                            leftRight.Left = leftRight.Right;
                            leftLeftRight.Left = leftLeft;
                            leftRight.Right = rightLeft;
                            Left = leftLeftRight;
                            if (!leftRight.isCheckChange())
                            {
                                if (leftLeft.Left.Count - (leftLeftRightCount - leftLeftRightRightCount - 1) > leftRightLeftCount - leftLeftRightRightCount)
                                {
                                    if (!leftLeft.isCheckChange()) Left.Right.CheckChange();
                                }
                                else if (!Left.Right.isCheckChange()) leftLeft.CheckChange();
                            }
                            #endregion
                        }
                        else
                        {
                            #region
                            int rightLeftLeftCount = rightLeft.Left != null ? rightLeft.Left.Count : 0;
                            Right.Left = rightLeft.Right;
                            Left.Count = leftRightLeftCount + leftLeftRightRightCount + 1;
                            leftLeft.Right = leftLeftRight.Left;
                            Right.Count -= rightLeftLeftCount + 1;
                            rightLeft.Right = Right;
                            leftLeft.Count -= leftLeftRightRightCount + 1;
                            Left.Left = leftLeftRight.Right;
                            leftRight.Count += rightLeftLeftCount - leftRightLeftCount;
                            Left.Right = leftRight.Left;
                            leftLeftRight.Left = leftLeft;
                            leftLeftRight.Count = leftLeft.Count + Left.Count + 1;
                            leftRight.Left = leftRight.Right;
                            leftLeftRight.Right = Left;
                            rightLeft.Count = leftRight.Count + Right.Count + 1;
                            leftRight.Right = rightLeft.Left;
                            Right = rightLeft;
                            rightLeft.Left = leftRight;
                            Left = leftLeftRight;
                            if (!rightLeft.isCheckChangeNotNull() && !Right.Right.isCheckChange())
                            {
                                if (leftLeft.Left.Count - (leftLeftRightCount - leftLeftRightRightCount - 1) > leftRightLeftCount - leftLeftRightRightCount)
                                {
                                    if (!leftLeft.isCheckChange()) Left.Right.CheckChange();
                                }
                                else if (!Left.Right.isCheckChange()) leftLeft.CheckChange();
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    int leftRightLeftCount = leftRight.Left != null ? leftRight.Left.Count : 0, leftLeftRightCount = leftLeft.Right.Count;
                    if (Right.Left == null)
                    {
                        #region
                        Right.Left = leftRight;
                        leftRight.Count -= leftRightLeftCount;
                        Left.Right = leftRight.Left;
                        Right.Count += leftRight.Count;
                        leftRight.Left = leftRight.Right;
                        leftRight.Right = null;
                        Left.Count = leftRightLeftCount + leftLeftRightCount + 1;
                        leftLeft.Count += leftRightLeftCount + 1;
                        Left.Left = leftLeft.Right;
                        leftLeft.Right = Left;
                        Left = leftLeft;
                        if (!leftRight.isCheckChange() && !Right.isCheckChangeNotNull()) Left.checkChange();
                        #endregion
                    }
                    else
                    {
                        #region
                        node rightLeft = Right.Left;
                        int rightLeftLeftCount = rightLeft.Left != null ? rightLeft.Left.Count : 0;
                        Right.Left = rightLeft.Right;
                        Right.Count -= rightLeftLeftCount + 1;
                        Left.Right = leftRight.Left;
                        leftRight.Count += rightLeftLeftCount - leftRightLeftCount;
                        rightLeft.Right = Right;
                        rightLeft.Count = Right.Count + leftRight.Count + 1;
                        leftRight.Left = leftRight.Right;
                        leftLeft.Count += leftRightLeftCount + 1;
                        Left.Left = leftLeft.Right;
                        Left.Count = leftRightLeftCount + leftLeftRightCount + 1;
                        leftRight.Right = rightLeft.Left;
                        leftLeft.Right = Left;
                        rightLeft.Left = leftRight;
                        Right = rightLeft;
                        Left = leftLeft;
                        if (!Right.Right.isCheckChange() && !Right.isCheckChangeNotNull()) Left.checkChange();
                        #endregion
                    }
                }
            }
            /// <summary>
            /// 右节点的右子节点数量大于左节点数量
            /// </summary>
            private void rightRightToLeft()
            {
                node rightLeft = Right.Left, rightRight = Right.Right;
                changeKey(rightLeft);
                if (Left.Right != null && Left.Left != null)
                {
                    node leftRight = Left.Right, leftLeft = Left.Left, rightRightLeft = rightRight.Left;
                    int rightLeftRightCount = rightLeft.Right != null ? rightLeft.Right.Count : 0;
                    if (rightRight.Right.Count > rightRightLeft.Count)
                    {
                        if (leftRight.Count < leftLeft.Count)
                        {
                            #region
                            Right.Left = rightLeft.Right;
                            int rightLeftLeftCount = rightLeft.Count - rightLeftRightCount - 1;
                            rightRight.Count += rightLeftRightCount + 1;
                            Left.Right = rightLeft;
                            Left.Count += rightLeftLeftCount + 1;
                            Right.Right = rightRightLeft;
                            rightLeft.Count += leftRight.Count - rightLeftRightCount;
                            Right.Count = rightLeftRightCount + rightRightLeft.Count + 1;
                            rightLeft.Right = rightLeft.Left;
                            rightRight.Left = Right;
                            rightLeft.Left = leftRight;
                            Right = rightRight;
                            if (!rightRight.isCheckChangeNotNull() && !Left.isCheckChangeNotNull()) rightLeft.CheckChange();
                            #endregion
                        }
                        else
                        {
                            #region
                            int leftRightRightCount = leftRight.Right != null ? leftRight.Right.Count : 0;
                            Left.Right = leftRight.Left;
                            Right.Left = rightLeft.Right;
                            rightRight.Count += rightLeftRightCount + 1;
                            Right.Right = rightRightLeft;
                            rightLeft.Right = rightLeft.Left;
                            Left.Count -= leftRightRightCount + 1;
                            leftRight.Left = Left;
                            rightLeft.Count += leftRightRightCount - rightLeftRightCount;
                            Right.Count = rightLeftRightCount + rightRightLeft.Count + 1;
                            rightLeft.Left = leftRight.Right;
                            leftRight.Count = rightLeft.Count + Left.Count + 1;
                            rightRight.Left = Right;
                            leftRight.Right = rightLeft;
                            Left = leftRight;
                            Right = rightRight;
                            if (!leftRight.isCheckChangeNotNull() && !Left.Left.isCheckChange() && !rightRight.isCheckChangeNotNull())
                            {
                                rightLeft.CheckChange();
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        int rightRightLeftLeftCount = rightRightLeft.Left != null ? rightRightLeft.Left.Count : 0;
                        int rightRightLeftCount = rightRightLeft.Count;
                        if (leftRight.Count < leftLeft.Count)
                        {
                            #region
                            Right.Count = rightRightLeftLeftCount + rightLeftRightCount + 1;
                            Right.Right = rightRightLeft.Left;
                            rightLeft.Count += leftRight.Count - rightLeftRightCount;
                            Left.Right = rightLeft;
                            rightRight.Count -= rightRightLeftLeftCount + 1;
                            rightRight.Left = rightRightLeft.Right;
                            Left.Count = rightLeft.Count + leftLeft.Count + 1;
                            Right.Left = rightLeft.Right;
                            rightRightLeft.Count = Right.Count + rightRight.Count + 1;
                            rightRightLeft.Left = Right;
                            rightLeft.Right = rightLeft.Left;
                            rightRightLeft.Right = rightRight;
                            rightLeft.Left = leftRight;
                            Right = rightRightLeft;
                            if (!rightLeft.isCheckChange())
                            {
                                if (rightRight.Right.Count - (rightRightLeftCount - rightRightLeftLeftCount - 1) > rightLeftRightCount - rightRightLeftLeftCount)
                                {
                                    if (!rightRight.isCheckChange()) Right.Left.CheckChange();
                                }
                                else if (!Right.Left.isCheckChange()) rightRight.CheckChange();
                            }
                            #endregion
                        }
                        else
                        {
                            #region
                            int leftRightRightCount = leftRight.Right != null ? leftRight.Right.Count : 0;
                            Left.Right = leftRight.Left;
                            Right.Count = rightLeftRightCount + rightRightLeftLeftCount + 1;
                            rightRight.Left = rightRightLeft.Right;
                            Left.Count -= leftRightRightCount + 1;
                            leftRight.Left = Left;
                            rightRight.Count -= rightRightLeftLeftCount + 1;
                            Right.Right = rightRightLeft.Left;
                            rightLeft.Count += leftRightRightCount - rightLeftRightCount;
                            Right.Left = rightLeft.Right;
                            rightRightLeft.Right = rightRight;
                            rightRightLeft.Count = rightRight.Count + Right.Count + 1;
                            rightLeft.Right = rightLeft.Left;
                            rightRightLeft.Left = Right;
                            leftRight.Count = rightLeft.Count + Left.Count + 1;
                            rightLeft.Left = leftRight.Right;
                            Left = leftRight;
                            leftRight.Right = rightLeft;
                            Right = rightRightLeft;
                            if (!leftRight.isCheckChangeNotNull() && !Left.Left.isCheckChange())
                            {
                                if (rightRight.Right.Count - (rightRightLeftCount - rightRightLeftLeftCount - 1) > rightLeftRightCount - rightRightLeftLeftCount)
                                {
                                    if (!rightRight.isCheckChange()) Right.Left.CheckChange();
                                }
                                else if (!Right.Left.isCheckChange()) rightRight.CheckChange();
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    int rightLeftRightCount = rightLeft.Right != null ? rightLeft.Right.Count : 0, rightRightLeftCount = rightRight.Left.Count;
                    if (Left.Right == null)
                    {
                        #region
                        Left.Right = rightLeft;
                        rightLeft.Count -= rightLeftRightCount;
                        Right.Left = rightLeft.Right;
                        Left.Count += rightLeft.Count;
                        rightLeft.Right = rightLeft.Left;
                        rightLeft.Left = null;
                        Right.Count = rightLeftRightCount + rightRightLeftCount + 1;
                        rightRight.Count += rightLeftRightCount + 1;
                        Right.Right = rightRight.Left;
                        rightRight.Left = Right;
                        Right = rightRight;
                        if (!rightLeft.isCheckChange() && !Left.isCheckChangeNotNull()) Right.checkChange();
                        #endregion
                    }
                    else
                    {
                        #region
                        node leftRight = Left.Right;
                        int leftRightRightCount = leftRight.Right != null ? leftRight.Right.Count : 0;
                        Left.Right = leftRight.Left;
                        Left.Count -= leftRightRightCount + 1;
                        Right.Left = rightLeft.Right;
                        rightLeft.Count += leftRightRightCount - rightLeftRightCount;
                        leftRight.Left = Left;
                        leftRight.Count = Left.Count + rightLeft.Count + 1;
                        rightLeft.Right = rightLeft.Left;
                        rightRight.Count += rightLeftRightCount + 1;
                        Right.Right = rightRight.Left;
                        Right.Count = rightLeftRightCount + rightRightLeftCount + 1;
                        rightLeft.Left = leftRight.Right;
                        rightRight.Left = Right;
                        leftRight.Right = rightLeft;
                        Left = leftRight;
                        Right = rightRight;
                        if (!Left.Left.isCheckChange() && !Left.isCheckChangeNotNull()) Right.checkChange();
                        #endregion
                    }
                }
            }
            /// <summary>
            /// 左节点的右子节点数量大于右节点数量
            /// </summary>
            private void leftRightToRight()
            {
                node leftRight = Left.Right, leftLeft = Left.Left, leftRightRight = leftRight.Right;
                int leftRightLeftCount = leftRight.Left.Count, leftRightRightCount = leftRightRight.Count;
                if (Right.Left != null && Right.Right != null)
                {
                    node rightLeft = Right.Left, rightRight = Right.Right;
                    if (leftRightLeftCount > leftRightRightCount)
                    {
                        changeKey(leftRight);
                        ++leftRightRightCount;
                        if (rightLeft.Count < rightRight.Count)
                        {
                            #region
                            Left.Right = leftRight.Left;
                            leftRight.Count += rightLeft.Count - leftRightLeftCount;
                            leftRight.Left = leftRightRight;
                            Left.Count -= leftRightRightCount;
                            Right.Left = leftRight;
                            Right.Count += leftRightRightCount;
                            leftRight.Right = rightLeft;
                            Left.checkChange();
                            #endregion
                        }
                        else
                        {
                            #region
                            int rightLeftLeftCount = rightLeft.Left != null ? rightLeft.Left.Count : 0;
                            Left.Right = leftRight.Left;
                            rightLeft.Count = Right.Count + leftRightRightCount;
                            Right.Left = rightLeft.Right;
                            Left.Count -= leftRightRightCount;
                            leftRight.Left = leftRightRight;
                            leftRight.Count += rightLeftLeftCount - leftRightLeftCount;
                            rightLeft.Right = Right;
                            Right.Count -= rightLeftLeftCount + 1;
                            leftRight.Right = rightLeft.Left;
                            Right = rightLeft;
                            rightLeft.Left = leftRight;
                            if (!Left.isCheckChangeNotNull()) Right.Right.CheckChange();
                            #endregion
                        }
                    }
                    else
                    {
                        if (leftRightRight.Left == null || leftRightRight.Right == null) leftRightRight.CheckChange();
                        else
                        {
                            int leftRightRightRightCount = leftRightRight.Right.Count + 1;
                            changeKey(leftRightRight);
                            if (rightLeft.Count < rightRight.Count)
                            {
                                #region
                                leftRightRight.Count -= leftRightRight.Left.Count;
                                leftRight.Right = leftRightRight.Left;
                                leftRight.Count -= leftRightRightRightCount;
                                Right.Count += leftRightRightRightCount;
                                leftRightRight.Left = leftRightRight.Right;
                                if (rightLeft.Left != null) leftRightRight.Count += rightLeft.Left.Count;
                                leftRightRight.Right = rightLeft.Left;
                                Left.Count -= leftRightRightRightCount;
                                rightLeft.Count += leftRightRightRightCount;
                                rightLeft.Left = leftRightRight;
                                if (!leftRight.isCheckChange() && !rightLeft.isCheckChange()) Right.checkChange();
                                #endregion
                            }
                            else
                            {
                                #region
                                int rightLeftLeftCount = rightLeft.Left != null ? rightLeft.Left.Count : 0;
                                leftRight.Right = leftRightRight.Left;
                                leftRight.Count -= leftRightRightRightCount;
                                leftRightRight.Count += rightLeftLeftCount;
                                Right.Left = rightLeft.Right;
                                if (leftRightRight.Left != null) leftRightRight.Count -= leftRightRight.Left.Count;
                                rightLeft.Count = Right.Count + leftRightRightRightCount;
                                leftRightRight.Left = leftRightRight.Right;
                                Left.Count -= leftRightRightRightCount;
                                rightLeft.Right = Right;
                                Right.Count -= rightLeftLeftCount + 1;
                                leftRightRight.Right = rightLeft.Left;
                                Right = rightLeft;
                                rightLeft.Left = leftRightRight;
                                if (!leftRight.isCheckChange() && !Right.isCheckChangeNotNull() && !Right.Right.isCheckChange()) leftRightRight.CheckChange();
                                #endregion
                            }
                        }
                    }
                }
                else
                {
                    if (leftRightLeftCount > leftRightRightCount)
                    {
                        changeKey(leftRight);
                        if (Right.Left == null)
                        {
                            #region
                            Left.Right = leftRight.Left;
                            leftRight.Count -= leftRightLeftCount;
                            leftRight.Left = leftRightRight;
                            Right.Count += leftRight.Count;
                            Right.Left = leftRight;
                            Left.Count = leftLeft.Count + leftRightLeftCount + 1;
                            leftRight.Right = null;
                            if (!leftRight.isCheckChange() && !Right.isCheckChangeNotNull()) Left.checkChange();
                            #endregion
                        }
                        else
                        {
                            #region
                            node rightLeft = Right.Left;
                            int rightLeftLeftCount = rightLeft.Left != null ? rightLeft.Left.Count : 0;
                            Left.Right = leftRight.Left;
                            leftRight.Count += rightLeftLeftCount - leftRightLeftCount;
                            Right.Left = rightLeft.Right;
                            Right.Count -= rightLeftLeftCount + 1;
                            leftRight.Left = leftRightRight;
                            Left.Count = leftLeft.Count + leftRightLeftCount + 1;
                            rightLeft.Right = Right;
                            rightLeft.Count = leftRight.Count + Right.Count + 1;
                            leftRight.Right = rightLeft.Left;
                            Right = rightLeft;
                            rightLeft.Left = leftRight;
                            if (!Right.Right.isCheckChange() && !Right.isCheckChangeNotNull()) Left.checkChange();
                            #endregion
                        }
                    }
                    else
                    {
                        if (leftRightRight.Left == null || leftRightRight.Right == null) leftRightRight.CheckChange();
                        else
                        {
                            changeKey(leftRightRight);
                            if (Right.Left == null)
                            {
                                #region
                                node rightRight = Right.Right;
                                leftRightRight.Count = leftRightRight.Right.Count + 1;
                                Right.Left = leftRightRight;
                                Right.Count = leftRightRight.Count + 1;
                                leftRight.Count -= leftRightRight.Count;
                                leftRight.Right = leftRightRight.Left;
                                rightRight.Count += Right.Count;
                                Left.Count -= leftRightRight.Count;
                                Right.Right = rightRight.Left;
                                if (rightRight.Left != null) Right.Count += rightRight.Left.Count;
                                leftRightRight.Left = leftRightRight.Right;
                                rightRight.Left = Right;
                                leftRightRight.Right = null;
                                Right = rightRight;
                                if (!leftRightRight.isCheckChange() && !Right.Left.isCheckChange()) Right.CheckChange();
                                #endregion
                            }
                            else
                            {
                                #region
                                node rightLeft = Right.Left;
                                int rightLeftLeftCount = rightLeft.Left != null ? rightLeft.Left.Count : 0;
                                leftRightRight.Count = leftRightRight.Right.Count + 1;
                                leftRight.Right = leftRightRight.Left;
                                leftRight.Count -= leftRightRight.Count;
                                Right.Left = rightLeft.Right;
                                Left.Count -= leftRightRight.Count;
                                leftRightRight.Left = leftRightRight.Right;
                                rightLeft.Count += leftRightRight.Count + 1;
                                rightLeft.Right = Right;
                                Right.Count -= rightLeftLeftCount + 1;
                                leftRightRight.Right = rightLeft.Left;
                                leftRightRight.Count += rightLeftLeftCount;
                                Right = rightLeft;
                                rightLeft.Left = leftRightRight;
                                if (!Right.Right.isCheckChange() && !leftRightRight.isCheckChange()) Right.checkChange();
                                #endregion
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// 右节点的左子节点数量大于左节点数量
            /// </summary>
            private void rightLeftToLeft()
            {
                node rightLeft = Right.Left, rightRight = Right.Right, rightLeftLeft = rightLeft.Left;
                int rightLeftRightCount = rightLeft.Right.Count, rightLeftLeftCount = rightLeftLeft.Count;
                if (Left.Right != null && Left.Left != null)
                {
                    node leftRight = Left.Right, leftLeft = Left.Left;
                    if (rightLeftRightCount > rightLeftLeftCount)
                    {
                        changeKey(rightLeft);
                        ++rightLeftLeftCount;
                        if (leftRight.Count < leftLeft.Count)
                        {
                            #region
                            Right.Left = rightLeft.Right;
                            rightLeft.Count += leftRight.Count - rightLeftRightCount;
                            rightLeft.Right = rightLeftLeft;
                            Right.Count -= rightLeftLeftCount;
                            Left.Right = rightLeft;
                            Left.Count += rightLeftLeftCount;
                            rightLeft.Left = leftRight;
                            Right.checkChange();
                            #endregion
                        }
                        else
                        {
                            #region
                            int leftRightRightCount = leftRight.Right != null ? leftRight.Right.Count : 0;
                            Right.Left = rightLeft.Right;
                            leftRight.Count = Left.Count + rightLeftLeftCount;
                            Left.Right = leftRight.Left;
                            Right.Count -= rightLeftLeftCount;
                            rightLeft.Right = rightLeftLeft;
                            rightLeft.Count += leftRightRightCount - rightLeftRightCount;
                            leftRight.Left = Left;
                            Left.Count -= leftRightRightCount + 1;
                            rightLeft.Left = leftRight.Right;
                            Left = leftRight;
                            leftRight.Right = rightLeft;
                            if (!Right.isCheckChangeNotNull()) Left.Left.CheckChange();
                            #endregion
                        }
                    }
                    else
                    {
                        if (rightLeftLeft.Right == null || rightLeftLeft.Left == null) rightLeftLeft.CheckChange();
                        else
                        {
                            int rightLeftLeftLeftCount = rightLeftLeft.Left.Count + 1;
                            changeKey(rightLeftLeft);
                            if (leftRight.Count < leftLeft.Count)
                            {
                                #region
                                rightLeftLeft.Count -= rightLeftLeft.Right.Count;
                                rightLeft.Left = rightLeftLeft.Right;
                                rightLeft.Count -= rightLeftLeftLeftCount;
                                Left.Count += rightLeftLeftLeftCount;
                                rightLeftLeft.Right = rightLeftLeft.Left;
                                if (leftRight.Right != null) rightLeftLeft.Count += leftRight.Right.Count;
                                rightLeftLeft.Left = leftRight.Right;
                                Right.Count -= rightLeftLeftLeftCount;
                                leftRight.Count += rightLeftLeftLeftCount;
                                leftRight.Right = rightLeftLeft;
                                if (!rightLeft.isCheckChange() && !leftRight.isCheckChange()) Left.checkChange();
                                #endregion
                            }
                            else
                            {
                                #region
                                int leftRightRightCount = leftRight.Right != null ? leftRight.Right.Count : 0;
                                rightLeft.Left = rightLeftLeft.Right;
                                rightLeft.Count -= rightLeftLeftLeftCount;
                                rightLeftLeft.Count += leftRightRightCount;
                                Left.Right = leftRight.Left;
                                if (rightLeftLeft.Right != null) rightLeftLeft.Count -= rightLeftLeft.Right.Count;
                                leftRight.Count = Left.Count + rightLeftLeftLeftCount;
                                rightLeftLeft.Right = rightLeftLeft.Left;
                                Right.Count -= rightLeftLeftLeftCount;
                                leftRight.Left = Left;
                                Left.Count -= leftRightRightCount + 1;
                                rightLeftLeft.Left = leftRight.Right;
                                Left = leftRight;
                                leftRight.Right = rightLeftLeft;
                                if (!rightLeft.isCheckChange() && !Left.isCheckChangeNotNull() && !Left.Left.isCheckChange()) rightLeftLeft.CheckChange();
                                #endregion
                            }
                        }
                    }
                }
                else
                {
                    if (rightLeftRightCount > rightLeftLeftCount)
                    {
                        changeKey(rightLeft);
                        if (Left.Right == null)
                        {
                            #region
                            Right.Left = rightLeft.Right;
                            rightLeft.Count -= rightLeftRightCount;
                            rightLeft.Right = rightLeftLeft;
                            Left.Count += rightLeft.Count;
                            Left.Right = rightLeft;
                            Right.Count = rightRight.Count + rightLeftRightCount + 1;
                            rightLeft.Left = null;
                            if (!rightLeft.isCheckChange() && !Left.isCheckChangeNotNull()) Right.checkChange();
                            #endregion
                        }
                        else
                        {
                            #region
                            node leftRight = Left.Right;
                            int leftRightRightCount = leftRight.Right != null ? leftRight.Right.Count : 0;
                            Right.Left = rightLeft.Right;
                            rightLeft.Count += leftRightRightCount - rightLeftRightCount;
                            Left.Right = leftRight.Left;
                            Left.Count -= leftRightRightCount + 1;
                            rightLeft.Right = rightLeftLeft;
                            Right.Count = rightRight.Count + rightLeftRightCount + 1;
                            leftRight.Left = Left;
                            leftRight.Count = rightLeft.Count + Left.Count + 1;
                            rightLeft.Left = leftRight.Right;
                            Left = leftRight;
                            leftRight.Right = rightLeft;
                            if (!Left.Left.isCheckChange() && !Left.isCheckChangeNotNull()) Right.checkChange();
                            #endregion
                        }
                    }
                    else
                    {
                        if (rightLeftLeft.Right == null || rightLeftLeft.Left == null) rightLeftLeft.CheckChange();
                        else
                        {
                            changeKey(rightLeftLeft);
                            if (Left.Right == null)
                            {
                                #region
                                node leftLeft = Left.Left;
                                rightLeftLeft.Count = rightLeftLeft.Left.Count + 1;
                                Left.Right = rightLeftLeft;
                                Left.Count = rightLeftLeft.Count + 1;
                                rightLeft.Count -= rightLeftLeft.Count;
                                rightLeft.Left = rightLeftLeft.Right;
                                leftLeft.Count += Left.Count;
                                Right.Count -= rightLeftLeft.Count;
                                Left.Left = leftLeft.Right;
                                if (leftLeft.Right != null) Left.Count += leftLeft.Right.Count;
                                rightLeftLeft.Right = rightLeftLeft.Left;
                                leftLeft.Right = Left;
                                rightLeftLeft.Left = null;
                                Left = leftLeft;
                                if (!rightLeftLeft.isCheckChange() && !Left.Right.isCheckChange()) Left.CheckChange();
                                #endregion
                            }
                            else
                            {
                                #region
                                node leftRight = Left.Right;
                                int leftRightRightCount = leftRight.Right != null ? leftRight.Right.Count : 0;
                                rightLeftLeft.Count = rightLeftLeft.Left.Count + 1;
                                rightLeft.Left = rightLeftLeft.Right;
                                rightLeft.Count -= rightLeftLeft.Count;
                                Left.Right = leftRight.Left;
                                Right.Count -= rightLeftLeft.Count;
                                rightLeftLeft.Right = rightLeftLeft.Left;
                                leftRight.Count += rightLeftLeft.Count + 1;
                                leftRight.Left = Left;
                                Left.Count -= leftRightRightCount + 1;
                                rightLeftLeft.Left = leftRight.Right;
                                rightLeftLeft.Count += leftRightRightCount;
                                Left = leftRight;
                                leftRight.Right = rightLeftLeft;
                                if (!Left.Left.isCheckChange() && !rightLeftLeft.isCheckChange()) Left.checkChange();
                                #endregion
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 关键字查找器
        /// </summary>
        internal unsafe struct keyFinder
        {
            /// <summary>
            /// 查找二叉树
            /// </summary>
            private searchTree<keyType, valueType> tree;
            /// <summary>
            /// 当前父节点
            /// </summary>
            private node father;
            /// <summary>
            /// 当前节点
            /// </summary>
            private node node;
            /// <summary>
            /// 当前节点
            /// </summary>
            internal node Node
            {
                get { return node; }
            }
            /// <summary>
            /// 当前路径位图
            /// </summary>
            private ulong pathMap;
            /// <summary>
            /// 当前路径位图2进制位数
            /// </summary>
            private int depth;
            /// <summary>
            /// 关键字查找器
            /// </summary>
            /// <param name="tree">查找二叉树</param>
            public keyFinder(searchTree<keyType, valueType> tree)
            {
                this.tree = tree;
                depth = 0;
                pathMap = 0;
                father = node = null;
            }
            /// <summary>
            /// 查找关键字
            /// </summary>
            /// <param name="key">关键字</param>
            /// <returns>是否找到关键字</returns>
            public bool Find(keyType key)
            {
                node node = tree.boot;
                int depth = 0;
                do
                {
                    int cmp = node.Key.CompareTo(key);
                    if (cmp == 0)
                    {
                        if (depth >= 64) throw new StackOverflowException();
                        this.node = node;
                        this.depth = depth;
                        return true;
                    }
                    father = node;
                    if (cmp > 0)
                    {
                        pathMap |= 1UL << depth;
                        node = node.Left;
                    }
                    else node = node.Right;
                    ++depth;
                }
                while (node != null);
                if (depth >= 64) throw new StackOverflowException();
                this.depth = depth;
                return false;
            }
            /// <summary>
            /// 根据关键字获取一个匹配节点位置
            /// </summary>
            /// <param name="key">关键字</param>
            /// <returns>一个匹配节点位置,失败返回-1</returns>
            public int IndexOf(keyType key)
            {
                node node = tree.boot;
                int index = 0;
                do
                {
                    int cmp = node.Key.CompareTo(key);
                    if (cmp == 0) return node.Left == null ? index : (index + node.Left.Count);
                    if (cmp > 0) node = node.Left;
                    else
                    {
                        if (node.Left != null) index += node.Left.Count;
                        node = node.Right;
                        ++index;
                    }
                }
                while (node != null);
                return -1;
            }
            /// <summary>
            /// 根据关键字比它小的节点数量
            /// </summary>
            /// <param name="key">关键字</param>
            /// <returns>节点数量</returns>
            public int CountLess(keyType key)
            {
                node node = tree.boot;
                int count = 0;
                do
                {
                    int cmp = node.Key.CompareTo(key);
                    if (cmp >= 0) node = node.Left;
                    else
                    {
                        if (node.Left != null) count += node.Left.Count;
                        node = node.Right;
                        ++count;
                    }
                }
                while (node != null);
                return count;
            }
            /// <summary>
            /// 根据关键字比它大的节点数量
            /// </summary>
            /// <param name="key">关键字</param>
            /// <returns>节点数量</returns>
            public int CountThan(keyType key)
            {
                node node = tree.boot;
                int count = 0;
                do
                {
                    int cmp = node.Key.CompareTo(key);
                    if (cmp <= 0) node = node.Right;
                    else
                    {
                        if (node.Right != null) count += node.Right.Count;
                        node = node.Left;
                        ++count;
                    }
                }
                while (node != null);
                return count;
            }
            /// <summary>
            /// 删除找到的节点
            /// </summary>
            public void Remove()
            {
                this.node.Remove();
                int depth = this.depth;
                ulong isLastLeft = pathMap & (1UL << (depth - 1));
                node father = null, node = tree.boot;
                while (depth != 0)
                {
                    father = node;
                    node = (pathMap & 1) == 0 ? node.Right : node.Left;
                    --father.Count;
                    pathMap >>= 1;
                    --depth;
                }
                if (this.node.Count == 0)
                {
                    if (father == null) tree.boot = null;
                    else if (isLastLeft == 0) father.Right = null;
                    else father.Left = null;
                }
            }
            /// <summary>
            /// 添加关键字
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="value">数据</param>
            public void Add(keyType key, valueType value)
            {
                if ((pathMap & (1UL << (depth - 1))) == 0) this.father.Right = new node(key, value);
                else this.father.Left = new node(key, value);

                node father = null, changeNode = null;
                int brotherCount = int.MaxValue;
                node = tree.boot;
                while (depth != 0)
                {
                    if (changeNode == null)
                    {
                        if ((pathMap & 1) == 0)
                        {
                            int leftCount = 0;
                            if (node.Right.Count > brotherCount
                                || (node.Left != null && (leftCount = node.Left.Count) > brotherCount)) changeNode = father;
                            father = node;
                            brotherCount = leftCount;
                            node = node.Right;
                        }
                        else
                        {
                            int rightCount = 0;
                            if (node.Left.Count > brotherCount
                                || (node.Right != null && (rightCount = node.Right.Count) > brotherCount)) changeNode = father;
                            father = node;
                            brotherCount = rightCount;
                            node = node.Left;
                        }
                    }
                    else
                    {
                        father = node;
                        node = (pathMap & 1) == 0 ? node.Right : node.Left;
                    }
                    ++father.Count;
                    pathMap >>= 1;
                    --depth;
                }
                if (changeNode != null) changeNode.CheckChange();
            }
        }
        /// <summary>
        /// 索引查找器
        /// </summary>
        private struct indexFinder
        {
            /// <summary>
            /// 跳过记录数
            /// </summary>
            private int skipCount;
            /// <summary>
            /// 查找数据
            /// </summary>
            public valueType Value;
            /// <summary>
            /// 数据复制器
            /// </summary>
            /// <param name="skipCount">跳过记录数</param>
            public indexFinder(int skipCount)
            {
                Value = default(valueType);
                this.skipCount = skipCount;
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            public void Find(node node)
            {
                node left = node.Left;
                while (left != null && left.Count > skipCount)
                {
                    if (left.Count > skipCount)
                    {
                        node = left;
                        left = node.Left;
                    }
                    else
                    {
                        skipCount -= left.Count;
                        break;
                    }
                }
                if (skipCount == 0)
                {
                    Value = node.Value;
                    return;
                }
                --skipCount;
                if (node.Right != null) Find(node.Right);
            }
        }
        /// <summary>
        /// 查找二叉树数据加载器
        /// </summary>
        private struct loader
        {
            /// <summary>
            /// 有序数据集合
            /// </summary>
            private keyValue<keyType, valueType>[] values;
            /// <summary>
            /// 当前读取位置
            /// </summary>
            private int index;
            /// <summary>
            /// 查找二叉树数据加载器
            /// </summary>
            /// <param name="values">有序数据集合</param>
            public loader(keyValue<keyType, valueType>[] values)
            {
                this.values = values;
                index = 0;
            }
            /// <summary>
            /// 二叉树节点
            /// </summary>
            /// <param name="count">节点数量</param>
            internal node Load(int count)
            {
                node node = new node(count);
                int leftCount = count >> 1;
                if (leftCount != 0) node.Left = Load(leftCount);
                keyValue<keyType, valueType> value = values[index++];
                count -= leftCount;
                node.Key = value.Key;
                node.Value = value.Value;
                if (--count != 0) node.Right = Load(count);
                return node;
            }
        }
        /// <summary>
        /// 数据复制器
        /// </summary>
        private struct copyer
        {
            /// <summary>
            /// 目标数组
            /// </summary>
            internal valueType[] Array;
            /// <summary>
            /// 当前写入位置
            /// </summary>
            private int index;
            /// <summary>
            /// 跳过记录数
            /// </summary>
            private int skipCount;
            /// <summary>
            /// 获取记录数
            /// </summary>
            public int count;
            /// <summary>
            /// 数据复制器
            /// </summary>
            /// <param name="array">目标数组</param>
            /// <param name="skipCount">跳过记录数</param>
            /// <param name="count">获取记录数</param>
            public copyer(valueType[] array, int skipCount, int count)
            {
                Array = array;
                this.skipCount = skipCount;
                this.count = count;
                index = 0;
            }
            /// <summary>
            /// 复制数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            public void Copy(node node)
            {
                node left = node.Left;
                for (int count = skipCount + this.count; left != null && left.Count >= count; left = node.Left) node = left;
                if (left != null)
                {
                    if (skipCount == 0) copy(left);
                    else if (skipCount >= left.Count) skipCount -= left.Count;
                    else skip(left);
                }
                if (skipCount == 0) Array[index++] = node.Value;
                else --skipCount;
                if (node.Right != null)
                {
                    int count = this.count - index;
                    if (count != 0)
                    {
                        if (skipCount == 0)
                        {
                            if (count < node.Right.Count) take(node.Right);
                            else copy(node.Right);
                        }
                        else Copy(node.Right);
                    }
                }
            }
            /// <summary>
            /// 复制节点数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            private void copy(node node)
            {
                if (node.Left != null) copy(node.Left);
                Array[index++] = node.Value;
                if (node.Right != null) copy(node.Right);
            }
            /// <summary>
            /// 跳过记录复制数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            private void skip(node node)
            {
                if (node.Left != null)
                {
                    if (skipCount >= node.Left.Count) skipCount -= node.Left.Count;
                    else skip(node.Left);
                }
                if (skipCount == 0) Array[index++] = node.Value;
                else --skipCount;
                if (node.Right != null)
                {
                    if (skipCount == 0) copy(node.Right);
                    else skip(node.Right);
                }
            }
            /// <summary>
            /// 复制节点数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            private void take(node node)
            {
                node left = node.Left;
                for (int count = this.count - index; left != null && left.Count >= count; left = node.Left) node = left;
                if (left != null) copy(left);
                Array[index++] = node.Value;
                if (node.Right != null)
                {
                    int count = this.count - index;
                    if (count != 0)
                    {
                        if (count < node.Right.Count) take(node.Right);
                        else copy(node.Right);
                    }
                }
            }
        }
        /// <summary>
        /// 数据复制器
        /// </summary>
        /// <typeparam name="arrayType">目标数据类型</typeparam>
        private struct copyer<arrayType>
        {
            /// <summary>
            /// 目标数组
            /// </summary>
            internal arrayType[] Array;
            /// <summary>
            /// 数据转换委托
            /// </summary>
            private Func<valueType, arrayType> getValue;
            /// <summary>
            /// 当前写入位置
            /// </summary>
            private int index;
            /// <summary>
            /// 跳过记录数
            /// </summary>
            private int skipCount;
            /// <summary>
            /// 获取记录数
            /// </summary>
            public int count;
            /// <summary>
            /// 数据复制器
            /// </summary>
            /// <param name="array">目标数组</param>
            /// <param name="getValue">数据转换委托</param>
            /// <param name="skipCount">跳过记录数</param>
            /// <param name="count">获取记录数</param>
            public copyer(arrayType[] array, Func<valueType, arrayType> getValue, int skipCount, int count)
            {
                Array = array;
                this.getValue = getValue;
                this.skipCount = skipCount;
                this.count = count;
                index = 0;
            }
            /// <summary>
            /// 复制数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            public void Copy(node node)
            {
                node left = node.Left;
                for (int count = skipCount + this.count; left != null && left.Count >= count; left = node.Left) node = left;
                if (left != null)
                {
                    if (skipCount == 0) copy(left);
                    else if (skipCount >= left.Count) skipCount -= left.Count;
                    else skip(left);
                }
                if (skipCount == 0) Array[index++] = getValue(node.Value);
                else --skipCount;
                if (node.Right != null)
                {
                    int count = this.count - index;
                    if (count != 0)
                    {
                        if (skipCount == 0)
                        {
                            if (count < node.Right.Count) take(node.Right);
                            else copy(node.Right);
                        }
                        else Copy(node.Right);
                    }
                }
            }
            /// <summary>
            /// 复制节点数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            private void copy(node node)
            {
                if (node.Left != null) copy(node.Left);
                Array[index++] = getValue(node.Value);
                if (node.Right != null) copy(node.Right);
            }
            /// <summary>
            /// 跳过记录复制数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            private void skip(node node)
            {
                if (node.Left != null)
                {
                    if (skipCount >= node.Left.Count) skipCount -= node.Left.Count;
                    else skip(node.Left);
                }
                if (skipCount == 0) Array[index++] = getValue(node.Value);
                else --skipCount;
                if (node.Right != null)
                {
                    if (skipCount == 0) copy(node.Right);
                    else skip(node.Right);
                }
            }
            /// <summary>
            /// 复制节点数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            private void take(node node)
            {
                node left = node.Left;
                for (int count = this.count - index; left != null && left.Count >= count; left = node.Left) node = left;
                if (left != null) copy(left);
                Array[index++] = getValue(node.Value);
                if (node.Right != null)
                {
                    int count = this.count - index;
                    if (count != 0)
                    {
                        if (count < node.Right.Count) take(node.Right);
                        else copy(node.Right);
                    }
                }
            }
        }
        /// <summary>
        /// 数据复制器
        /// </summary>
        private struct finder
        {
            /// <summary>
            /// 目标数组
            /// </summary>
            private valueType[] array;
            /// <summary>
            /// 数据匹配委托
            /// </summary>
            private Func<valueType, bool> isValue;
            /// <summary>
            /// 当前写入位置
            /// </summary>
            private int index;
            /// <summary>
            /// 查找目标结果
            /// </summary>
            internal subArray<valueType> Array
            {
                get { return subArray<valueType>.Unsafe(array, 0, index); }
            }
            /// <summary>
            /// 复制数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            /// <param name="isValue">数据匹配委托</param>
            public void Find(node node, Func<valueType, bool> isValue)
            {
                if (node.Count != 0)
                {
                    array = new valueType[node.Count];
                    this.isValue = isValue;
                    index = 0;
                    find(node);
                }
            }
            /// <summary>
            /// 复制数据
            /// </summary>
            /// <param name="node">二叉树节点</param>
            private void find(node node)
            {
                if (node.Left != null) find(node.Left);
                if (isValue(node.Value)) array[index++] = node.Value;
                if (node.Right != null) find(node.Right);
            }
        }
        /// <summary>
        /// 根节点
        /// </summary>
        private node boot;
        /// <summary>
        /// 节点数据
        /// </summary>
        public int Count
        {
            get { return boot != null ? boot.Count : 0; }
        }
        /// <summary>
        /// 根据关键字获取或者设置数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据,获取失败KeyNotFoundException</returns>
        public valueType this[keyType key]
        {
            get
            {
                node node = get(key);
                if (node != null) return node.Value;
                throw new KeyNotFoundException(key.ToString());
            }
            set { set(key, value, false); }
        }
        /// <summary>
        /// 基于叔侄节点数量比较与旋转概率选择的非严格平衡查找二叉树
        /// </summary>
        public searchTree() { }
        /// <summary>
        /// 创建查找二叉树
        /// </summary>
        /// <param name="values">已排序无重复的节点集合,不能为空</param>
        /// <returns>查找二叉树</returns>
        internal static searchTree<keyType, valueType> Unsafe(keyValue<keyType, valueType>[] values)
        {
            searchTree<keyType, valueType> tree = new searchTree<keyType, valueType>();
            tree.boot = new loader(values).Load(values.Length);
            return tree;
        }
        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            boot = null;
        }
        /// <summary>
        /// 根据关键字获取二叉树节点
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>二叉树节点,失败返回null</returns>
        private node get(keyType key)
        {
            node node = boot;
            while (node != null)
            {
                int cmp = node.Key.CompareTo(key);
                if (cmp == 0) return node;
                node = cmp > 0 ? node.Left : node.Right;
            }
            return null;
        }
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据</param>
        /// <param name="isCheck">是否检测重复</param>
        private void set(keyType key, valueType value, bool isCheck)
        {
            if (boot != null)
            {
                keyFinder finder = new keyFinder(this);
                if (finder.Find(key))
                {
                    if (isCheck) throw new ArgumentException("关键字 " + key.ToString() + " 已存在");
                    finder.Node.Value = value;
                }
                else finder.Add(key, value);
            }
            else boot = new node(key, value);
        }
        /// <summary>
        /// 添加关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(keyType key, valueType value)
        {
            set(key, value, true);
        }
        /// <summary>
        /// 根据关键字删除节点
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否存在关键字</returns>
        public bool Remove(keyType key)
        {
            if (boot != null)
            {
                keyFinder finder = new keyFinder(this);
                if (finder.Find(key))
                {
                    finder.Remove();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 根据关键字删除节点
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">被删除数据</param>
        /// <returns>是否存在关键字</returns>
        public bool Remove(keyType key, out valueType value)
        {
            if (boot != null)
            {
                keyFinder finder = new keyFinder(this);
                if (finder.Find(key))
                {
                    value = finder.Node.Value;
                    finder.Remove();
                    return true;
                }
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 判断是否包含关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否包含关键字</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool ContainsKey(keyType key)
        {
            return get(key) != null;
        }
        /// <summary>
        /// 根据关键字获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">目标数据</param>
        /// <returns>是否成功</returns>
        public bool TryGetValue(keyType key, out valueType value)
        {
            node node = get(key);
            if (node != null)
            {
                value = node.Value;
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取数据数组
        /// </summary>
        /// <returns>数据数组</returns>
        public valueType[] GetArray()
        {
            if (boot != null)
            {
                copyer copyer = new copyer(new valueType[boot.Count], 0, boot.Count);
                copyer.Copy(boot);
                return copyer.Array;
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取数据数组
        /// </summary>
        /// <typeparam name="arrayType">数据类型</typeparam>
        /// <param name="getValue">获取数据值的委托</param>
        /// <returns>数据数组</returns>
        public arrayType[] GetArray<arrayType>(Func<valueType, arrayType> getValue)
        {
            if (boot != null)
            {
                if (getValue == null) throw new NullReferenceException();
                copyer<arrayType> copyer = new copyer<arrayType>(new arrayType[boot.Count], getValue, 0, boot.Count);
                copyer.Copy(boot);
                return copyer.Array;
            }
            return nullValue<arrayType>.Array;
        }
        /// <summary>
        /// 获取数据数组
        /// </summary>
        /// <param name="isValue">数据匹配委托</param>
        /// <returns>数据数组</returns>
        public subArray<valueType> GetFind(Func<valueType, bool> isValue)
        {
            if (boot != null)
            {
                if (isValue == null) throw new NullReferenceException();
                finder finder = new finder();
                finder.Find(boot, isValue);
                return finder.Array;
            }
            return default(subArray<valueType>);
        }
        /// <summary>
        /// 获取排序范围数据集合
        /// </summary>
        /// <param name="values"></param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数</param>
        /// <returns>实际获取数据数量</returns>
        public int GetRange(valueType[] values, int skipCount, int getCount)
        {
            if (values.length() < getCount) throw new IndexOutOfRangeException();
            if (boot != null)
            {
                array.range range = new array.range(boot.Count, skipCount, getCount);
                if (range.GetCount != 0)
                {
                    copyer copyer = new copyer(values, range.SkipCount, range.GetCount);
                    copyer.Copy(boot);
                    return range.GetCount;
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取排序范围数据集合,用于分页
        /// </summary>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数</param>
        /// <returns>范围数据集合</returns>
        public valueType[] GetRange(int skipCount, int getCount)
        {
            if (boot != null)
            {
                array.range range = new array.range(boot.Count, skipCount, getCount);
                if (range.GetCount != 0)
                {
                    copyer copyer = new copyer(new valueType[range.GetCount], range.SkipCount, range.GetCount);
                    copyer.Copy(boot);
                    return copyer.Array;
                }
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取排序范围数据集合,用于分页
        /// </summary>
        /// <typeparam name="arrayType">数据类型</typeparam>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数</param>
        /// <param name="getValue">获取数据值的委托</param>
        /// <returns>范围数据集合</returns>
        public arrayType[] GetRange<arrayType>(int skipCount, int getCount, Func<valueType, arrayType> getValue)
        {
            if (boot != null)
            {
                array.range range = new array.range(boot.Count, skipCount, getCount);
                if (range.GetCount != 0)
                {
                    if (getValue == null) throw new NullReferenceException();
                    copyer<arrayType> copyer = new copyer<arrayType>(new arrayType[range.GetCount], getValue, range.SkipCount, range.GetCount);
                    copyer.Copy(boot);
                    return copyer.Array;
                }
            }
            return nullValue<arrayType>.Array;
        }
        /// <summary>
        /// 根据关键字获取一个匹配节点位置
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>一个匹配节点位置,失败返回-1</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int IndexOf(keyType key)
        {
            if (boot != null) return new keyFinder(this).IndexOf(key);
            return -1;
        }
        /// <summary>
        /// 根据关键字比它小的节点数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>节点数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int CountLess(keyType key)
        {
            if (boot != null) return new keyFinder(this).CountLess(key);
            return 0;
        }
        /// <summary>
        /// 根据关键字比它大的节点数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>节点数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int CountThan(keyType key)
        {
            if (boot != null) return new keyFinder(this).CountThan(key);
            return 0;
        }
        /// <summary>
        /// 根据节点位置获取数据
        /// </summary>
        /// <param name="index">节点位置</param>
        /// <returns>数据</returns>
        public valueType GetIndex(int index)
        {
            if (boot != null)
            {
                indexFinder indexer = new indexFinder(index);
                indexer.Find(boot);
                return indexer.Value;
            }
            return default(valueType);
        }
    }
}
