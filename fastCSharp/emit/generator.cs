using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace fastCSharp.emit
{
    /// <summary>
    /// MSIL生成
    /// </summary>
    public static class generator
    {
        /// <summary>
        /// 加载Int32数据
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="value"></param>
        public static void int32(this ILGenerator generator, int value)
        {
            switch (value)
            {
                case 0:
                    generator.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    generator.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    generator.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    generator.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    generator.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    generator.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    generator.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    generator.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    generator.Emit(OpCodes.Ldc_I4_8);
                    return;
            }
            if (value == -1) generator.Emit(OpCodes.Ldc_I4_M1);
            else generator.Emit((uint)value <= sbyte.MaxValue ? OpCodes.Ldc_I4_S : OpCodes.Ldc_I4, value);
        }
        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="index"></param>
        public static void loadArgument(this ILGenerator generator, int index)
        {
            switch (index)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    generator.Emit(index <= sbyte.MaxValue ? OpCodes.Ldarg_S : OpCodes.Ldarg, index);
                    break;
            }
        }
        /// <summary>
        /// 判断成员位图是否匹配成员索引
        /// </summary>
        private static readonly MethodInfo memberMapIsMemberMethod = typeof(fastCSharp.code.memberMap).GetMethod("IsMember", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(int) }, null);
        /// <summary>
        /// 判断成员位图是否匹配成员索引
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public static void memberMapIsMember(this ILGenerator generator, OpCode target, int value)
        {
            generator.Emit(target);
            generator.int32(value);
            generator.Emit(OpCodes.Callvirt, memberMapIsMemberMethod);
        }
        /// <summary>
        /// 整数转换成指针
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static unsafe char* toPointer(long value)
        {
            return (char*)value;
        }
        /// <summary>
        /// 整数转换成指针
        /// </summary>
        private static readonly MethodInfo toPointerMethod = typeof(generator).GetMethod("toPointer", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(long) }, null);
        /// <summary>
        /// 内存字符流写入字符串方法信息
        /// </summary>
        private static readonly MethodInfo charStreamSimpleWriteNotNullCharsMethod = typeof(charStream).GetMethod("SimpleWriteNotNull", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(char*), typeof(int) }, null);
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="target"></param>
        /// <param name="value">字符串起始位置</param>
        /// <param name="count">写入字符数</param>
        public static unsafe void charStreamSimpleWriteNotNull(this ILGenerator generator, OpCode target, char* value, int count)
        {
            generator.Emit(target);
            generator.Emit(OpCodes.Ldc_I8, (long)value);
            generator.Emit(OpCodes.Call, toPointerMethod);
            //if ((ulong)value > uint.MaxValue) generator.Emit(OpCodes.Ldc_I8, (ulong)value);
            //else generator.Emit(OpCodes.Ldc_I4, (uint)value);
            //generator.Emit(OpCodes.Conv_U);
            generator.int32(count);
            generator.Emit(OpCodes.Call, charStreamSimpleWriteNotNullCharsMethod);
        }
        /// <summary>
        /// 内存字符流写入字符方法信息
        /// </summary>
        private static readonly MethodInfo charStreamWriteCharMethod = typeof(charStream).GetMethod("Write", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(char) }, null);
        /// <summary>
        /// 写入字符
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public static void charStreamWriteChar(this ILGenerator generator, OpCode target, char value)
        {
            generator.Emit(target);
            generator.int32(value);
            generator.Emit(OpCodes.Callvirt, charStreamWriteCharMethod);
        }
        /// <summary>
        /// 写入字符
        /// </summary>
        /// <param name="generator"></param>
        public static void charStreamWriteChar(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Callvirt, charStreamWriteCharMethod);
        }
        /// <summary>
        /// 内存字符流写入字符串方法信息
        /// </summary>
        private static readonly MethodInfo charStreamSimpleWriteNotNullMethod = typeof(charStream).GetMethod("SimpleWriteNotNull", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null);
        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="generator"></param>
        public static void charStreamSimpleWriteNotNull(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Call, charStreamSimpleWriteNotNullMethod);
        }
        /// <summary>
        /// 内存字符流写入字符串方法信息
        /// </summary>
        private static readonly MethodInfo charStreamWriteNotNullMethod = typeof(charStream).GetMethod("WriteNotNull", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
        /// <summary>
        /// 内存字符流写入字符串
        /// </summary>
        /// <param name="generator"></param>
        public static void charStreamWriteNotNull(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Call, charStreamWriteNotNullMethod);
        }
        /// <summary>
        /// 字符流写入null方法信息
        /// </summary>
        private static readonly MethodInfo charStreamWriteNullMethod = typeof(fastCSharp.web.ajax).GetMethod("WriteNull", BindingFlags.Public | BindingFlags.Static);
        /// <summary>
        /// 字符流写入null
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="target"></param>
        public static void charStreamWriteNull(this ILGenerator generator, OpCode target)
        {
            generator.Emit(target);
            generator.Emit(OpCodes.Call, charStreamWriteNullMethod);
        }
        /// <summary>
        /// 内存流安全写入Int32方法信息
        /// </summary>
        private static readonly MethodInfo unmanagedStreamUnsafeWriteIntMethod = typeof(unmanagedStream).GetMethod("UnsafeWrite", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(int) }, null);
        /// <summary>
        /// 内存流安全写入Int32
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="value"></param>
        public static void unmanagedStreamUnsafeWriteInt(this ILGenerator generator, int value)
        {
            generator.int32(value);
            generator.Emit(OpCodes.Callvirt, unmanagedStreamUnsafeWriteIntMethod);
        }
        /// <summary>
        /// 内存流写入Int32方法信息
        /// </summary>
        private static readonly MethodInfo unmanagedStreamWriteIntMethod = typeof(unmanagedStream).GetMethod("Write", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(int) }, null);
        /// <summary>
        /// 内存流写入Int32
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="value"></param>
        public static void unmanagedStreamWriteInt(this ILGenerator generator, int value)
        {
            generator.int32(value);
            generator.Emit(OpCodes.Callvirt, unmanagedStreamWriteIntMethod);
        }
    }
}
