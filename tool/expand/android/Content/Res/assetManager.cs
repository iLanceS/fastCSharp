using System;
using Android.Content.Res;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Content.Res.AssetManager 扩展
    /// </summary>
    public static class assetManager
    {
        /// <summary>
        /// 打开资源文件
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IO.Stream open(this AssetManager manager, string fileName)
        {
            return manager.Open(fileName);
        }
    }
}