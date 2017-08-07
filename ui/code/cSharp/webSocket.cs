using System;
using fastCSharp.code;

namespace fastCSharp.code
{
    /// <summary>
    /// WebSocket调用配置
    /// </summary>
    internal sealed partial class webSocket
    {
        /// <summary>
        /// WebSocket调用代码生成
        /// </summary
        [auto(Name = "WebSocket", DependType = typeof(coder.cSharper), IsAuto = true)]
        internal sealed partial class cSharp : webView.cSharp<fastCSharp.code.cSharp.webSocket>, IAuto
        {
            /// <summary>
            /// WebSocket调用类型信息
            /// </summary>
            public class socketType
            {
                /// <summary>
                /// WebSocket调用类型
                /// </summary>
                public memberType type;
                /// <summary>
                /// WebSocket调用类型
                /// </summary>
                public memberType WebSocketMethodType
                {
                    get { return type; }
                }
                /// <summary>
                /// WebSocket调用配置
                /// </summary>
                public fastCSharp.code.cSharp.webSocket Attribute;
                /// <summary>
                /// 序号
                /// </summary>
                public int Index;
                /// <summary>
                /// 默认程序集名称
                /// </summary>
                public string DefaultNamespace;
                /// <summary>
                /// WebSocket调用类型名称
                /// </summary>
                private string callTypeName
                {
                    get
                    {
                        string callName = type.FullName;
                        if (callName.StartsWith(DefaultNamespace, StringComparison.Ordinal)) callName = callName.Substring(DefaultNamespace.Length - 1);
                        else callName = "/" + callName;
                        return callName.replace('.', '/');
                    }
                }
                /// <summary>
                /// 是否忽略大小写
                /// </summary>
                public bool IgnoreCase;
                /// <summary>
                /// WebSocket调用名称
                /// </summary>
                private string callName;
                /// <summary>
                /// WebSocket调用名称
                /// </summary>
                public string CallName
                {
                    get
                    {
                        if (callName == null)
                        {
                            if ((callName = Attribute.TypeCallName) == null) callName = callTypeName;
                            else if (callName.Length == 0 || callName[0] != '/') callName = "/" + callName;
                            if (Attribute.MethodName != null) callName += "/" + Attribute.MethodName.replace('.', '/');
                            if (IgnoreCase) callName = callName.toLower();
                        }
                        return callName;
                    }
                }
            }
            /// <summary>
            /// WebSocket调用类型集合
            /// </summary>
            private list<socketType> sockets = new list<socketType>();
            /// <summary>
            /// WebSocket调用类型集合
            /// </summary>
            public socketType[] Sockets;
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected override void nextCreate()
            {
                keyValue<methodInfo, fastCSharp.code.cSharp.webSocket> loadMethod = fastCSharp.code.cSharp.webSocket.GetLoadMethod(type.Type);
                LoadMethod = loadMethod.Key;
                LoadAttribute = loadMethod.Value;
                sockets.Add(new socketType { type = type, Attribute = Attribute, DefaultNamespace = AutoParameter.DefaultNamespace + ".", Index = sockets.Count, IgnoreCase = AutoParameter.WebConfig.IgnoreCase });
                IsServer = false;
                create(true);
            }
            /// <summary>
            /// 安装完成处理
            /// </summary>
            protected override void onCreated()
            {
                if (sockets.Count != 0)
                {
                    Sockets = sockets.ToArray();
                    IsServer = true;
                    _code_.Empty();
                    create(false);
                    fastCSharp.code.coder.Add(@"
namespace " + AutoParameter.DefaultNamespace + @"
{
" + _code_.ToString() + @"
}");
                }
            }
        }
    }
}
