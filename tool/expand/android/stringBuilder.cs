using System;
using System.Text;

namespace fastCSharp.android
{
    /// <summary>
    /// 字符串拼接器
    /// </summary>
    public static class stringBuilder
    {
        /// <summary>
        /// 添加字符串 Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, string value)
        {
            return builder.Append(value);
        }
        /// <summary>
        /// 添加字符 Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, char value)
        {
            return builder.Append(value);
        }
        /// <summary>
        /// 添加数字 Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, int value)
        {
            return builder.Append(value.toString());
        }
        /// <summary>
        /// 添加数字 Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, long value)
        {
            return builder.Append(value.toString());
        }
        /// <summary>
        /// 添加数字 Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, uint value)
        {
            return builder.Append(value.toString());
        }
        /// <summary>
        /// 添加数字 Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, ulong value)
        {
            return builder.Append(value.toString());
        }
        /// <summary>
        /// 添加数字 Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, short value)
        {
            return builder.Append(value.toString());
        }
        /// <summary>
        /// 添加数字 Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, ushort value)
        {
            return builder.Append(value.toString());
        }
        /// <summary>
        /// 添加数字 Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, byte value)
        {
            return builder.Append(value.toString());
        }
        /// <summary>
        /// 添加数字 Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, sbyte value)
        {
            return builder.Append(value.toString());
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, bool value)
        {
            return builder.Append(value);
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, double value)
        {
            return builder.Append(value);
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, float value)
        {
            return builder.Append(value);
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, decimal value)
        {
            return builder.Append(value);
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static StringBuilder append(this StringBuilder builder, char[] value)
        {
            return builder.Append(value);
        }
        /// <summary>
        /// Length
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int length(this StringBuilder builder)
        {
            return builder.Length;
        }
    }
}