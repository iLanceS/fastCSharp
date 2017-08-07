using System;

namespace fastCSharp.test.radixSort
{
    /// <summary>
    /// 关键字定义
    /// </summary>
    static class key
    {
        public struct int32 : IComparable<int32>
        {
            /// <summary>
            /// 关键字
            /// </summary>
            public int Key;
            /// <summary>
            /// 关键字比较
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public int CompareTo(int32 other)
            {
                if (Key > other.Key) return 1;
                return Key < other.Key ? -1 : 0;
            }
        }
        public struct uint32 : IComparable<uint32>
        {
            /// <summary>
            /// 关键字
            /// </summary>
            public uint Key;
            /// <summary>
            /// 关键字比较
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public int CompareTo(uint32 other)
            {
                if (Key > other.Key) return 1;
                return Key < other.Key ? -1 : 0;
            }
        }
        public struct int64 : IComparable<int64>
        {
            /// <summary>
            /// 关键字
            /// </summary>
            public long Key;
            /// <summary>
            /// 关键字比较
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public int CompareTo(int64 other)
            {
                if (Key > other.Key) return 1;
                return Key < other.Key ? -1 : 0;
            }
        }
        public struct uint64 : IComparable<uint64>
        {
            /// <summary>
            /// 关键字
            /// </summary>
            public ulong Key;
            /// <summary>
            /// 关键字比较
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public int CompareTo(uint64 other)
            {
                if (Key > other.Key) return 1;
                return Key < other.Key ? -1 : 0;
            }
        }
    }
}
