using System;

namespace fastCSharp.test.uniqueHashCode
{
    /// <summary>
    /// 位置信息栈
    /// </summary>
    struct indexStack
    {
        /// <summary>
        /// 位置信息数量
        /// </summary>
        private int count;
        /// <summary>
        /// 位置信息集合
        /// </summary>
        private int[] indexs;
        /// <summary>
        /// 位置信息栈
        /// </summary>
        /// <param name="count"></param>
        public indexStack(int count)
        {
            indexs = new int[count];
            this.count = 0;
        }
        /// <summary>
        /// 清空位置信息
        /// </summary>
        public void Reset()
        {
            count = 0;
        }
        /// <summary>
        /// 检测位置信息，不存在则入栈
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Check(int index)
        {
            for (int count = this.count; count != 0;)
            {
                if (indexs[--count] == index) return true;
            }
            indexs[this.count++] = index;
            return false;
        }
    }
}
