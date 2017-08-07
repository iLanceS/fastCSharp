using System;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 任务信息
    /// </summary>
    public abstract class run
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        public abstract void Run();
    }
    /// <summary>
    /// 泛型任务信息
    /// </summary>
    /// <typeparam name="parameterType">任务执行参数类型</typeparam>
    public sealed class run<parameterType> : run
    {
        /// <summary>
        /// 任务执行委托
        /// </summary>
        private Action<parameterType> func;
        /// <summary>
        /// 任务执行参数
        /// </summary>
        private parameterType parameter;
        /// <summary>
        /// 执行任务
        /// </summary>
        public override void Run()
        {
            Action<parameterType> func = this.func;
            parameterType parameter = this.parameter;
            this.func = null;
            this.parameter = default(parameterType);
            try
            {
                typePool<run<parameterType>>.PushNotNull(this);
            }
            finally
            {
                func(parameter);
            }
        }
        /// <summary>
        /// 泛型任务信息
        /// </summary>
        /// <param name="action">任务执行委托</param>
        /// <param name="parameter">任务执行参数</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set(Action<parameterType> action, ref parameterType parameter)
        {
            this.func = action;
            this.parameter = parameter;
        }
        /// <summary>
        /// 泛型任务信息
        /// </summary>
        /// <param name="action">任务执行委托</param>
        /// <param name="parameter">任务执行参数</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set(Action<parameterType> action, parameterType parameter)
        {
            this.func = action;
            this.parameter = parameter;
        }
        /// <summary>
        /// 泛型任务信息
        /// </summary>
        /// <returns>泛型任务信息</returns>
        public static run<parameterType> Pop()
        {
            run<parameterType> run = typePool<run<parameterType>>.Pop();
            if (run == null)
            {
                try
                {
                    run = new run<parameterType>();
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
            return run;
        }
        ///// <summary>
        ///// 泛型任务信息
        ///// </summary>
        ///// <param name="action">任务执行委托</param>
        ///// <param name="parameter">任务执行参数</param>
        ///// <returns>泛型任务信息</returns>
        //public static Action Create(Action<parameterType> action, ref parameterType parameter)
        //{
        //    run<parameterType> run = Pop();
        //    return run == null ? null : run.Set(action, ref parameter);
        //}
    }
    ///// <summary>
    ///// 泛型任务信息
    ///// </summary>
    ///// <typeparam name="returnType">返回值参数类型</typeparam>
    //public sealed class runReturn<returnType>
    //{
    //    /// <summary>
    //    /// 任务执行委托
    //    /// </summary>
    //    private Func<returnType> func;
    //    /// <summary>
    //    /// 执行任务
    //    /// </summary>
    //    private Action action;
    //    /// <summary>
    //    /// 泛型任务信息
    //    /// </summary>
    //    private runReturn()
    //    {
    //        action = call;
    //    }
    //    /// <summary>
    //    /// 执行任务
    //    /// </summary>
    //    private void call()
    //    {
    //        Func<returnType> func = this.func;
    //        this.func = null;
    //        try
    //        {
    //            typePool<runReturn<returnType>>.PushNotNull(this);
    //        }
    //        finally
    //        {
    //            func();
    //        }
    //    }
    //    /// <summary>
    //    /// 泛型任务信息
    //    /// </summary>
    //    /// <returns>泛型任务信息</returns>
    //    public static runReturn<returnType> Pop()
    //    {
    //        runReturn<returnType> run = typePool<runReturn<returnType>>.Pop();
    //        if (run == null)
    //        {
    //            try
    //            {
    //                run = new runReturn<returnType>();
    //            }
    //            catch (Exception error)
    //            {
    //                log.Error.Add(error, null, false);
    //            }
    //        }
    //        return run;
    //    }
    //    /// <summary>
    //    /// 泛型任务信息
    //    /// </summary>
    //    /// <param name="func">任务执行委托</param>
    //    /// <returns>泛型任务信息</returns>
    //    public static Action Create(Func<returnType> func)
    //    {
    //        runReturn<returnType> run = Pop();
    //        if (run != null)
    //        {
    //            run.func = func;
    //            return run.action;
    //        }
    //        return null;
    //    }
    //}
    ///// <summary>
    ///// 泛型任务信息
    ///// </summary>
    ///// <typeparam name="parameterType">任务执行参数类型</typeparam>
    //public sealed class runPushPool<parameterType> : run
    //{
    //    /// <summary>
    //    /// 任务执行委托
    //    /// </summary>
    //    private pushPool<parameterType> func;
    //    /// <summary>
    //    /// 任务执行参数
    //    /// </summary>
    //    private parameterType parameter;
    //    /// <summary>
    //    /// 执行任务
    //    /// </summary>
    //    public override void Run()
    //    {
    //        pushPool<parameterType> func = this.func;
    //        parameterType parameter = this.parameter;
    //        this.func = null;
    //        this.parameter = default(parameterType);
    //        try
    //        {
    //            typePool<runPushPool<parameterType>>.PushNotNull(this);
    //        }
    //        finally
    //        {
    //            func(ref parameter);
    //        }
    //    }
    //    /// <summary>
    //    /// 泛型任务信息
    //    /// </summary>
    //    /// <param name="action">任务执行委托</param>
    //    /// <param name="parameter">任务执行参数</param>
    //    /// <returns>泛型任务信息</returns>
    //    [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
    //    public void Set(pushPool<parameterType> action, ref parameterType parameter)
    //    {
    //        this.func = action;
    //        this.parameter = parameter;
    //    }
    //    /// <summary>
    //    /// 泛型任务信息
    //    /// </summary>
    //    /// <param name="action">任务执行委托</param>
    //    /// <param name="parameter">任务执行参数</param>
    //    /// <returns>泛型任务信息</returns>
    //    [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
    //    public void Set(pushPool<parameterType> action, parameterType parameter)
    //    {
    //        this.func = action;
    //        this.parameter = parameter;
    //    }
    //    /// <summary>
    //    /// 泛型任务信息
    //    /// </summary>
    //    /// <returns>泛型任务信息</returns>
    //    public static runPushPool<parameterType> Pop()
    //    {
    //        runPushPool<parameterType> run = typePool<runPushPool<parameterType>>.Pop();
    //        if (run == null)
    //        {
    //            try
    //            {
    //                run = new runPushPool<parameterType>();
    //            }
    //            catch (Exception error)
    //            {
    //                log.Error.Add(error, null, false);
    //            }
    //        }
    //        return run;
    //    }
    //    ///// <summary>
    //    ///// 泛型任务信息
    //    ///// </summary>
    //    ///// <param name="action">任务执行委托</param>
    //    ///// <param name="parameter">任务执行参数</param>
    //    ///// <returns>泛型任务信息</returns>
    //    //public static Action Create(pushPool<parameterType> action, ref parameterType parameter)
    //    //{
    //    //    runPushPool<parameterType> run = Pop();
    //    //    return run == null ? null : run.Set(action, ref parameter);
    //    //}
    //}
    ///// <summary>
    ///// 泛型任务信息
    ///// </summary>
    ///// <typeparam name="parameterType">任务执行参数类型</typeparam>
    ///// <typeparam name="returnType">返回值类型</typeparam>
    //public sealed class run<parameterType, returnType>
    //{
    //    /// <summary>
    //    /// 任务执行委托
    //    /// </summary>
    //    private Func<parameterType, returnType> func;
    //    /// <summary>
    //    /// 返回值执行委托
    //    /// </summary>
    //    private Action<returnType> onReturn;
    //    /// <summary>
    //    /// 任务执行参数
    //    /// </summary>
    //    private parameterType parameter;
    //    /// <summary>
    //    /// 执行任务
    //    /// </summary>
    //    private Action action;
    //    /// <summary>
    //    /// 泛型任务信息
    //    /// </summary>
    //    private run()
    //    {
    //        action = call;
    //    }
    //    /// <summary>
    //    /// 执行任务
    //    /// </summary>
    //    private void call()
    //    {
    //        Func<parameterType, returnType> func = this.func;
    //        Action<returnType> onReturn = this.onReturn;
    //        parameterType parameter = this.parameter;
    //        this.func = null;
    //        this.onReturn = null;
    //        this.parameter = default(parameterType);
    //        try
    //        {
    //            typePool<run<parameterType, returnType>>.PushNotNull(this);
    //        }
    //        finally
    //        {
    //            onReturn(func(parameter));
    //        }
    //    }
    //    /// <summary>
    //    /// 泛型任务信息
    //    /// </summary>
    //    /// <param name="func">任务执行委托</param>
    //    /// <param name="onReturn">返回值执行委托</param>
    //    /// <param name="parameter">任务执行参数</param>
    //    /// <returns>泛型任务信息</returns>
    //    [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
    //    public Action Set(Func<parameterType, returnType> func, ref parameterType parameter, Action<returnType> onReturn)
    //    {
    //        this.func = func;
    //        this.parameter = parameter;
    //        this.onReturn = onReturn;
    //        return action;
    //    }
    //    /// <summary>
    //    /// 泛型任务信息
    //    /// </summary>
    //    /// <returns>泛型任务信息</returns>
    //    public static run<parameterType, returnType> Pop()
    //    {
    //        run<parameterType, returnType> run = typePool<run<parameterType, returnType>>.Pop();
    //        if (run == null)
    //        {
    //            try
    //            {
    //                run = new run<parameterType, returnType>();
    //            }
    //            catch (Exception error)
    //            {
    //                log.Error.Add(error, null, false);
    //            }
    //        }
    //        return run;
    //    }
    //    /// <summary>
    //    /// 泛型任务信息
    //    /// </summary>
    //    /// <param name="func">任务执行委托</param>
    //    /// <param name="parameter">任务执行参数</param>
    //    /// <param name="onReturn">返回值执行委托</param>
    //    /// <returns>泛型任务信息</returns>
    //    public static Action Create(Func<parameterType, returnType> func, ref parameterType parameter, Action<returnType> onReturn)
    //    {
    //        run<parameterType, returnType> run = Pop();
    //        return run == null ? null : run.Set(func, ref parameter, onReturn);
    //    }
    //}
}
