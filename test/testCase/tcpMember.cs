using System;

namespace fastCSharp.testCase
{
    /// <summary>
    /// TCP服务字段与属性支持测试
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(IsIdentityCommand = true, Host = "127.0.0.1", Port = 12345, IsRememberIdentityCommand = false)]
    public partial class tcpMember
    {
        /// <summary>
        /// 测试字段
        /// </summary>
        [fastCSharp.code.cSharp.tcpMethod(IsOnlyGetMember = false)]
        private int field;
        /// <summary>
        /// 测试属性
        /// </summary>
        [fastCSharp.code.cSharp.tcpMethod(IsOnlyGetMember = false)]
        private int property
        {
            get { return field; }
            set { field = value; }
        }
        /// <summary>
        /// 只读属性[不支持不可读属性]
        /// </summary>
        [fastCSharp.code.cSharp.tcpMethod]
        private int getProperty
        {
            get { return field; }
        }
        /// <summary>
        /// 单索引测试
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private int this[int index]
        {
            get { return field + index; }
        }
        /// <summary>
        /// 多索引测试
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsOnlyGetMember = false)]
        private int this[int left, int right]
        {
            get { return field - left * right; }
            set { field = value + left * right; }
        }
#if NotFastCSharpCode
#else
        /// <summary>
        /// TCP服务字段与属性支持测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            tcpMember serverTarget = new tcpMember();
            using (tcpMember.tcpServer server = new tcpMember.tcpServer(null, null, serverTarget))
            {
                if (server.Start())
                {
                    using (tcpMember.tcpClient client = new tcpMember.tcpClient())
                    {
                        client.field = 1;
                        if (serverTarget.field != 1) return false;

                        serverTarget.field = 2;
                        if (client.field != 2) return false;

                        serverTarget.field = 3;
                        if (client.getProperty != 3) return false;

                        if (client[1] != 4) return false;

                        client.property = 5;
                        if (serverTarget.property != 5) return false;

                        serverTarget.property = 6;
                        if (client.property != 6) return false;

                        client[2, 3] = 7;
                        if (serverTarget[2, 3] != 7) return false;

                        serverTarget[3, 5] = 8;
                        if (client[3, 5] != 8) return false;

                        return true;
                    }
                }
            }
            return false;
        }
#endif
    }
}
