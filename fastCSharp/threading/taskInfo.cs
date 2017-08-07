using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace fastCSharp.threading
{
    /// <summary>
    /// 任务信息
    /// </summary>
    internal struct taskInfo
    {
        /// <summary>
        /// 调用委托或对象
        /// </summary>
        public object Call;
        /// <summary>
        /// 调用类型
        /// </summary>
        public thread.callType Type;
        /// <summary>
        /// 任务执行出错委托,停止任务参数null
        /// </summary>
        public Action<Exception> OnError;
        /// <summary>
        /// 执行任务
        /// </summary>
        public void Run()
        {
            try
            {
                new thread.call { Value = Call, Type = Type }.Call();
            }
            catch (Exception error)
            {
                if (OnError == null) fastCSharp.log.Error.Add(error, null, false);
                else
                {
                    try
                    {
                        OnError(error);
                    }
                    catch (Exception exception)
                    {
                        fastCSharp.log.Error.Add(exception, null, false);
                    }
                }
            }
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void RunClear()
        {
            Run();
            Call = null;
            OnError = null;
        }
        /// <summary>
        /// 任务抛到线程池
        /// </summary>
        /// <param name="threadPool"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Start(threadPool threadPool)
        {
            if (OnError == null) threadPool.FastStart(Call, Type);
            else threadPool.FastStart(Call, OnError, Type, thread.errorType.Action);
        }
        /// <summary>
        /// 设置任务信息
        /// </summary>
        /// <param name="call">调用委托或对象</param>
        /// <param name="type">调用类型</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set(object call, thread.callType type)
        {
            Call = call;
            Type = type;
        }
        /// <summary>
        /// 设置任务信息
        /// </summary>
        /// <param name="call">调用委托或对象</param>
        /// <param name="type">调用类型</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set(object call, thread.callType type, Action<Exception> onError)
        {
            Call = call;
            Type = type;
            OnError = onError;
        }
        /// <summary>
        /// 取消任务
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Cancel()
        {
            Type = thread.callType.None;
            Call = null;
            OnError = null;
        }
        /// <summary>
        /// 任务抛到线程池
        /// </summary>
        /// <param name="threadPool"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void CheckStart(threadPool threadPool)
        {
            if (Type != thread.callType.None)
            {
                if (OnError == null) threadPool.FastStart(Call, Type);
                else threadPool.FastStart(Call, OnError, Type, thread.errorType.Action);
            }
        }
    }
}
