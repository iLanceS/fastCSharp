using System;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Content.Context 扩展
    /// </summary>
    public static class context
    {
        /// <summary>
        /// 获取应用 APP 上下文 ApplicationContext
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Context getApplicationContext(this Context context)
        {
            return context.ApplicationContext;
        }
        /// <summary>
        /// 获取包名称 PackageName
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getPackageName(this Context context)
        {
            return context.PackageName;
        }
        /// <summary>
        /// 获取包管理器 PackageManager
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static PackageManager getPackageManager(this Context context)
        {
            return context.PackageManager;
        }
        /// <summary>
        /// OpenFileInput
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IO.Stream openFileInput(this Context context, string name)
        {
            return context.OpenFileInput(name);
        }
        /// <summary>
        /// Assets
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static AssetManager getAssets(this Context context)
        {
            return context.Assets;
        }
        /// <summary>
        /// GetSystemService
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Object getSystemService(this Context context, string name)
        {
            return context.GetSystemService(name);
        }
        /// <summary>
        /// OpenFileOutput
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IO.Stream openFileOutput(this Context context, string name, FileCreationMode mode)
        {
            return context.OpenFileOutput(name, mode);
        }
        /// <summary>
        /// OpenFileOutput
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IO.Stream openFileOutput(this Context context, string name, int mode)
        {
            return context.OpenFileOutput(name, (FileCreationMode)mode);
        }
        /// <summary>
        /// FilesDir
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.IO.File getFilesDir(this Context context)
        {
            return context.FilesDir;
        }
        /// <summary>
        /// ContentResolver
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ContentResolver getContentResolver(this Context context)
        {
            return context.ContentResolver;
        }
        /// <summary>
        /// Resources
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Resources getResources(this Context context)
        {
            return context.Resources;
        }
        /// <summary>
        /// GetSharedPreferences
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ISharedPreferences getSharedPreferences(this Context context, string name, FileCreationMode mode)
        {
            return context.GetSharedPreferences(name, mode);
        }
        /// <summary>
        /// GetDir
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.IO.File getDir(this Context context, string name, FileCreationMode mode)
        {
            return context.GetDir(name, mode);
        }
        /// <summary>
        /// StartActivity
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void startActivity(this Context context, Intent intent)
        {
            context.StartActivity(intent);
        }
        /// <summary>
        /// MainLooper
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Looper getMainLooper(this Context context)
        {
            return context.MainLooper;
        }
        /// <summary>
        /// ApplicationInfo
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ApplicationInfo getApplicationInfo(this Context context)
        {
            return context.ApplicationInfo;
        }
    }
}