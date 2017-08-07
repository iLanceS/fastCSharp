using System;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// TCP调用函数
    /// </summary>
    public class tcpMethod : ignoreMember
    {
        /// <summary>
        /// tcpCall 服务名称。如果不指定 Service，则默认绑定到该 class 申明配置的 Service；一个 class 中的不同函数可以绑定到不同服务名称。
        /// </summary>
        public string Service;
        /// <summary>
        /// 服务名称
        /// </summary>
        public virtual string ServiceName
        {
            get { return Service; }
        }
        /// <summary>
        /// HTTP 调用的唯一名称，默认为函数名称，同一个服务不能存在两个相同的调用名称。
        /// </summary>
        public string HttpName;
        /// <summary>
        /// 分组标识用于选择性关闭服务处理，默认为 0 表示无分组。
        /// </summary>
        public int GroupId;
        /// <summary>
        /// 输入参数序列化后的数据最大字节数，0 表示不限。
        /// </summary>
        public int InputParameterMaxLength;
        /// <summary>
        /// 自定义命令序号，不能重复，服务申明中 IsIdentityCommand = true 时有效，默认为 int.MaxValue 表示不指定。存在自定义需求时不要使用巨大的数据，建议从 0 开始，因为它会是某个数组的大小。
        /// </summary>
        public int CommandIentity = int.MaxValue;
        /// <summary>
        /// 申明验证方法，客户端只有通过了验证才能调用其它函数。一个 TCP 服务只能指定一个验证方法（对于跨类型单例服务只能定义在 fastCSharp.code.cSharp.tcpCall.IsServer = true 的 class 中），且返回值类型必须为 bool。从安全的角度考虑，实际项目中的服务都应该定义验证方法，除非你能保证该服务绝对不会被其它人建立非法连接。比如参考 fastCSharp.net.tcp.timeVerifyServer。
        /// </summary>
        public bool IsVerifyMethod;
        /// <summary>
        /// 为了安全，默认为 true 表示服务端同步调用丢到任务线程池中处理（异步化），需要注意的是线程调度的开销可能会比较大。一般只有可以快速结束的非阻塞同步函数才考虑设置为 false，直接在 Socket 接收数据的 IO 线程中处理以避免线程调度；需要知道的是这种模式下如果产生阻塞会造成 Socket 停止接收数据。
        /// </summary>
        public bool IsServerSynchronousTask = true;
        /// <summary>
        /// 默认为 true 表示生成同步调用代理函数，同步模式使用的是 Monitor.Wait，会占用一个工作线程，并存在线程调度开销，优点是使用方便、安全。
        /// </summary>
        public bool IsClientSynchronous = true;
        /// <summary>
        /// 默认为 false 表示不生成异步调用代理函数。
        /// </summary>
        public bool IsClientAsynchronous;
        /// <summary>
        /// 为了安全，默认为 true 表示客户端异步回调丢到任务线程池中处理，需要注意的是线程调度的开销可能会比较大。一般只有可以快速结束的非阻塞回调处理才考虑设置为 false，直接在 Socket 接收数据的 IO 线程中处理以避免线程调度，需要知道的是这种模式下如果产生阻塞会造成 Socket 停止接收数据；更严重的是，如果你不小心在回调处理事件中又使用同步的方式调用了服务端，那么客户端死锁是必然的。
        /// </summary>
        public bool IsClientCallbackTask = true;
        /// <summary>
        /// 客户端异步回调的返回值是否和第一个相同类型的输入参数公用同一个对象，类似于 ref 的作用。
        /// </summary>
        public bool IsClientAsynchronousReturnInputParameter;
        /// <summary>
        /// 默认为 false 表示服务端需要应答客户端请求，否则仅仅是客户端发送数据到服务端（服务端不应答）。
        /// </summary>
        public bool IsClientSendOnly;
        /// <summary>
        /// 保持异步回调，1 问多答的交互模式（客户端一个请求，服务器端可以任意多次回调回应）。
        /// </summary>
        public bool IsKeepCallback;
        /// <summary>
        /// HTTP 调用是否仅支持 POST，否则同时支持 GET。
        /// </summary>
        public bool IsHttpPostOnly = true;
        /// <summary>
        /// 如果服务申明中 IsJsonSerialize = true 则默认为与服务保持一致，否则默认使用二进制序列化。
        /// </summary>
        public bool IsJsonSerialize;
        /// <summary>
        /// 默认为 true 表示输入参数二进制序列化需要检测循环引用，如果可以保证参数没有循环引用而且对象无需重用则应该设置为 false 减少 CPU 开销。
        /// </summary>
        public bool IsInputSerializeReferenceMember = true;
        /// <summary>
        /// 默认为 true 表示输出参数（包括 ref / out）二进制序列化需要检测循环引用，如果可以保证参数没有循环引用而且对象无需重用则应该设置为 false 减少 CPU 开销。
        /// </summary>
        public bool IsOutputSerializeReferenceMember = true;
        /// <summary>
        /// 输入参数是否添加包装处理申明 fastCSharp.emit.boxSerialize，用于只有一个输入参数的类型忽略外壳类型的处理以减少序列化开销。
        /// </summary>
        public bool IsInputSerializeBox;
        /// <summary>
        /// 输出参数是否添加包装处理申明 fastCSharp.emit.boxSerialize，用于只有一个输出参数的类型忽略外壳类型的处理以减少序列化开销。
        /// </summary>
        public bool IsOutputSerializeBox = true;
        /// <summary>
        /// 默认为 false 表示对输入参数生成 struct 以减少 new 开销，但是会增加参数赋值的开销，否则使用 class 包装输入参数。
        /// </summary>
        public bool IsInputParameterClass;
        /// <summary>
        /// 默认为 false 表示对输出参数生成 struct 以减少 new 开销，但是会增加参数赋值的开销，否则使用 class 包装输出参数。
        /// </summary>
        public bool IsOutputParameterClass;
        /// <summary>
        /// 默认为 true 表示对于 属性 / 字段 仅仅生成获取数据的代理，否则生成设置数据的代理（如果属性可写）。
        /// </summary>
        public bool IsOnlyGetMember = true;
        /// <summary>
        /// 是否过期
        /// </summary>
        public bool IsExpired;
    }
}
