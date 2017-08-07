using System;
using System.Threading;
using fastCSharp.threading;

namespace fastCSharp.net
{
    /// <summary>
    /// 方法信息
    /// </summary>
    public abstract class asynchronousMethod
    {
#if NOJIT
        /// <summary>
        /// 返回参数
        /// </summary>
        /// <typeparam name="valueType">返回参数类型</typeparam>
        public interface IReturnParameter
        {
            /// <summary>
            /// 返回值
            /// </summary>
            object ReturnObject { get; set; }
        }
        /// <summary>
        /// 返回参数
        /// </summary>
        /// <typeparam name="valueType">返回参数类型</typeparam>
        public class returnParameter<valueType> : IReturnParameter
        {
            [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
            [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
            internal valueType Ret;
            /// <summary>
            /// 返回值
            /// </summary>
            public object ReturnObject
            {
                get { return Ret; }
                set { Ret = (valueType)value; }
            }
        }
#else
        /// <summary>
        /// 返回参数
        /// </summary>
        /// <typeparam name="valueType">返回参数类型</typeparam>
        public interface IReturnParameter<valueType>
        {
            /// <summary>
            /// 返回值
            /// </summary>
            valueType Return { get; set; }
        }
        /// <summary>
        /// 返回参数
        /// </summary>
        /// <typeparam name="valueType">返回参数类型</typeparam>
        public class returnParameter<valueType> : IReturnParameter<valueType>
        {
            [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
            [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
            internal valueType Ret;
            /// <summary>
            /// 返回值
            /// </summary>
            public valueType Return
            {
                get { return Ret; }
                set { Ret = value; }
            }
        }
#endif
        /// <summary>
        /// 异步回调
        /// </summary>
        /// <typeparam name="outputParameterType">输出参数类型</typeparam>
        public sealed class callReturn<outputParameterType> : callback<returnValue<outputParameterType>>
        {
            /// <summary>
            /// 回调委托
            /// </summary>
            private Action<returnValue> callback;
            /// <summary>
            /// 异步回调返回值
            /// </summary>
            /// <param name="outputParameter">输出参数</param>
            public override void Callback(ref returnValue<outputParameterType> outputParameter)
            {
                try
                {
                    callback(new returnValue { Type = outputParameter.Type });
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
            }
            /// <summary>
            /// 获取异步回调
            /// </summary>
            /// <param name="callback">回调委托</param>
            /// <returns>异步回调</returns>
            public static callReturn<outputParameterType> Get(Action<returnValue> callback)
            {
                if (callback == null) return null;
                try
                {
                    return new callReturn<outputParameterType> { callback = callback };
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                callback(new returnValue { Type = returnValue.type.ClientException });
                return null;
            }
        }
        /// <summary>
        /// 异步回调
        /// </summary>
        /// <typeparam name="returnType">返回值类型</typeparam>
        /// <typeparam name="outputParameterType">输出参数类型</typeparam>
        public sealed class callReturn<returnType, outputParameterType> : callback<returnValue<outputParameterType>>
#if NOJIT
            where outputParameterType : IReturnParameter
#else
            where outputParameterType : IReturnParameter<returnType>
#endif
        {
            /// <summary>
            /// 回调委托
            /// </summary>
            private Action<returnValue<returnType>> callback;
            /// <summary>
            /// 异步回调返回值
            /// </summary>
            /// <param name="outputParameter">输出参数</param>
            public override void Callback(ref returnValue<outputParameterType> outputParameter)
            {
                try
                {
#if NOJIT
                    callback(outputParameter.Type == returnValue.type.Success ? new returnValue<returnType> { Type = returnValue.type.Success, Value = (returnType)outputParameter.Value.ReturnObject } : new returnValue<returnType> { Type = outputParameter.Type });
#else
                    callback(outputParameter.Type == returnValue.type.Success ? new returnValue<returnType> { Type = returnValue.type.Success, Value = outputParameter.Value.Return } : new returnValue<returnType> { Type = outputParameter.Type });
#endif
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
            }
            /// <summary>
            /// 获取异步回调
            /// </summary>
            /// <param name="callback">回调委托</param>
            /// <returns>异步回调</returns>
            public static callReturn<returnType, outputParameterType> Get(Action<returnValue<returnType>> callback)
            {
                if (callback == null) return null;
                try
                {
                    return new callReturn<returnType, outputParameterType> { callback = callback };
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                callback(new returnValue<returnType> { Type = returnValue.type.ClientException });
                return null;
            }
        }
        /// <summary>
        /// 异步回调泛型返回值
        /// </summary>
        /// <typeparam name="returnType">返回值类型</typeparam>
        /// <typeparam name="outputParameterType">输出参数类型</typeparam>
        public sealed class callReturnGeneric<returnType, outputParameterType> : callback<returnValue<outputParameterType>>
#if NOJIT
            where outputParameterType : IReturnParameter
#else
            where outputParameterType : IReturnParameter<object>
#endif
        {
            /// <summary>
            /// 回调委托
            /// </summary>
            private Action<returnValue<returnType>> callback;
            /// <summary>
            /// 异步回调返回值
            /// </summary>
            /// <param name="outputParameter">输出参数</param>
            public override void Callback(ref returnValue<outputParameterType> outputParameter)
            {
                try
                {
#if NOJIT
                    callback(outputParameter.Type == returnValue.type.Success ? new returnValue<returnType> { Type = returnValue.type.Success, Value = (returnType)outputParameter.Value.ReturnObject } : new returnValue<returnType> { Type = outputParameter.Type });
#else
                    callback(outputParameter.Type == returnValue.type.Success ? new returnValue<returnType> { Type = returnValue.type.Success, Value = (returnType)outputParameter.Value.Return } : new returnValue<returnType> { Type = outputParameter.Type });
#endif
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
            }
            /// <summary>
            /// 异步回调泛型返回值
            /// </summary>
            /// <param name="callback">异步回调返回值</param>
            /// <returns>异步回调返回值</returns>
            public static callReturnGeneric<returnType, outputParameterType> Get(Action<returnValue<returnType>> callback)
            {
                if (callback == null) return null;
                try
                {
                    return new callReturnGeneric<returnType, outputParameterType> { callback = callback };
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                callback(new returnValue<returnType> { Type = returnValue.type.ClientException });
                return null;
            }
        }
    }
}
