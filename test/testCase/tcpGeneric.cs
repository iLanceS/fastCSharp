using System;
using System.Collections.Generic;

namespace fastCSharp.testCase
{
    /// <summary>
    /// TCP服务泛型支持测试[由于旧版本反射模式的效率问题，暂时放弃泛型支持，等待重新设计]
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(IsIdentityCommand = true, Host = "127.0.0.1", Port = 12345, IsRememberIdentityCommand = false)]
    internal partial class tcpGeneric
    {
        /// <summary>
        /// 泛型支持测试
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private valueType get<valueType>()
        {
            object value;
            return values.TryGetValue(typeof(valueType), out value) ? (valueType)value : default(valueType);
        }
        /// <summary>
        /// 测试数据
        /// </summary>
        private static readonly Dictionary<Type, object> values = dictionary.CreateOnly<Type, object>();
#if NotFastCSharpCode
#else
        /// <summary>
        /// TCP服务泛型支持测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            int count = 0;
            using (tcpGeneric<int>.tcpServer server = new tcpGeneric<int>.tcpServer())
            {
                if (server.Start())
                {
                    using (tcpGeneric<int>.tcpClient client = new tcpGeneric<int>.tcpClient())
                    {
                        client.set(1);
                        if (tcpGeneric<int>.Value != 1) return false;

                        tcpGeneric<int>.Value = 2;
                        if (client.get() != 2) return false;

                        ++count;
                    }
                }
            }
            using (tcpGeneric<string>.tcpServer server = new tcpGeneric<string>.tcpServer())
            {
                if (server.Start())
                {
                    using (tcpGeneric<string>.tcpClient client = new tcpGeneric<string>.tcpClient())
                    {
                        client.set("a");
                        if (tcpGeneric<string>.Value != "a") return false;

                        tcpGeneric<string>.Value = "b";
                        if (client.get() != "b") return false;

                        ++count;
                    }
                }
            }
            if (count == 2)
            {
                return true;
                //由于旧版本反射模式的效率问题，暂时放弃泛型支持，等待重新设计
                //using (tcpGeneric.tcpServer server = new tcpGeneric.tcpServer())
                //{
                //    if (server.Start())
                //    {
                //        using (tcpGeneric.tcpClient client = new tcpGeneric.tcpClient())
                //        {
                //            values[typeof(int)] = 1;
                //            if (client.get<int>().Value != 1) return false;

                //            values[typeof(string)] = "a";
                //            if (client.get<string>().Value != "a") return false;

                //            return true;
                //        }
                //    }
                //}
            }
            return false;
        }
#endif
    }
    /// <summary>
    /// TCP服务泛型支持测试
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    [fastCSharp.code.cSharp.tcpServer(IsIdentityCommand = true, Host = "127.0.0.1", Port = 12345, IsRememberIdentityCommand = false)]
    internal partial class tcpGeneric<valueType>
    {
        /// <summary>
        /// 泛型支持测试
        /// </summary>
        /// <param name="value"></param>
        [fastCSharp.code.cSharp.tcpMethod]
        private void set(valueType value)
        {
            Value = value;
        }
        /// <summary>
        /// 泛型支持测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private valueType get()
        {
            return Value;
        }
        /// <summary>
        /// 测试数据
        /// </summary>
        internal static valueType Value;
    }
}
