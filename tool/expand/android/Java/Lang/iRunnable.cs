using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.IRunnable À©Õ¹
    /// </summary>
    public static class iRunnable
    {
        /// <summary>
        /// Run
        /// </summary>
        /// <param name="runnable"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void run(this Java.Lang.IRunnable runnable)
        {
            runnable.Run();
        }
    }
}