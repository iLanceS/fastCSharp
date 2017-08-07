using System;
using fastCSharp.code;
using System.Runtime.CompilerServices;

namespace fastCSharp.emit
{
    /// <summary>
    /// 二进制数据序列化类型配置
    /// </summary>
    public sealed class dataSerialize : binarySerialize
    {
        /// <summary>
        /// 默认二进制数据序列化类型配置
        /// </summary>
        internal static readonly dataSerialize Default = new dataSerialize { IsBaseType = false };
        /// <summary>
        /// 是否检测相同的引用成员(作为根节点时有效)
        /// </summary>
        public bool IsReferenceMember = true;
        /// <summary>
        /// 是否序列化成员位图
        /// </summary>
        public bool IsMemberMap = true;
        /// <summary>
        /// 自定义类型成员标识配置
        /// </summary>
        public sealed class custom : Attribute
        {
        }
        /// <summary>
        /// 二进制数据序列化缓存
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        public struct cache<valueType> where valueType : class
        {
            /// <summary>
            /// 缓存数据
            /// </summary>
            private byte[] data;
            /// <summary>
            /// 数据
            /// </summary>
            public valueType Value;
            /// <summary>
            /// 使用次数
            /// </summary>
            public int Count;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="value">二进制数据序列化缓存</param>
            /// <returns>数据</returns>
            public static implicit operator valueType(cache<valueType> value)
            {
                return value.Value;
            }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="deSerializer">序列化数据</param>
            /// <param name="value"></param>
            [fastCSharp.emit.dataSerialize.custom]
            private unsafe static void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref cache<valueType> value)
            {
                if (deSerializer.CheckNull() != 0)
                {
                    int length = deSerializer.ReadInt();
                    if (length == 1)
                    {
                        fastCSharp.emit.dataDeSerializer.typeDeSerializer<valueType>.DeSerialize(deSerializer, ref value.Value);
                    }
                    else
                    {
                        if (fastCSharp.emit.dataDeSerializer.DeSerialize(deSerializer.Read, length, ref value.Value)) deSerializer.Read += length;
                        else deSerializer.Error(binaryDeSerializer.deSerializeState.Custom);
                    }
                }
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void serialize(fastCSharp.emit.dataSerializer serializer)
            {
                if (Value == null) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                else if (Count > 1)
                {
                    if (data == null) data = fastCSharp.emit.dataSerializer.Serialize(Value);
                    fastCSharp.emit.binarySerializer.Serialize(serializer.Stream, data);
                }
                else
                {
                    serializer.Stream.Write(1);
                    fastCSharp.emit.dataSerializer.typeSerializer<valueType>.Serialize(serializer, Value);
                }
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="value"></param>
            [fastCSharp.emit.dataSerialize.custom]
            private static void serialize(fastCSharp.emit.dataSerializer serializer, cache<valueType> value)
            {
                value.serialize(serializer);
            }
        }
    }
}
