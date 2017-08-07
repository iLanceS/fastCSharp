using System;

namespace fastCSharp.threading
{
    /// <summary>
    /// 回调池
    /// </summary>
    /// <typeparam name="callbackType">回调对象类型</typeparam>
    /// <typeparam name="valueType">回调值类型</typeparam>
    public abstract class callbackPool<callbackType, valueType>
        where callbackType : class
    {
        /// <summary>
        /// 回调委托
        /// </summary>
        public Action<valueType> Callback;
        /// <summary>
        /// 添加回调对象
        /// </summary>
        /// <param name="poolCallback">回调对象</param>
        /// <param name="value">回调值</param>
        protected void push(callbackType poolCallback, valueType value)
        {
            Action<valueType> callback = Callback;
            Callback = null;
            try
            {
                typePool<callbackType>.PushNotNull(poolCallback);
            }
            finally
            {
                if (callback != null)
                {
                    try
                    {
                        callback(value);
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                    }
                }
            }
        }
        /// <summary>
        /// 回调处理
        /// </summary>
        /// <param name="value">回调值</param>
        protected void onlyCallback(valueType value)
        {
            Action<valueType> callback = Callback;
            if (callback != null)
            {
                try
                {
                    callback(value);
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
            }
        }
    }
}
