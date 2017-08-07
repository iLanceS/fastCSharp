using System;
using fastCSharp.threading;

namespace fastCSharp.net
{
    /// <summary>
    /// 同步等待调用
    /// </summary>
    public sealed class waitCall : callback<fastCSharp.net.returnValue>
    {
        /// <summary>
        /// 同步等待
        /// </summary>
        private autoWaitHandle waitHandle;
        /// <summary>
        /// 输出参数
        /// </summary>
        private fastCSharp.net.returnValue.type outputParameter;
        /// <summary>
        /// 调用返回值（警告：每次调用只能使用一次）
        /// </summary>
        /// <param name="value"></param>
        public void Get(out fastCSharp.net.returnValue value)
        {
            waitHandle.Wait();
            value.Type = outputParameter;
            outputParameter = returnValue.type.Unknown;
            typePool<waitCall>.PushNotNull(this);
        }
        /// <summary>
        /// 等待返回
        /// </summary>
        /// <returns>是否存在返回值</returns>
        public fastCSharp.net.returnValue.type Wait()
        {
            waitHandle.Wait();
            returnValue.type outputParameter = this.outputParameter;
            this.outputParameter = returnValue.type.Unknown;
            typePool<waitCall>.PushNotNull(this);
            return outputParameter;
        }
        /// <summary>
        /// 同步等待调用
        /// </summary>
        private waitCall()
        {
            waitHandle = new autoWaitHandle(false);
        }
        /// <summary>
        /// 回调处理
        /// </summary>
        /// <param name="outputParameter">是否调用成功</param>
        public override void Callback(ref fastCSharp.net.returnValue outputParameter)
        {
            this.outputParameter = outputParameter.Type;
            //if (!outputParameter) log.Default.Add("异步调用失败(bool)", true, false);
            waitHandle.Set();
        }
        /// <summary>
        /// 获取同步等待调用
        /// </summary>
        /// <returns>同步等待调用</returns>
        public static waitCall Get()
        {
            waitCall value = typePool<waitCall>.Pop();
            if (value == null)
            {
                try
                {
                    value = new waitCall();
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                    return null;
                }
            }
            return value;
        }
    }
    /// <summary>
    /// 同步等待调用
    /// </summary>
    /// <typeparam name="outputParameterType">输出参数类型</typeparam>
    public sealed class waitCall<outputParameterType> : callback<fastCSharp.net.returnValue<outputParameterType>>
    {
        /// <summary>
        /// 同步等待
        /// </summary>
        private autoWaitHandle waitHandle;
        /// <summary>
        /// 输出参数
        /// </summary>
        private fastCSharp.net.returnValue<outputParameterType> outputParameter;
        /// <summary>
        /// 调用返回值（警告：每次调用只能使用一次）
        /// </summary>
        /// <param name="value"></param>
        public void Get(out fastCSharp.net.returnValue<outputParameterType> value)
        {
            waitHandle.Wait();
            value = outputParameter;
            outputParameter.Null();
            typePool<waitCall<outputParameterType>>.PushNotNull(this);
        }
        /// <summary>
        /// 等待返回(无意义)
        /// </summary>
        /// <returns>是否存在返回值</returns>
        public fastCSharp.net.returnValue.type Wait()
        {
            return fastCSharp.net.returnValue.type.Unknown;
        }
        /// <summary>
        /// 同步等待调用
        /// </summary>
        private waitCall()
        {
            waitHandle = new autoWaitHandle(false);
        }
        /// <summary>
        /// 回调处理
        /// </summary>
        /// <param name="outputParameter">输出参数</param>
        public override void Callback(ref fastCSharp.net.returnValue<outputParameterType> outputParameter)
        {
            this.outputParameter = outputParameter;
            //if (!outputParameter.IsReturn) log.Default.Add("异步调用失败()", true, false);
            waitHandle.Set();
        }
        /// <summary>
        /// 获取同步等待调用
        /// </summary>
        /// <returns>同步等待调用</returns>
        public static waitCall<outputParameterType> Get()
        {
            waitCall<outputParameterType> value = typePool<waitCall<outputParameterType>>.Pop();
            if (value == null)
            {
                try
                {
                    value = new waitCall<outputParameterType>();
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                    return null;
                }
            }
            return value;
        }
    }
}
