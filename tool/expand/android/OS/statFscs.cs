using System;
using Android.OS;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.OS.StatFs À©Õ¹
    /// </summary>
    public static class statFscs
    {
        /// <summary>
        /// BlockSizeLong
        /// </summary>
        /// <param name="statFs"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long getBlockSize(this StatFs statFs)
        {
            return statFs.BlockSizeLong;
        }
        /// <summary>
        /// BlockCountLong
        /// </summary>
        /// <param name="statFs"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long getBlockCount(this StatFs statFs)
        {
            return statFs.BlockCountLong;
        }
        /// <summary>
        /// AvailableBlocksLong
        /// </summary>
        /// <param name="statFs"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long getAvailableBlocks(this StatFs statFs)
        {
            return statFs.AvailableBlocksLong;
        }
    }
}