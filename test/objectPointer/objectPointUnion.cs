using System;
using System.Runtime.InteropServices;
using fastCSharp.threading;

namespace fastCSharp.test.objectPointer
{
    /// <summary>
    /// 对象指针联合体，用于获取对象指针
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct objectPointUnion
    {
        /// <summary>
        /// 对象
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private sealed class objectValue
        {
            /// <summary>
            /// 对象
            /// </summary>
            [FieldOffset(0)]
            public object Value;
        }
        /// <summary>
        /// 指针
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private sealed unsafe class pointerValue
        {
            /// <summary>
            /// 指针
            /// </summary>
            [FieldOffset(0)]
            public void* Pointer;
        }
        /// <summary>
        /// 指针
        /// </summary>
        [FieldOffset(0)]
        private pointerValue Pointer;
        /// <summary>
        /// 对象
        /// </summary>
        [FieldOffset(0)]
        private objectValue Value;

        /// <summary>
        /// 对象
        /// </summary>
        private static readonly objectValue value = new objectValue();
        /// <summary>
        /// 对象访问锁
        /// </summary>
        private static int valueLock;
        /// <summary>
        /// 根据对象获取指针
        /// </summary>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        public static void* GetPoint(object objectValue)
        {
            objectPointUnion objectPoint = new objectPointUnion();
            interlocked.CompareSetYield(ref valueLock);
            value.Value = objectValue;
            objectPoint.Value = value;
            void* pointer = objectPoint.Pointer.Pointer;
            valueLock = 0;
            return pointer;
        }
    }
}
