using System;

namespace fastCSharp.testCase
{
    /// <summary>
    /// TCP服务JSON序列化支持测试，必须指定[IsJsonSerialize = true]，否则默认为二进制序列化
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(IsJsonSerialize = true, IsIdentityCommand = true, Host = "127.0.0.1", Port = 12345, IsRememberIdentityCommand = false)]
    internal partial class tcpJson
    {
        /// <summary>
        /// 测试数据
        /// </summary>
        private static int incValue;
        /// <summary>
        /// 无参数无返回值调用测试
        /// </summary>
        [fastCSharp.code.cSharp.tcpMethod]
        private void Inc()
        {
            ++incValue;
        }
        /// <summary>
        /// 单参数无返回值调用测试
        /// </summary>
        /// <param name="a"></param>
        [fastCSharp.code.cSharp.tcpMethod]
        private void Set(int a)
        {
            incValue = a;
        }
        /// <summary>
        /// 多参数无返回值调用测试
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [fastCSharp.code.cSharp.tcpMethod]
        private void Add(int a, int b)
        {
            incValue = a + b;
        }

        /// <summary>
        /// 无参数有返回值调用测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private int inc()
        {
            return ++incValue;
        }
        /// <summary>
        /// 单参数有返回值调用测试
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private int inc(int a)
        {
            return a + 1;
        }
        /// <summary>
        /// 多参数有返回值调用测试
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private int add(int a, int b)
        {
            return a + b;
        }

        /// <summary>
        /// 输出参数测试
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private int inc(out int a)
        {
            a = incValue;
            return a + 1;
        }
        /// <summary>
        /// 混合输出参数测试
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private int inc(int a, out int b)
        {
            b = a;
            return a + 1;
        }
        /// <summary>
        /// 混合输出参数测试
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private int add(int a, int b, out int c)
        {
            c = b;
            return a + b;
        }
#if NotFastCSharpCode
#else
        /// <summary>
        /// TCP服务JSON序列化测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            using (tcpJson.tcpServer server = new tcpJson.tcpServer())
            {
                if (server.Start())
                {
                    using (tcpJson.tcpClient client = new tcpJson.tcpClient())
                    {
                        incValue = 0;
                        client.Inc();
                        if (incValue != 1) return false;

                        client.Set(3);
                        if (incValue != 3) return false;

                        client.Add(2, 3);
                        if (incValue != 5) return false;

                        if (client.inc().Value != 6) return false;
                        if (client.inc(8).Value != 9) return false;
                        if (client.add(10, 13).Value != 23) return false;

                        incValue = 15;
                        int outValue;
                        if (client.inc(out outValue).Value != 16 && outValue != 15) return false;
                        if (client.inc(20, out outValue).Value != 21 && outValue != 20) return false;
                        if (client.add(30, 33, out outValue).Value != 63 && outValue != 33) return false;
                        return true;
                    }
                }
            }
            return false;
        }
#endif
    }
}
