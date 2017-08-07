using System;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql
{
    /// <summary>
    /// 文件分块成员
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public struct fileBlockMember<valueType>
    {
        /// <summary>
        /// 文件索引自定义枚举
        /// </summary>
        [Flags]
        private enum custom
        {
            /// <summary>
            /// 是否已经加载数据
            /// </summary>
            HasValue = 1,
            /// <summary>
            /// 是否加载数据错误
            /// </summary>
            LoadError = 2,
            /// <summary>
            /// 序列化数据
            /// </summary>
            Serialize = 4
        }
        /// <summary>
        /// 数据获取
        /// </summary>
        private sealed class getter
        {
            /// <summary>
            /// 数据
            /// </summary>
            public valueType Value;
            /// <summary>
            /// 数据是否加载成功
            /// </summary>
            public bool IsValue;
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="buffer"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Get(subArray<byte> buffer)
            {
                if (buffer.length != 0 && fastCSharp.emit.dataDeSerializer.DeSerialize(ref buffer, ref Value)) IsValue = true;
            }
        }
        ///// <summary>
        ///// 是否值类型
        ///// </summary>
        //private static readonly bool isValueType = typeof(valueType).IsValueType;
#if NotFastCSharpCode
#else
        /// <summary>
        /// 文件分块服务 TCP客户端
        /// </summary>
        private fastCSharp.io.fileBlockServer.tcpClient client;
        /// <summary>
        /// 数据对象
        /// </summary>
        public unsafe valueType Value
        {
            get
            {
                if ((index.Custom & (int)custom.HasValue) == 0)
                {
                    if ((index.Custom & (int)custom.LoadError) == 0)
                    {
                        if (index.Size == 0)
                        {
                            value = default(valueType);
                            index.Custom |= (int)custom.HasValue;
                        }
                        else
                        {
                            byte[] buffer = fastCSharp.memoryPool.TinyBuffers.Get();
                            try
                            {
                                getter getter = new getter();
                                fastCSharp.code.cSharp.tcpBase.subByteArrayEvent bufferEvent = default(fastCSharp.code.cSharp.tcpBase.subByteArrayEvent);
                                fixed (byte* bufferFixed = buffer) *(fastCSharp.io.fileBlockStream.index*)bufferFixed = index;
                                bufferEvent.Buffer.UnsafeSet(buffer, 0, sizeof(fastCSharp.io.fileBlockStream.index));
                                bufferEvent.DeSerializeEvent = getter.Get;
                                if (client.read(bufferEvent).Type == fastCSharp.net.returnValue.type.Success && getter.IsValue)
                                {
                                    value = getter.Value;
                                    index.Custom |= (int)custom.HasValue;
                                    return value;
                                }
                                index.Custom |= (int)custom.LoadError;
                            }
                            catch (Exception error)
                            {
                                index.Custom |= (int)custom.LoadError;
                                fastCSharp.log.Default.Add(error, null, true);
                            }
                            finally { fastCSharp.memoryPool.TinyBuffers.PushNotNull(buffer); }
                            fastCSharp.log.Error.Throw(log.exceptionType.Unknown);
                        }
                    }
                    else fastCSharp.log.Error.Throw(log.exceptionType.Unknown);
                }
                return value;
            }
            set
            {
                this.value = value;
                index.Custom = (int)custom.HasValue;
            }
        }
#endif
        /// <summary>
        /// 文件索引
        /// </summary>
        private fastCSharp.io.fileBlockStream.index index;
        /// <summary>
        /// 数据对象
        /// </summary>
        private valueType value;
        /// <summary>
        /// 设置序列化数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetSerialize()
        {
            index.Custom |= (int)custom.Serialize;
        }

        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="serializer">对象序列化器</param>
        /// <param name="value"></param>
        [fastCSharp.emit.dataSerialize.custom]
        private unsafe static void serialize(fastCSharp.emit.dataSerializer serializer, fileBlockMember<valueType> value)
        {
            int indexCustom = value.index.Custom;
            if ((indexCustom & (int)custom.Serialize) == 0)
            {
                if (value.index.Size == 0) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                else
                {
                    unmanagedStream stream = serializer.Stream;
                    stream.PrepLength(sizeof(int) * 2 + sizeof(long));
                    byte* data = stream.CurrentData;
                    *(int*)data = (emit.pub.PuzzleValue & 0x7fffff00) + indexCustom;
                    *(int*)(data + sizeof(int)) = value.index.Size;
                    *(long*)(data + sizeof(int) * 2) = value.index.Index;
                    stream.UnsafeAddLength(sizeof(int) * 2 + sizeof(long));
                    stream.PrepLength();
                }
            }
            else if (value.value == null) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
            else
            {
                serializer.Stream.Write(indexCustom);
                fastCSharp.emit.dataSerializer.typeSerializer<valueType>.Serialize(serializer, value.value);
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="deSerializer">对象反序列化器</param>
        /// <param name="value"></param>
        /// <returns>是否成功</returns>
        [fastCSharp.emit.dataSerialize.custom]
        private unsafe static void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref fileBlockMember<valueType> value)
        {
            if (deSerializer.CheckNull() == 0)
            {
                value.index.Null();
                value.value = default(valueType);
            }
            else
            {
                byte* read = deSerializer.Read;
                int indexCustom = *(int*)read;
                if ((indexCustom & 0x7fffff00) == (emit.pub.PuzzleValue & 0x7fffff00))
                {
                    if ((indexCustom & (int)custom.Serialize) == 0)
                    {
                        if (value.index.ReSet(*(long*)(read + sizeof(int) * 2), *(int*)(read + sizeof(int))) == 0)
                        {
                            value.index.Custom = 0;
                            value.value = default(valueType);
                        }
                        deSerializer.Read += sizeof(int) * 2 + sizeof(long);
                    }
                    else
                    {
                        deSerializer.Read += sizeof(int);
                        fastCSharp.emit.dataDeSerializer.typeDeSerializer<valueType>.DeSerialize(deSerializer, ref value.value);
                    }
                }
                else deSerializer.Error(emit.binaryDeSerializer.deSerializeState.UnknownData);
            }
        }
    }
}
