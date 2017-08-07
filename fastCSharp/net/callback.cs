using System;

namespace fastCSharp.net
{
    ///// <summary>
    ///// 异步回调
    ///// </summary>
    //public abstract class callback
    //{
    //    /// <summary>
    //    /// 异步回调返回值
    //    /// </summary>
    //    /// <param name="outputParameter">输出参数</param>
    //    public abstract void OnReturn(returnValue outputParameter);
    //}
    /// <summary>
    /// 异步回调
    /// </summary>
    /// <typeparam name="outputParameterType"></typeparam>
    public abstract class callback<outputParameterType>
    {
        /// <summary>
        /// 异步回调返回值
        /// </summary>
        /// <param name="outputParameter">输出参数</param>
        public abstract void Callback(ref outputParameterType outputParameter);
    }
}
