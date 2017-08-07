using System;
using Java.Util.Concurrent;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.Concurrent À©Õ¹
    /// </summary>
    public static class iExecutor
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="runnable"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void execute(this IExecutor executor, Java.Lang.IRunnable runnable)
        {
            executor.Execute(runnable);
        }
    }
}