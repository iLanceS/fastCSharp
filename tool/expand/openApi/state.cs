using System;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Threading;

namespace fastCSharp.openApi
{
    /// <summary>
    /// 状态验证
    /// </summary>
    public static class state
    {
        /// <summary>
        /// 第三方登录URL重定向Cooike名称
        /// </summary>
        public static readonly byte[] OpenLoginUrlCookieName = ("OpenLoginUrl").getBytes();
        /// <summary>
        /// 状态验证
        /// </summary>
        private static readonly Dictionary<ulong, DateTime> states = dictionary.CreateULong<DateTime>();
        /// <summary>
        /// 状态验证访问锁
        /// </summary>
        private static readonly object stateLock = new object();
        /// <summary>
        /// 验证状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool Verify(string state)
        {
            return Verify(state.parseHex16NoCheck());
        }
        /// <summary>
        /// 验证状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool Verify(ulong state)
        {
            if (state != 0)
            {
                DateTime timeout;
                Monitor.Enter(stateLock);
                if (states.TryGetValue(state, out timeout)) states.Remove(state);
                else timeout = DateTime.MinValue;
                Monitor.Exit(stateLock); 
                return timeout >= date.NowSecond;
            }
            return false;
        }
        /// <summary>
        /// 获取状态验证
        /// </summary>
        /// <returns></returns>
        public static ulong Get()
        {
            do
            {
                DateTime timeout = date.NowSecond.AddMinutes(10);
                ulong value = fastCSharp.random.Default.SecureNextULongNotZero();
                Monitor.Enter(stateLock);
                try
                {
                    if (states.ContainsKey(value)) value = 0;
                    else states.Add(value, timeout);
                }
                finally { Monitor.Exit(stateLock); }
                if (value != 0)
                {
                    if (Interlocked.CompareExchange(ref isRefresh, 1, 0) == 0) fastCSharp.threading.timerTask.Default.Add(refreshHandle, timeout);
                    return value;
                }
            }
            while (true);
        }
        /// <summary>
        /// 获取状态验证字符串
        /// </summary>
        /// <returns></returns>
        public static string GetString()
        {
            return Get().toHex16();
        }
        /// <summary>
        /// 刷新状态验证集合
        /// </summary>
        private static subArray<ulong> refreshStates;
        /// <summary>
        /// 是否正在刷新状态验证集合
        /// </summary>
        private static int isRefresh;
        /// <summary>
        /// 刷新状态验证集合
        /// </summary>
        private static readonly Action refreshHandle = refresh;
        /// <summary>
        /// 刷新状态验证集合
        /// </summary>
        private static void refresh()
        {
            DateTime timeout = date.Now;
            Monitor.Enter(stateLock);
            try
            {
                foreach (KeyValuePair<ulong, DateTime> state in states)
                {
                    if (state.Value <= timeout) refreshStates.Add(state.Key);
                }
                int count = refreshStates.Count;
                if (count != 0)
                {
                    if (count == states.Count) states.Clear();
                    else
                    {
                        foreach (ulong state in refreshStates.UnsafeArray) states.Remove(state);
                    }
                }
            }
            finally
            {
                Monitor.Exit(stateLock); 
                refreshStates.Empty();
                if (states.Count == 0) isRefresh = 0;
                else fastCSharp.threading.timerTask.Default.Add(refreshHandle, date.NowSecond.AddMinutes(10));
            }
        }
        static state()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
