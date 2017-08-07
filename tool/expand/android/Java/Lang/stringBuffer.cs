using System;

namespace fastCSharp.android
{
    /// <summary>
    /// ×Ö·û´®Æ´½Ó»º³åÇø
    /// </summary>
    public static class stringBuffer
    {
        /// <summary>
        /// Ìí¼Ó×Ö·û´® Append
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StringBuffer append(this Java.Lang.StringBuffer buffer, string str)
        {
            buffer.Append(str);
            return buffer;
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StringBuffer append(this Java.Lang.StringBuffer buffer, int value)
        {
            buffer.Append(value);
            return buffer;
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StringBuffer append(this Java.Lang.StringBuffer buffer, char[] value)
        {
            buffer.Append(value);
            return buffer;
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StringBuffer append(this Java.Lang.StringBuffer buffer, long value)
        {
            buffer.Append(value);
            return buffer;
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StringBuffer append(this Java.Lang.StringBuffer buffer, Java.Lang.StringBuffer value)
        {
            buffer.Append(value);
            return buffer;
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StringBuffer append(this Java.Lang.StringBuffer buffer, bool value)
        {
            buffer.Append(value);
            return buffer;
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StringBuffer append(this Java.Lang.StringBuffer buffer, char value)
        {
            buffer.Append(value);
            return buffer;
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StringBuffer append(this Java.Lang.StringBuffer buffer, Java.Lang.ICharSequence value)
        {
            buffer.Append(value);
            return buffer;
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StringBuffer append(this Java.Lang.StringBuffer buffer, float value)
        {
            buffer.Append(value);
            return buffer;
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StringBuffer append(this Java.Lang.StringBuffer buffer, double value)
        {
            buffer.Append(value);
            return buffer;
        }
    }
}