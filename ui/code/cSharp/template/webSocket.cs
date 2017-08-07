using System;
#pragma warning disable 649

namespace fastCSharp.code.cSharp.template
{
    class webSocket : pub
    {
        #region PART CLASS
        #region NOT IsServer
        #region IF LoadMethod
        /*NOTE*/
        public abstract partial class /*NOTE*/@TypeNameDefinition : /*NOTE*/fastCSharp.code.cSharp.webSocket.socket<TypeNameDefinition>, /*NOTE*/fastCSharp.code.cSharp.webSocket.IWebSocket
        {
            private struct webSocketQuery
            {
                #region IF LoadMethod.Parameters.Length
                [fastCSharp.emit.jsonParse.member(IsDefault = true)]
                #endregion IF LoadMethod.Parameters.Length
                #region LOOP LoadMethod.Parameters
                #region IF XmlDocument
                /// <summary>
                /// @XmlDocument
                /// </summary>
                #endregion IF XmlDocument
                public @ParameterType.FullName @ParameterName;
                #endregion LOOP LoadMethod.Parameters
                #region NOTE
                public object ParameterJoinName;
                #endregion NOTE
            }
            /// <summary>
            /// WebSocket调用
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool loadSocket()
            {
                if (base.loadSocket())
                {
                    webSocketQuery query = new webSocketQuery();
                    if (parseParameterQuery(ref query))
                    {
                        return loadSocket(/*LOOP:LoadMethod.Parameters*/query.@ParameterJoinName/*LOOP:LoadMethod.Parameters*/);
                    }
                }
                return false;
            }
            #region NOTE
            bool loadSocket(object _) { return false; }
            #endregion NOTE
        }
        #endregion IF LoadMethod
        #endregion NOT IsServer

        #region IF IsServer
        #region IF Sockets.Length
        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<@SessionType.FullName>
        {
            #region NOTE
            static readonly FullName[] Sockets;
            const int Index = 0;
            #endregion NOTE
            /// <summary>
            /// webSocket处理索引集合
            /// </summary>
            protected override string[] webSockets
            {
                get
                {
                    string[] names = new string[@Sockets.Length];
                    #region LOOP Sockets
                    names[@Index] = "@CallName";
                    #endregion LOOP Sockets
                    return names;
                }
            }
            /// <summary>
            /// webSocket调用处理
            /// </summary>
            /// <param name="callIndex"></param>
            /// <param name="socket"></param>
            /// <param name="socketIdentity"></param>
            protected override void callWebSocket(int callIndex, fastCSharp.net.tcp.http.socketBase socket, long socketIdentity)
            {
                switch(callIndex)
                {
                    #region LOOP Sockets
                    case @Index: loadWebSocket(new @WebSocketMethodType.FullName(), socket, socketIdentity); return;
                    #endregion LOOP Sockets
                }
            }
        }
        #endregion IF Sockets.Length
        #endregion IF IsServer
        #endregion PART CLASS

    }
    /// <summary>
    /// CSharp模板公用模糊类型
    /// </summary>
    internal partial class pub
    {
        /// <summary>
        /// 获取该函数的类型
        /// </summary>
        public class WebSocketMethodType
        {
            /// <summary>
            /// 类型全名
            /// </summary>
            public partial class FullName : fastCSharp.code.cSharp.webSocket.socket<FullName>, fastCSharp.code.cSharp.webSocket.IWebSocket
            {
            }
        }
    }
}
