using System;
using System.Threading;
using fastCSharp;
using fastCSharp.reflection;
using fastCSharp.memoryDatabase;
using System.Collections.Generic;
using fastCSharp.code;
using System.Reflection;
using System.Runtime.CompilerServices;
using fastCSharp.threading;

namespace fastCSharp.emit
{
    /// <summary>
    /// 内存数据库表格配置
    /// </summary>
    public class memoryDatabaseTable
    {
        /// <summary>
        /// 内存数据库表格操作工具 字段名称
        /// </summary>
        public static readonly string MemoryDatabaseTableName = "MdbTable";
        /// <summary>
        /// 序列化类型
        /// </summary>
        public enum serializeType : byte
        {
            /// <summary>
            /// 二进制索引序列化
            /// </summary>
            Index,
            /// <summary>
            /// JSON序列化
            /// </summary>
            Json,
            /// <summary>
            /// 二进制数据序列化
            /// </summary>
            Data
        }
        /// <summary>
        /// 数据序列化
        /// </summary>
        internal unsafe abstract class serializer
        {
            /// <summary>
            /// 数据库成员信息
            /// </summary>
            protected struct dataMember
            {
                /// <summary>
                /// 类型序号集合
                /// </summary>
                private readonly static Dictionary<Type, int> typeIndexs;
                /// <summary>
                /// 成员名称
                /// </summary>
                public string Name;
                /// <summary>
                /// 类型名称
                /// </summary>
                public string TypeName;
                /// <summary>
                /// 类型序号
                /// </summary>
                public int TypeIndex;
                /// <summary>
                /// 成员序号
                /// </summary>
                public int MemberIndex;
                /// <summary>
                /// 成员集合
                /// </summary>
                public dataMember[] Members;
                /// <summary>
                /// 数据库成员信息
                /// </summary>
                /// <param name="member">成员信息</param>
                /// <param name="serializeType">序列化类型</param>
                public dataMember(memberIndex member, Type serializeType)
                {
                    Name = member.Member.Name;
                    MemberIndex = member.MemberIndex;
                    if (typeIndexs.TryGetValue(member.Type, out TypeIndex))
                    {
                        TypeName = null;
                        Members = nullValue<dataMember>.Array;
                    }
                    else
                    {
                        TypeName = member.Type.fullName();
                        Members = (dataMember[])serializeType.MakeGenericType(member.Type).GetField("dataMembers", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).GetValue(null);
                    }
                }
                /// <summary>
                /// 判断是否相等
                /// </summary>
                /// <param name="other"></param>
                /// <param name="history"></param>
                /// <returns></returns>
                private bool equals(dataMember other, list<dataMember[]> history)
                {
                    if (((TypeIndex ^ other.TypeIndex) | (MemberIndex ^ other.MemberIndex)) == 0 && Name == other.Name && TypeName == other.TypeName)
                    {
                        return Members == null ? other.Members == null : other.Members != null && Equals(Members, other.Members, history);
                    }
                    return false;
                }
                /// <summary>
                /// 判断是否相等
                /// </summary>
                /// <param name="left"></param>
                /// <param name="right"></param>
                /// <param name="history"></param>
                /// <returns></returns>
                public static bool Equals(dataMember[] left, dataMember[] right, list<dataMember[]> history)
                {
                    if (history.IndexOf(left) != -1) return true;
                    history.Add(left);
                    if (left.Length == right.Length)
                    {
                        int index = 0;
                        foreach (dataMember member in left)
                        {
                            if (!member.Equals(right[index++])) return false;
                        }
                        return true;
                    }
                    return false;
                }
                ///// <summary>
                ///// 获取类型序号
                ///// </summary>
                ///// <param name="type">类型</param>
                ///// <returns>类型序号</returns>
                //public static int GetMemberTypeIndex(Type type)
                //{
                //    int index;
                //    return typeIndexs.TryGetValue(type, out index) ? index : 0;
                //}
                static dataMember()
                {
                    typeIndexs = dictionary.CreateOnly<Type, int>();
                    int index = 0;
                    typeIndexs.Add(typeof(bool), ++index);
                    typeIndexs.Add(typeof(bool?), ++index);
                    typeIndexs.Add(typeof(byte), ++index);
                    typeIndexs.Add(typeof(byte?), ++index);
                    typeIndexs.Add(typeof(sbyte), ++index);
                    typeIndexs.Add(typeof(sbyte?), ++index);
                    typeIndexs.Add(typeof(short), ++index);
                    typeIndexs.Add(typeof(short?), ++index);
                    typeIndexs.Add(typeof(ushort), ++index);
                    typeIndexs.Add(typeof(ushort?), ++index);
                    typeIndexs.Add(typeof(int), ++index);
                    typeIndexs.Add(typeof(int?), ++index);
                    typeIndexs.Add(typeof(uint), ++index);
                    typeIndexs.Add(typeof(uint?), ++index);
                    typeIndexs.Add(typeof(long), ++index);
                    typeIndexs.Add(typeof(long?), ++index);
                    typeIndexs.Add(typeof(ulong), ++index);
                    typeIndexs.Add(typeof(ulong?), ++index);
                    typeIndexs.Add(typeof(DateTime), ++index);
                    typeIndexs.Add(typeof(DateTime?), ++index);
                    typeIndexs.Add(typeof(float), ++index);
                    typeIndexs.Add(typeof(float?), ++index);
                    typeIndexs.Add(typeof(double), ++index);
                    typeIndexs.Add(typeof(double?), ++index);
                    typeIndexs.Add(typeof(decimal), ++index);
                    typeIndexs.Add(typeof(decimal?), ++index);
                    typeIndexs.Add(typeof(Guid), ++index);
                    typeIndexs.Add(typeof(Guid?), ++index);
                    typeIndexs.Add(typeof(char), ++index);
                    typeIndexs.Add(typeof(char?), ++index);
                    typeIndexs.Add(typeof(string), ++index);
                    typeIndexs.Add(typeof(bool[]), ++index);
                    typeIndexs.Add(typeof(bool?[]), ++index);
                    typeIndexs.Add(typeof(byte[]), ++index);
                    typeIndexs.Add(typeof(byte?[]), ++index);
                    typeIndexs.Add(typeof(sbyte[]), ++index);
                    typeIndexs.Add(typeof(sbyte?[]), ++index);
                    typeIndexs.Add(typeof(short[]), ++index);
                    typeIndexs.Add(typeof(short?[]), ++index);
                    typeIndexs.Add(typeof(ushort[]), ++index);
                    typeIndexs.Add(typeof(ushort?[]), ++index);
                    typeIndexs.Add(typeof(int[]), ++index);
                    typeIndexs.Add(typeof(int?[]), ++index);
                    typeIndexs.Add(typeof(uint[]), ++index);
                    typeIndexs.Add(typeof(uint?[]), ++index);
                    typeIndexs.Add(typeof(long[]), ++index);
                    typeIndexs.Add(typeof(long?[]), ++index);
                    typeIndexs.Add(typeof(ulong[]), ++index);
                    typeIndexs.Add(typeof(ulong?[]), ++index);
                    typeIndexs.Add(typeof(DateTime[]), ++index);
                    typeIndexs.Add(typeof(DateTime?[]), ++index);
                    typeIndexs.Add(typeof(float[]), ++index);
                    typeIndexs.Add(typeof(float?[]), ++index);
                    typeIndexs.Add(typeof(double[]), ++index);
                    typeIndexs.Add(typeof(double?[]), ++index);
                    typeIndexs.Add(typeof(decimal[]), ++index);
                    typeIndexs.Add(typeof(decimal?[]), ++index);
                    typeIndexs.Add(typeof(Guid[]), ++index);
                    typeIndexs.Add(typeof(Guid?[]), ++index);
                    typeIndexs.Add(typeof(char[]), ++index);
                    typeIndexs.Add(typeof(char?[]), ++index);
                    typeIndexs.Add(typeof(string[]), ++index);
                }
            }
            /// <summary>
            /// 序列化类型
            /// </summary>
            internal abstract serializeType Type { get; }
            /// <summary>
            /// 加载数据成员位图
            /// </summary>
            internal abstract memberMap LoadMemberMap { get; }
            /// <summary>
            /// 是否存在成员集合
            /// </summary>
            internal abstract bool IsDataMember { get; }
            /// <summary>
            /// 判断当前成员集合是否匹配
            /// </summary>
            internal abstract bool IsCurrentDataMembers { get; }
            /// <summary>
            /// 序列化成员集合
            /// </summary>
            /// <param name="stream"></param>
            internal abstract void Members(unmanagedStream stream);
            /// <summary>
            /// 准备加载数据
            /// </summary>
            internal abstract void ReadyLoad();
            /// <summary>
            /// 加载数据结束
            /// </summary>
            internal abstract void Loaded();
            /// <summary>
            /// 反序列化成员集合
            /// </summary>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal abstract bool LoadMembers(byte* data, int size);
            /// <summary>
            /// 获取成员集合
            /// </summary>
            /// <param name="members">成员集合</param>
            /// <param name="serializeType">序列化类型</param>
            /// <returns></returns>
            protected static dataMember[] getMembers(subArray<memberIndex> members, Type serializeType)
            {
                dataMember[] dataMembers = new dataMember[members.length];
                int index = 0;
                foreach (memberIndex member in members.Sort((left, right) => left.Member.Name.CompareTo(right.Member.Name))) dataMembers[index++] = new dataMember(member, serializeType);
                return dataMembers;
            }
        }
        /// <summary>
        /// 数据序列化
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        internal unsafe abstract class serializer<valueType> : serializer, IDisposable
        {
            /// <summary>
            /// 反序列化添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal abstract bool LoadInsert(valueType value, byte* data, int size);
            /// <summary>
            /// 反序列化更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal abstract bool LoadUpdate(valueType value, byte* data, int size);
            /// <summary>
            /// 反序列化删除数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal abstract bool LoadDelete<keyType>(ref keyType value, byte* data, int size);
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            internal abstract void Insert(valueType value, unmanagedStream stream);
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            /// <param name="memberMap"></param>
            internal abstract void Update(valueType value, unmanagedStream stream, memberMap memberMap);
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            internal abstract void Delete<keyType>(keyType value, unmanagedStream stream);
            /// <summary>
            /// 释放资源
            /// </summary>
            public virtual void Dispose() { Loaded(); }
        }
        /// <summary>
        /// 二进制数据序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        private abstract class binarySerializer<valueType> : serializer<valueType>
        {
            /// <summary>
            /// 当前成员集合
            /// </summary>
            protected dataMember[] currentDataMembers;
            /// <summary>
            /// 是否存在成员集合
            /// </summary>
            internal override bool IsDataMember
            {
                get { return true; }
            }
            /// <summary>
            /// 反序列化成员集合
            /// </summary>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal unsafe override bool LoadMembers(byte* data, int size)
            {
                if (fastCSharp.emit.dataDeSerializer.DeSerialize(data, size, ref currentDataMembers))
                {
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Json解析配置参数
            /// </summary>
            protected fastCSharp.emit.binaryDeSerializer.config deSerializerConfig;
            /// <summary>
            /// 加载数据成员位图
            /// </summary>
            internal override memberMap LoadMemberMap { get { return deSerializerConfig.MemberMap; } }
            /// <summary>
            /// 准备加载数据
            /// </summary>
            internal override void ReadyLoad()
            {
                deSerializerConfig = new binaryDeSerializer.config { IsLogError = false, MemberMap = memberMap<valueType>.New() };
            }
            /// <summary>
            /// 加载数据结束
            /// </summary>
            internal override void Loaded()
            {
                if (deSerializerConfig != null)
                {
                    deSerializerConfig.MemberMap.Dispose();
                    deSerializerConfig = null;
                }
            }
            /// <summary>
            /// 获取成员集合
            /// </summary>
            /// <param name="members">成员集合</param>
            /// <param name="memberMap"></param>
            protected static subArray<memberIndex> getMembers(subArray<memberIndex> members, memberMap memberMap)
            {
                if (members.length != 0)
                {
                    subArray<memberIndex> dataMembers = subArray<memberIndex>.Unsafe(members.array, 0, 0);
                    memberMap modelMemberMap = memoryDatabaseModel<valueType>.MemberMap;
                    int isAllMember = memoryDatabaseModel<valueType>.IsAllMember;
                    foreach (memberIndex member in members)
                    {
                        if (modelMemberMap.IsMember(member.MemberIndex))
                        {
                            memberMap.SetMember(member.MemberIndex);
                            dataMembers.Add(member);
                        }
                        else isAllMember = 0;
                    }
                    if (isAllMember == 0) return dataMembers;
                    memberMap.Clear();
                }
                return members;
            }
        }
        /// <summary>
        /// 二进制索引序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        private unsafe sealed class indexSerializer<valueType> : binarySerializer<valueType>
        {
            /// <summary>
            /// 成员位图
            /// </summary>
            private static readonly memberMap memberMap = memberMap<valueType>.NewEmpty();
            /// <summary>
            /// 成员集合
            /// </summary>
            private static dataMember[] dataMembers;
            /// <summary>
            /// 序列化类型
            /// </summary>
            internal override serializeType Type
            {
                get { return serializeType.Index; }
            }
            /// <summary>
            /// 判断当前成员集合是否匹配
            /// </summary>
            internal override bool IsCurrentDataMembers
            {
                get
                {
                    return dataMember.Equals(dataMembers, currentDataMembers, new list<dataMember[]>());
                }
            }
            /// <summary>
            /// 反序列化添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal override bool LoadInsert(valueType value, byte* data, int size)
            {
                return fastCSharp.emit.indexDeSerializer.DeSerialize(data, size, ref value, deSerializerConfig);
            }
            /// <summary>
            /// 反序列化更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal override bool LoadUpdate(valueType value, byte* data, int size)
            {
                return fastCSharp.emit.indexDeSerializer.DeSerialize(data, size, ref value, deSerializerConfig);
            }
            /// <summary>
            /// 反序列化删除数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal override bool LoadDelete<keyType>(ref keyType value, byte* data, int size)
            {
                return fastCSharp.emit.indexDeSerializer.DeSerialize(data, size, ref value);
            }
            /// <summary>
            /// 序列化成员集合
            /// </summary>
            /// <param name="stream"></param>
            internal override void Members(unmanagedStream stream)
            {
                fastCSharp.emit.dataSerializer.Serialize(dataMembers, stream);
            }
            /// <summary>
            /// 序列化参数
            /// </summary>
            private readonly fastCSharp.emit.binarySerializer.config serializeConfig = new binarySerializer.config { IsMemberMapErrorLog = false };
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            internal override void Insert(valueType value, unmanagedStream stream)
            {
                serializeConfig.MemberMap = memberMap.IsDefault ? null : memberMap;
                fastCSharp.emit.indexSerializer.Serialize(value, stream, serializeConfig);
            }
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            /// <param name="memberMap"></param>
            internal override void Update(valueType value, unmanagedStream stream, memberMap memberMap)
            {
                serializeConfig.MemberMap = memberMap;
                fastCSharp.emit.indexSerializer.Serialize(value, stream, serializeConfig);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            internal override void Delete<keyType>(keyType value, unmanagedStream stream)
            {
                serializeConfig.MemberMap = null;
                fastCSharp.emit.indexSerializer.Serialize(value, stream, serializeConfig);
            }
            static indexSerializer()
            {
                dataMembers = getMembers(getMembers(emit.indexSerializer.typeSerializer<valueType>.GetMembers(), memberMap), typeof(indexSerializer<>));
            }
        }
        /// <summary>
        /// 二进制数据序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        private unsafe sealed class dataSerializer<valueType> : binarySerializer<valueType>
        {
            /// <summary>
            /// 成员位图
            /// </summary>
            private static readonly memberMap memberMap = memberMap<valueType>.NewEmpty();
            /// <summary>
            /// 成员集合
            /// </summary>
            private static dataMember[] dataMembers;
            /// <summary>
            /// 序列化类型
            /// </summary>
            internal override serializeType Type
            {
                get { return serializeType.Data; }
            }
            /// <summary>
            /// 判断当前成员集合是否匹配
            /// </summary>
            internal override bool IsCurrentDataMembers
            {
                get
                {
                    return dataMember.Equals(dataMembers, currentDataMembers, new list<dataMember[]>());
                }
            }
            /// <summary>
            /// 反序列化添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal override bool LoadInsert(valueType value, byte* data, int size)
            {
                return fastCSharp.emit.dataDeSerializer.DeSerialize(data, size, ref value, deSerializerConfig);
            }
            /// <summary>
            /// 反序列化更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal override bool LoadUpdate(valueType value, byte* data, int size)
            {
                return fastCSharp.emit.dataDeSerializer.DeSerialize(data, size, ref value, deSerializerConfig);
            }
            /// <summary>
            /// 反序列化删除数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal override bool LoadDelete<keyType>(ref keyType value, byte* data, int size)
            {
                return fastCSharp.emit.dataDeSerializer.DeSerialize(data, size, ref value);
            }
            /// <summary>
            /// 序列化成员集合
            /// </summary>
            /// <param name="stream"></param>
            internal override void Members(unmanagedStream stream)
            {
                fastCSharp.emit.dataSerializer.Serialize(dataMembers, stream);
            }
            /// <summary>
            /// 序列化参数
            /// </summary>
            private readonly fastCSharp.emit.dataSerializer.config serializeConfig = new dataSerializer.config { IsMemberMapErrorLog = false };
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            internal override void Insert(valueType value, unmanagedStream stream)
            {
                serializeConfig.MemberMap = memberMap.IsDefault ? null : memberMap;
                fastCSharp.emit.dataSerializer.Serialize(value, stream, serializeConfig);
            }
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            /// <param name="memberMap"></param>
            internal override void Update(valueType value, unmanagedStream stream, memberMap memberMap)
            {
                serializeConfig.MemberMap = memberMap;
                fastCSharp.emit.dataSerializer.Serialize(value, stream, serializeConfig);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            internal override void Delete<keyType>(keyType value, unmanagedStream stream)
            {
                serializeConfig.MemberMap = null;
                fastCSharp.emit.dataSerializer.Serialize(value, stream, serializeConfig);
            }
            static dataSerializer()
            {
                dataMembers = getMembers(getMembers(emit.dataSerializer.typeSerializer<valueType>.GetMembers(), memberMap), typeof(dataSerializer<>));
            }
        }
        /// <summary>
        /// JSON数据序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        private unsafe sealed class jsonSerializer<valueType> : serializer<valueType>
        {
            /// <summary>
            /// 成员位图
            /// </summary>
            private static readonly memberMap<valueType> memberMap = memberMap<valueType>.NewEmpty();
            /// <summary>
            /// 序列化类型
            /// </summary>
            internal override serializeType Type
            {
                get { return serializeType.Json; }
            }
            /// <summary>
            /// 是否存在成员集合
            /// </summary>
            internal override bool IsDataMember
            {
                get { return false; }
            }
            /// <summary>
            /// 反序列化成员集合
            /// </summary>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal override bool LoadMembers(byte* data, int size)
            {
                log.Error.Throw(log.exceptionType.ErrorOperation);
                return false;
            }
            /// <summary>
            /// 判断当前成员集合是否匹配
            /// </summary>
            internal override bool IsCurrentDataMembers { get { return true; } }
            /// <summary>
            /// Json解析配置参数
            /// </summary>
            private fastCSharp.emit.jsonParser.config parseConfig;
            /// <summary>
            /// 加载数据成员位图
            /// </summary>
            internal override memberMap LoadMemberMap { get { return parseConfig.MemberMap; } }
            /// <summary>
            /// 释放资源
            /// </summary>
            public override void Dispose()
            {
                base.Dispose();
                jsonStream.Dispose();
            }
            /// <summary>
            /// 准备加载数据
            /// </summary>
            internal override void ReadyLoad()
            {
                parseConfig = new jsonParser.config { IsOutputError = false, IsEndSpace = false, MemberMap = memberMap<valueType>.New() };
            }
            /// <summary>
            /// 加载数据结束
            /// </summary>
            internal override void Loaded()
            {
                if (parseConfig != null)
                {
                    parseConfig.MemberMap.Dispose();
                    parseConfig = null;
                }
            }
            /// <summary>
            /// 检测数据结束位置
            /// </summary>
            /// <param name="data"></param>
            /// <param name="size"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void checkSize(byte* data, ref int size)
            {
                if (*(ushort*)(data + size - sizeof(ushort)) == 32) size -= sizeof(ushort);
            }
            /// <summary>
            /// 反序列化添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal override bool LoadInsert(valueType value, byte* data, int size)
            {
                checkSize(data, ref size);
                return fastCSharp.emit.jsonParser.UnsafeParse((char*)data, size >> 1, ref value);
            }
            /// <summary>
            /// 反序列化更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal override bool LoadUpdate(valueType value, byte* data, int size)
            {
                checkSize(data, ref size);
                return fastCSharp.emit.jsonParser.UnsafeParse((char*)data, size >> 1, ref value, parseConfig);
            }
            /// <summary>
            /// 反序列化删除数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="data"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            internal override bool LoadDelete<keyType>(ref keyType value, byte* data, int size)
            {
                checkSize(data, ref size);
                return fastCSharp.emit.jsonParser.UnsafeParse((char*)data, size >> 1, ref value);
            }
            /// <summary>
            /// 序列化成员集合
            /// </summary>
            /// <param name="stream"></param>
            internal override void Members(unmanagedStream stream)
            {
                log.Error.Throw(log.exceptionType.ErrorOperation);
            }
            /// <summary>
            /// 序列化参数
            /// </summary>
            private readonly fastCSharp.emit.jsonSerializer.config serializeConfig = new jsonSerializer.config { IsMemberMapErrorLog = false, CheckLoopDepth = fastCSharp.config.appSetting.SerializeDepth };
            /// <summary>
            /// JSON序列化流
            /// </summary>
            private readonly charStream jsonStream = new charStream(1 << 10);
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            internal override void Insert(valueType value, unmanagedStream stream)
            {
                jsonStream.Clear();
                serializeConfig.MemberMap = memberMap;
                fastCSharp.emit.jsonSerializer.ToJson(value, jsonStream, serializeConfig);
                fastCSharp.web.ajax.FormatJavascript(jsonStream, stream);
                if ((stream.Length & 2) != 0) stream.Write(' ');
            }
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            /// <param name="memberMap"></param>
            internal override void Update(valueType value, unmanagedStream stream, memberMap memberMap)
            {
                jsonStream.Clear();
                serializeConfig.MemberMap = memberMap;
                fastCSharp.emit.jsonSerializer.ToJson(value, jsonStream, serializeConfig);
                fastCSharp.web.ajax.FormatJavascript(jsonStream, stream);
                if ((stream.Length & 2) != 0) stream.Write(' ');
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="stream"></param>
            internal override void Delete<keyType>(keyType value, unmanagedStream stream)
            {
                jsonStream.Clear();
                serializeConfig.MemberMap = null;
                fastCSharp.emit.jsonSerializer.ToJson(value, jsonStream, serializeConfig);
                fastCSharp.web.ajax.FormatJavascript(jsonStream, stream);
                if ((stream.Length & 2) != 0) stream.Write(' ');
            }
            static unsafe jsonSerializer()
            {
                subArray<memberIndex> members = emit.jsonParser.typeParser<valueType>.GetMembers();
                if (members.length != 0)
                {
                    byte* map = stackalloc byte[memberMap.Type.MemberMapSize];
                    memberMap modelMemberMap = memoryDatabaseModel<valueType>.MemberMap;
                    unsafer.memory.Fill(map, 0UL, memberMap.Type.MemberMapSize >> 3);
                    foreach (memberIndex member in members) map[member.MemberIndex >> 3] |= (byte)(1 << (member.MemberIndex & 7));
                    members = emit.jsonSerializer.typeToJsoner<valueType>.GetMembers();
                    foreach (memberIndex member in members)
                    {
                        if ((map[member.MemberIndex >> 3] & (1 << (member.MemberIndex & 7))) != 0)
                        {
                            if (member.IsField)
                            {
                                if (modelMemberMap.IsMember(member.MemberIndex)) memberMap.SetMember(member.MemberIndex);
                            }
                            else
                            {
                                emit.dataMember attribute = member.GetAttribute<emit.dataMember>(true, true);
                                if (attribute == null || attribute.IsSetup) memberMap.SetMember(member.MemberIndex);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 日志类型
        /// </summary>
        internal enum logType : byte
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown,
            /// <summary>
            /// 添加对象
            /// </summary>
            Insert,
            /// <summary>
            /// 修改对象
            /// </summary>
            Update,
            /// <summary>
            /// 删除对象
            /// </summary>
            Delete,
            /// <summary>
            /// 成员变换
            /// </summary>
            MemberData
        }
        /// <summary>
        /// 内存数据库表格操作工具
        /// </summary>
        public abstract class table : IDisposable
        {
            ///// <summary>
            ///// 数据库日志文件最小刷新尺寸
            ///// </summary>
            //private readonly long minRefreshSize;
            /// <summary>
            /// 日志数据长度
            /// </summary>
            protected long dataSize;
            /// <summary>
            /// 表格名称
            /// </summary>
            protected readonly string name;
            /// <summary>
            /// 序列化输出缓冲区
            /// </summary>
            protected unmanagedStream stream;
            /// <summary>
            /// 成员集合日志字节数
            /// </summary>
            protected int dataMemberSize;
            /// <summary>
            /// 是否已经释放资源
            /// </summary>
            protected int isDisposed;
            /// <summary>
            /// 是否已经释放资源
            /// </summary>
            public bool IsDisposed
            {
                get { return isDisposed != 0; }
            }
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            protected table(string name, int minRefreshSize)
            {
                this.name = name;
                //this.minRefreshSize = (long)(minRefreshSize < fastCSharp.config.memoryDatabase.DefaultMinRefreshSize ? fastCSharp.config.memoryDatabase.Default.MinRefreshSize : minRefreshSize) << 10;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            protected virtual void dispose()
            {
                fastCSharp.pub.Dispose(ref stream);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                if (Interlocked.Increment(ref isDisposed) == 1) dispose();
            }
        }
        /// <summary>
        /// 内存数据库表格操作工具
        /// </summary>
        /// <typeparam name="modelType">模型类型</typeparam>
        public abstract class table<modelType> : table
            where modelType : class
        {
            /// <summary>
            /// 数据序列化
            /// </summary>
            internal serializer<modelType> Serializer;
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="serializeType">数据序列化类型</param>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            protected table(serializeType serializeType, string name, int minRefreshSize)
                : base(name, minRefreshSize)
            {
                switch (serializeType)
                {
                    case serializeType.Index:
                        Serializer = new indexSerializer<modelType>();
                        break;
                    case serializeType.Data:
                        Serializer = new dataSerializer<modelType>();
                        break;
                    case serializeType.Json:
                        Serializer = new jsonSerializer<modelType>();
                        break;
                }
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            protected override void dispose()
            {
                base.dispose();
                fastCSharp.pub.Dispose(ref Serializer);
            }
            /// <summary>
            /// 获取更新成员位图
            /// </summary>
            /// <param name="memberMap"></param>
            /// <returns></returns>
            protected abstract memberMap<modelType> GetUpdateMemberMap(memberMap<modelType> memberMap);
        }
        /// <summary>
        /// 内存数据库表格操作工具
        /// </summary>
        /// <typeparam name="valueType">表格类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        public abstract class table<valueType, modelType> : table<modelType>
            where valueType : class, modelType
            where modelType : class
        {
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="serializeType">数据序列化类型</param>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            protected table(serializeType serializeType, string name, int minRefreshSize)
                : base(serializeType, name, minRefreshSize)
            {
            }
        }
        /// <summary>
        /// 内存数据库表格操作工具
        /// </summary>
        /// <typeparam name="valueType">表格类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <typeparam name="keyType">关键字类型</typeparam>
        public abstract class table<valueType, modelType, keyType> : table<valueType, modelType>
            where valueType : class, modelType
            where modelType : class
            where keyType : IEquatable<keyType>
        {
#if NOJIT
            /// <summary>
            /// 自增数据加载基本缓存接口
            /// </summary>
            protected fastCSharp.memoryDatabase.cache.ILoadCacheKey cache;
            /// <summary>
            /// 自增数据加载基本缓存接口
            /// </summary>
            public fastCSharp.memoryDatabase.cache.ILoadCacheKey Cache
            {
                get { return cache; }
            }
#else
            /// <summary>
            /// 自增数据加载基本缓存接口
            /// </summary>
            protected fastCSharp.memoryDatabase.cache.ILoadCache<valueType, modelType, keyType> cache;
            /// <summary>
            /// 自增数据加载基本缓存接口
            /// </summary>
            public fastCSharp.memoryDatabase.cache.ILoadCache<valueType, modelType, keyType> Cache
            {
                get { return cache; }
            }
#endif
            /// <summary>
            /// 获取关键字
            /// </summary>
            public readonly Func<modelType, keyType> GetPrimaryKey;
            /// <summary>
            /// 设置关键字
            /// </summary>
            public readonly Action<modelType, keyType> SetPrimaryKey;
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="cache">自增数据加载基本缓存接口</param>
            /// <param name="serializeType">数据序列化类型</param>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            /// <param name="getKey">获取关键字</param>
            /// <param name="setKey">设置关键字</param>
#if NOJIT
            protected table(fastCSharp.memoryDatabase.cache.ILoadCacheKey cache
#else
            protected table(fastCSharp.memoryDatabase.cache.ILoadCache<valueType, modelType, keyType> cache
#endif
                , serializeType serializeType , string name, int minRefreshSize
                , Func<modelType, keyType> getKey, Action<modelType, keyType> setKey)
                : base(serializeType, name ?? typeof(valueType).onlyName(), minRefreshSize)
            {
                if (cache == null || Serializer == null) log.Error.Throw(log.exceptionType.Null);
                if (setKey == null || getKey == null)
                {
                    cache.Loaded(false);
                    log.Error.Throw(log.exceptionType.Null);
                }
                this.cache = cache;
                this.SetPrimaryKey = setKey;
                this.GetPrimaryKey = getKey;
            }
            /// <summary>
            /// 设置文件头数据
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="bufferSize"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected unsafe void headerData(byte* buffer, int bufferSize)
            {
                *(int*)buffer = fastCSharp.emit.pub.PuzzleValue;
                *(int*)(buffer + sizeof(int)) = sizeof(int) * 4;
                *(int*)(buffer + sizeof(int) * 2) = bufferSize == 0 ? fastCSharp.config.memoryDatabase.Default.PhysicalBufferSize : bufferSize;
                *(int*)(buffer + sizeof(int) * 3) = (int)(byte)Serializer.Type;
            }
            /// <summary>
            /// 加载数据
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            protected unsafe int load(subArray<byte> data)
            {
                int count = data.length;
                if (count >= sizeof(int))
                {
                    fixed (byte* dataFixed = data.array)
                    {
                        valueType value;
                        byte* buffer = dataFixed + data.startIndex;
                        do
                        {
                            if (*(int*)buffer <= sizeof(int) * 2 || (count -= *(int*)buffer) < 0) return 0;
                            switch (*(int*)(buffer + sizeof(int)))
                            {
                                case (int)(byte)logType.Insert:
                                    if (!Serializer.LoadInsert(value = fastCSharp.emit.constructor<valueType>.New(), buffer + sizeof(int) * 2, *(int*)buffer - sizeof(int) * 2)) return 0;
                                    cache.LoadInsert(value, GetPrimaryKey(value), *(int*)buffer);
                                    dataSize += *(int*)buffer;
                                    break;
                                case (int)(byte)logType.Update:
                                    if (!Serializer.LoadUpdate(value = fastCSharp.emit.constructor<valueType>.New(), buffer + sizeof(int) * 2, *(int*)buffer - sizeof(int) * 2)) return 0;
                                    cache.LoadUpdate(value, GetPrimaryKey(value), Serializer.LoadMemberMap);
                                    break;
                                case (int)(byte)logType.Delete:
                                    keyType key = default(keyType);
                                    if (!Serializer.LoadDelete(ref key, buffer + sizeof(int) * 2, *(int*)buffer - sizeof(int) * 2)) return 0;
                                    dataSize -= cache.LoadDelete(key);
                                    break;
                                case (int)(byte)logType.MemberData:
                                    if (!Serializer.LoadMembers(buffer + sizeof(int) * 2, *(int*)buffer - sizeof(int) * 2)) return 0;
                                    dataSize -= dataMemberSize;
                                    dataSize += (dataMemberSize = *(int*)buffer);
                                    break;
                                default:
                                    return 0;
                            }
                            buffer += *(int*)buffer;
                        }
                        while (count != 0);
                    }
                    return 1;
                }
                return 0;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            protected override void dispose()
            {
                base.dispose();
                fastCSharp.pub.Dispose(ref cache);
            }
        }
        /// <summary>
        /// 内存数据库表格操作工具(本地)
        /// </summary>
        /// <typeparam name="valueType">表格类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <typeparam name="keyType">关键字类型</typeparam>
        public abstract class localTable<valueType, modelType, keyType> : table<valueType, modelType, keyType> 
            where valueType : class, modelType
            where modelType : class
            where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 数据库物理层
            /// </summary>
            internal physical Physical;
            /// <summary>
            /// 数据库物理层访问锁
            /// </summary>
            protected readonly object physicalLock = new object();
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="cache">自增数据加载基本缓存接口</param>
            /// <param name="serializeType">数据序列化类型</param>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            /// <param name="bufferSize">缓冲区字节大小</param>
            /// <param name="getKey">获取关键字</param>
            /// <param name="setKey">设置关键字</param>
#if NOJIT
            protected localTable(fastCSharp.memoryDatabase.cache.ILoadCacheKey cache
#else
            protected localTable(fastCSharp.memoryDatabase.cache.ILoadCache<valueType, modelType, keyType> cache
#endif
                , serializeType serializeType, string name, int minRefreshSize, int bufferSize
                , Func<modelType, keyType> getKey, Action<modelType, keyType> setKey)
                : base(cache, serializeType, name, minRefreshSize, getKey, setKey)
            {
                threading.threadPool.TinyPool.FastStart(open, ref bufferSize);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            protected override void dispose()
            {
                base.dispose();
                fastCSharp.pub.Dispose(ref Physical);
            }
            /// <summary>
            /// 打开数据库文件
            /// </summary>
            /// <param name="bufferSize"></param>
            private unsafe void open(int bufferSize)
            {
                int isLoaded = 0, isPhysicalLoaded = 1;
                try
                {
                    if ((Physical = new physical(name)).LastException == null)
                    {
                        if (Physical.IsLoader)
                        {
                            isPhysicalLoaded = 0;
                            subArray<byte> header = Physical.LoadHeader();
                            if (header.length == sizeof(int) * 2)
                            {
                                fixed (byte* headerFixed = header.array)
                                {
                                    byte* buffer = headerFixed + header.startIndex;
                                    if (((*(int*)buffer ^ (bufferSize == 0 ? fastCSharp.config.memoryDatabase.Default.PhysicalBufferSize : bufferSize)) | (*(int*)(buffer + sizeof(int)) ^ (int)(byte)Serializer.Type)) == 0)
                                    {
                                        Serializer.ReadyLoad();
                                        try
                                        {
                                            do
                                            {
                                                subArray<byte> data = Physical.Load();
                                                if (data.array == null) break;
                                                if (data.length == 0)
                                                {
                                                    if (Physical.Loaded(true))
                                                    {
                                                        isPhysicalLoaded = 1;
                                                        if (Serializer.IsCurrentDataMembers || members())
                                                        {
                                                            cache.Loaded(true);
                                                            isLoaded = 1;
                                                            return;
                                                        }
                                                    }
                                                    break;
                                                }
                                                if (load(data) == 0) break;
                                            }
                                            while (true);
                                        }
                                        finally { Serializer.Loaded(); }
                                    }
                                }
                            }
                            log.Error.Add("数据库 " + name + " 加载失败", null, false);
                        }
                        else
                        {
                            byte* buffer = stackalloc byte[sizeof(int) * 4];
                            headerData(buffer, bufferSize);
                            if (Physical.Create(buffer, sizeof(int) * 4) && (!Serializer.IsDataMember || members()))
                            {
                                cache.Loaded(true);
                                isLoaded = 1;
                                return;
                            }
                            log.Error.Add("数据库 " + name + " 创建失败", null, false);
                        }
                    }
                    else log.Error.Add("数据库 " + name + " 打开失败", null, false);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, "数据库 " + name + " 打开失败", false);
                }
                finally
                {
                    if (isPhysicalLoaded == 0) Physical.Loaded(false);
                    if (isLoaded == 0) Dispose();
                }
            }
            /// <summary>
            /// 成员变换
            /// </summary>
            /// <returns></returns>
            private unsafe bool members()
            {
                pointer data = fastCSharp.unmanagedPool.StreamBuffers.Get();
                unmanagedStream stream = Interlocked.Exchange(ref this.stream, null);
                try
                {
                    if (stream == null) stream = new unmanagedStream(data.Byte, fastCSharp.unmanagedPool.StreamBuffers.Size);
                    else stream.UnsafeReset(data.Byte, fastCSharp.unmanagedPool.StreamBuffers.Size);
                    stream.UnsafeAddLength(sizeof(int) * 2);
                    Serializer.Members(stream);
                    byte* buffer = stream.data.Byte;
                    *(int*)(buffer + sizeof(int)) = (int)(byte)logType.MemberData;
                    if (Physical.Append(buffer, *(int*)buffer = stream.Length) != 0)
                    {
                        dataSize -= dataMemberSize;
                        dataSize += (dataMemberSize = stream.Length);
                        return true;
                    }
                }
                finally
                {
                    if (isDisposed == 0)
                    {
                        if (stream != null) Interlocked.Exchange(ref this.stream, stream);
                    }
                    else fastCSharp.pub.Dispose(ref stream);
                    fastCSharp.unmanagedPool.StreamBuffers.Push(ref data);
                }
                return false;
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="isCopy">是否复制缓存数据</param>
            /// <returns></returns>
            public abstract valueType Insert(valueType value, bool isCopy = true);
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="memberMap"></param>
            /// <returns></returns>
            public unsafe valueType Update(valueType value, memberMap<modelType> memberMap)
            {
                if (value != null && memberMap != null && !memberMap.IsDefault)
                {
                    Exception exception = null;
                    unmanagedStream stream = null;
                    keyType key = GetPrimaryKey(value);
                    int isError = 0;
                    Monitor.Enter(physicalLock);
                    try
                    {
                        if (cache.ContainsKey(key))
                        {
                            stream = Interlocked.Exchange(ref this.stream, null);
                            byte[] buffer = Physical.LocalBuffer();
                            if (buffer == null)
                            {
                                value = null;
                                isError = 1;
                            }
                            else
                            {
                                fixed (byte* bufferFixed = buffer)
                                {
                                    if (stream == null) stream = new unmanagedStream(bufferFixed + Physical.BufferIndex, buffer.Length - Physical.BufferIndex);
                                    else stream.UnsafeReset(bufferFixed + Physical.BufferIndex, buffer.Length - Physical.BufferIndex);
                                    stream.UnsafeAddLength(sizeof(int) * 2);
                                    using (memberMap<modelType> updateMemberMap = GetUpdateMemberMap(memberMap)) Serializer.Update(value, stream, updateMemberMap);
                                    byte* data = stream.data.Byte;
                                    *(int*)(data + sizeof(int)) = (int)(byte)logType.Update;
                                    *(int*)data = stream.Length;
                                    isError = 1;
                                    if (data == bufferFixed + Physical.BufferIndex)
                                    {
                                        Physical.BufferIndex += stream.Length;
#if NOJIT
                                        if ((value = (valueType)cache.Update(value, key, memberMap)) == null) isError = 0;
#else
                                        if ((value = cache.Update(value, key, memberMap)) == null) isError = 0;
#endif
                                    }
                                    else if (Physical.Append(stream.data.Byte, stream.Length) == 0) value = null;
#if NOJIT
                                    else if ((value = (valueType)cache.Update(value, key, memberMap)) == null) isError = 0;
#else
                                    else if ((value = cache.Update(value, key, memberMap)) == null) isError = 0;
#endif
                                }
                            }
                        }
                        else value = null;
                    }
                    catch (Exception error)
                    {
                        value = null;
                        exception = error;
                    }
                    finally
                    {
                        if (isDisposed == 0)
                        {
                            if (stream != null) Interlocked.Exchange(ref this.stream, stream);
                        }
                        else fastCSharp.pub.Dispose(ref stream);
                        Monitor.Exit(physicalLock);
                        if (exception != null) log.Error.Add(exception, null, false);
                    }
                    if (value != null)
                    {
                        Physical.WaitBuffer();
                        return value;
                    }
                    if (isError != 0) Dispose();
                }
                return null;
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Delete(valueType value)
            {
                return value != null ? Delete(GetPrimaryKey(value)) : null;
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public unsafe valueType Delete(keyType key)
            {
                Exception exception = null;
                unmanagedStream stream = null;
                valueType value = null;
                int isError = 0, logSize;
                Monitor.Enter(physicalLock);
                try
                {
                    if (cache.ContainsKey(key))
                    {
                        stream = Interlocked.Exchange(ref this.stream, null);
                        byte[] buffer = Physical.LocalBuffer();
                        if (buffer == null) isError = 1;
                        else
                        {
                            fixed (byte* bufferFixed = buffer)
                            {
                                if (stream == null) stream = new unmanagedStream(bufferFixed + Physical.BufferIndex, buffer.Length - Physical.BufferIndex);
                                else stream.UnsafeReset(bufferFixed + Physical.BufferIndex, buffer.Length - Physical.BufferIndex);
                                stream.UnsafeAddLength(sizeof(int) * 2);
                                Serializer.Delete(key, stream);
                                byte* data = stream.data.Byte;
                                *(int*)(data + sizeof(int)) = (int)(byte)logType.Delete;
                                *(int*)data = stream.Length;
                                isError = 1;
                                if (data == bufferFixed + Physical.BufferIndex)
                                {
                                    Physical.BufferIndex += stream.Length;
#if NOJIT
                                    if ((value = (valueType)cache.Delete(key, out logSize)) != null)
#else
                                    if ((value = cache.Delete(key, out logSize)) != null)
#endif
                                    {
                                        dataSize -= logSize;
                                        isError = 0;
                                    }
                                }
#if NOJIT
                                else if (Physical.Append(stream.data.Byte, stream.Length) != 0 && (value = (valueType)cache.Delete(key, out logSize)) != null)
#else
                                else if (Physical.Append(stream.data.Byte, stream.Length) != 0 && (value = cache.Delete(key, out logSize)) != null)
#endif
                                {
                                    dataSize -= logSize;
                                    isError = 0;
                                }
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    exception = error;
                }
                finally
                {
                    if (isDisposed == 0)
                    {
                        if (stream != null) Interlocked.Exchange(ref this.stream, stream);
                    }
                    else fastCSharp.pub.Dispose(ref stream);
                    Monitor.Exit(physicalLock);
                    if (exception != null) log.Error.Add(exception, null, false);
                }
                if (value != null)
                {
                    Physical.WaitBuffer();
                    return value;
                }
                if (isError != 0) Dispose();
                return null;
            }
            /// <summary>
            /// 刷新写入文件缓存区
            /// </summary>
            /// <param name="isWriteFile">是否写入文件</param>
            /// <returns>是否操作成功</returns>
            public bool Flush(bool isWriteFile)
            {
                if (Physical.Flush() && Physical.FlushFile(isWriteFile))
                {
                    return true;
                }
                Dispose();
                return false;
            }
        }
        /// <summary>
        /// 内存数据库表格操作工具(远程)数据加载
        /// </summary>
        internal abstract class remoteTableLoader
        {
            /// <summary>
            /// 数据加载完毕
            /// </summary>
            internal abstract void End();
            /// <summary>
            /// 数据加载完毕
            /// </summary>
            internal abstract void Loaded();
        }
        /// <summary>
        /// 内存数据库表格操作工具(远程)
        /// </summary>
        /// <typeparam name="valueType">表格类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <typeparam name="keyType">关键字类型</typeparam>
        public abstract class remoteTable<valueType, modelType, keyType> : table<valueType, modelType, keyType> 
            where valueType : class, modelType
            where modelType : class
            where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 数据加载
            /// </summary>
            private sealed class loader : remoteTableLoader
            {
                /// <summary>
                /// 内存数据库表格操作工具
                /// </summary>
                private remoteTable<valueType, modelType, keyType> table;
                /// <summary>
                /// 加载数据
                /// </summary>
                private Action<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer>> onLoadHandle;
                /// <summary>
                /// 物理层缓冲区字节数
                /// </summary>
                private int bufferSize;
                /// <summary>
                /// 数据是否加载成功
                /// </summary>
                private int isLoaded;
                /// <summary>
                /// 是否需要通知物理层加载失败
                /// </summary>
                private int isPhysicalLoaded = 1;
                /// <summary>
                /// 是否已经初始化序列化
                /// </summary>
                private int isSerializerLoaded;
                /// <summary>
                /// 数据加载
                /// </summary>
                /// <param name="table">内存数据库表格操作工具</param>
                /// <param name="bufferSize">物理层缓冲区字节数</param>
                public loader(remoteTable<valueType, modelType, keyType> table, int bufferSize)
                {
                    this.table = table;
                    this.bufferSize = bufferSize;
                }
#if NotFastCSharpCode
#else
                /// <summary>
                /// 打开数据库
                /// </summary>
                public void Open()
                {
                    try
                    {
                        table.client.open(table.name, onOpen);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, "数据库 " + table.name + " 打开失败", false);
                        end();
                    }
                }
                /// <summary>
                /// 数据加载完毕
                /// </summary>
                internal override void End()
                {
                    end();
                }
                /// <summary>
                /// 数据加载完毕
                /// </summary>
                private void end()
                {
                    if (isSerializerLoaded != 0 && table.Serializer != null) table.Serializer.Loaded();
                    if (isPhysicalLoaded == 0)
                    {
                        try
                        {
                            table.client.loaded(table.physicalIdentity, false);
                        }
                        catch (Exception error)
                        {
                            log.Default.Add(error, null, false);
                        }
                    }
                    if (isLoaded == 0) table.Dispose();
                }
                /// <summary>
                /// 打开数据库
                /// </summary>
                /// <param name="identity">数据库物理层初始化信息</param>
                private unsafe void onOpen(fastCSharp.net.returnValue<physicalServer.physicalIdentity> identity)
                {
                    if (identity.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        table.physicalIdentity = identity.Value.Identity;
                        if (identity.Value.IsOpen)
                        {
                            try
                            {
                                if (identity.Value.IsLoader)
                                {
                                    isPhysicalLoaded = 0;
                                    table.client.loadHeader(table.physicalIdentity, onLoadHeader);
                                    return;
                                }
                                else
                                {
                                    byte* buffer = stackalloc byte[sizeof(physicalServer.timeIdentity) + sizeof(int) * 4];
                                    *(physicalServer.timeIdentity*)buffer = table.physicalIdentity;
                                    table.headerData(buffer + sizeof(physicalServer.timeIdentity), bufferSize);
                                    (table.stream = new unmanagedStream(buffer, sizeof(physicalServer.timeIdentity) + sizeof(int) * 4)).UnsafeAddLength(sizeof(physicalServer.timeIdentity) + sizeof(int) * 4);
                                    if (table.client.create(table.stream) && (!table.Serializer.IsDataMember || members()))
                                    {
                                        table.cache.Loaded(true);
                                        isLoaded = 1;
                                        end();
                                        return;
                                    }
                                    log.Error.Add("数据库 " + table.name + " 创建失败", new System.Diagnostics.StackFrame(), false);
                                }
                            }
                            catch (Exception error)
                            {
                                log.Error.Add(error, "数据库 " + table.name + (identity.Value.IsLoader ? " 加载失败" : " 创建失败"), false);
                            }
                            end();
                            return;
                        }
                    }
                    log.Error.Add("数据库 " + table.name + " 打开失败", new System.Diagnostics.StackFrame(), false);
                    end();
                }
                /// <summary>
                /// 成员变换
                /// </summary>
                /// <returns></returns>
                private unsafe bool members()
                {
                    pointer data = fastCSharp.unmanagedPool.StreamBuffers.Get();
                    unmanagedStream stream = Interlocked.Exchange(ref table.stream, null);
                    try
                    {
                        if (stream == null) stream = new unmanagedStream(data.Byte, fastCSharp.unmanagedPool.StreamBuffers.Size);
                        else stream.UnsafeReset(data.Byte, fastCSharp.unmanagedPool.StreamBuffers.Size);
                        stream.UnsafeAddLength(sizeof(physicalServer.timeIdentity) + sizeof(int) * 2);
                        table.Serializer.Members(stream);
                        byte* buffer = stream.data.Byte;
                        *(physicalServer.timeIdentity*)buffer = table.physicalIdentity;
                        *(int*)(buffer + (sizeof(physicalServer.timeIdentity) + sizeof(int))) = (int)(byte)logType.MemberData;
                        *(int*)(buffer + sizeof(physicalServer.timeIdentity)) = stream.Length - sizeof(physicalServer.timeIdentity);
                        int value = table.client.append(stream);
                        if (value != 0)
                        {
                            table.dataSize -= table.dataMemberSize;
                            table.dataSize += (table.dataMemberSize = stream.Length);
                            return true;
                        }
                    }
                    finally
                    {
                        if (table.isDisposed == 0)
                        {
                            if (stream != null) Interlocked.Exchange(ref table.stream, stream);
                        }
                        else fastCSharp.pub.Dispose(ref stream);
                        fastCSharp.unmanagedPool.StreamBuffers.Push(ref data);
                    }
                    return false;
                }
                /// <summary>
                /// 加载文件头部数据
                /// </summary>
                /// <param name="data"></param>
                private unsafe void onLoadHeader(fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer> data)
                {
                    if (data.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        subArray<byte> header = data.Value.Buffer;
                        if (header.length == sizeof(int) * 2)
                        {
                            fixed (byte* headerFixed = header.array)
                            {
                                byte* buffer = headerFixed + header.startIndex;
                                if (((*(int*)buffer ^ (bufferSize == 0 ? fastCSharp.config.memoryDatabase.Default.PhysicalBufferSize : bufferSize)) | (*(int*)(buffer + sizeof(int)) ^ (int)(byte)table.Serializer.Type)) == 0)
                                {
                                    try
                                    {
                                        table.Serializer.ReadyLoad();
                                        isSerializerLoaded = 1;
                                        load();
                                    }
                                    catch (Exception error)
                                    {
                                        log.Error.Add(error, "数据库 " + table.name + " 加载失败", false);
                                        fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.MemoryDatabaseTableRemoteTableLoadEnd);
                                    }
                                    return;
                                }
                            }
                        }
                    }
                    log.Error.Add("数据库 " + table.name + " 加载失败", new System.Diagnostics.StackFrame(), false);
                    fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.MemoryDatabaseTableRemoteTableLoadEnd);
                }
                /// <summary>
                /// 加载数据
                /// </summary>
                private void load()
                {
                    try
                    {
                        if (onLoadHandle == null) onLoadHandle = onLoad;
                        table.client.load(table.physicalIdentity, onLoadHandle);
                        return;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, "数据库 " + table.name + " 加载失败", false);
                    }
                    fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.MemoryDatabaseTableRemoteTableLoadEnd);
                }
                /// <summary>
                /// 加载数据
                /// </summary>
                /// <param name="buffer">数据缓冲区</param>
                private void onLoad(fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer> buffer)
                {
                    if (buffer.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        subArray<byte> data = buffer.Value.Buffer;
                        if (data.array != null)
                        {
                            try
                            {
                                if (data.length == 0)
                                {
                                    fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.MemoryDatabaseTableRemoteTableLoaded);
                                    return;
                                }
                                else if (table.load(data) != 0)
                                {
                                    load();
                                    return;
                                }
                            }
                            catch (Exception error)
                            {
                                log.Error.Add(error, "数据库 " + table.name + " 加载失败", false);
                                fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.MemoryDatabaseTableRemoteTableLoadEnd);
                                return;
                            }
                        }
                    }
                    log.Error.Add("数据库 " + table.name + " 加载失败", new System.Diagnostics.StackFrame(), false);
                    fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.MemoryDatabaseTableRemoteTableLoadEnd);
                }
                /// <summary>
                /// 数据加载完毕
                /// </summary>
                internal override void Loaded()
                {
                    try
                    {
                        if (table.client.loaded(table.physicalIdentity, true))
                        {
                            isPhysicalLoaded = 1;
                            if (table.Serializer.IsCurrentDataMembers || members())
                            {
                                table.cache.Loaded(true);
                                isLoaded = 1;
                                return;
                            }
                        }
                        log.Error.Add("数据库 " + table.name + " 加载失败", new System.Diagnostics.StackFrame(), false);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, "数据库 " + table.name + " 加载失败", false);
                    }
                    finally { end(); }
                }
#endif
            }
#if NotFastCSharpCode
#else
            /// <summary>
            /// 内存数据库客户端
            /// </summary>
            protected fastCSharp.memoryDatabase.physicalServer.tcpClient client;
#endif
            /// <summary>
            /// 数据库物理层唯一标识
            /// </summary>
            protected physicalServer.timeIdentity physicalIdentity;
            /// <summary>
            /// 内存池
            /// </summary>
            protected unmanagedPool memoryPool;
            /// <summary>
            /// 缓存访问锁
            /// </summary>
            protected readonly object cacheLock = new object();
            /// <summary>
            /// 最后一次添加数据操作返回值
            /// </summary>
            protected int appendValue;
            /// <summary>
            /// 是否自动关系内存数据库客户端
            /// </summary>
            private bool isCloseCient;
            /// <summary>
            /// 自动关闭客户端时是否等待数据库关闭
            /// </summary>
            private bool isWaitClose;
#if NotFastCSharpCode
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="cache">自增数据加载基本缓存接口</param>
            /// <param name="serializeType">序列化类型</param>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            /// <param name="bufferSize">缓冲区字节大小</param>
            /// <param name="getKey">获取关键字</param>
            /// <param name="setKey">设置关键字</param>
            /// <param name="memoryPool">数据缓冲区内存池</param>
            /// <param name="isWaitClose">是否等待数据库关闭</param>
            /// <param name="isCloseCient">是否自动关系内存数据库客户端</param>
            protected remoteTable(fastCSharp.memoryDatabase.cache.ILoadCache<valueType, modelType, keyType> cache
                , serializeType serializeType, string name, int minRefreshSize, int bufferSize
                , Func<modelType, keyType> getKey, Action<modelType, keyType> setKey, unmanagedPool memoryPool, bool isWaitClose, bool isCloseCient)
                : base(cache, serializeType, name, minRefreshSize, getKey, setKey)
            {
                fastCSharp.log.Error.Throw(log.exceptionType.NotFastCSharpCode);
            }
#else
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="client"></param>
            /// <param name="cache">自增数据加载基本缓存接口</param>
            /// <param name="serializeType">序列化类型</param>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            /// <param name="bufferSize">缓冲区字节大小</param>
            /// <param name="getKey">获取关键字</param>
            /// <param name="setKey">设置关键字</param>
            /// <param name="memoryPool">数据缓冲区内存池</param>
            /// <param name="isWaitClose">是否等待数据库关闭</param>
            /// <param name="isCloseCient">是否自动关系内存数据库客户端</param>
#if NOJIT
            protected remoteTable(fastCSharp.memoryDatabase.physicalServer.tcpClient client, fastCSharp.memoryDatabase.cache.ILoadCacheKey cache
#else
            protected remoteTable(fastCSharp.memoryDatabase.physicalServer.tcpClient client, fastCSharp.memoryDatabase.cache.ILoadCache<valueType, modelType, keyType> cache
#endif
                , serializeType serializeType, string name, int minRefreshSize, int bufferSize
                , Func<modelType, keyType> getKey, Action<modelType, keyType> setKey, unmanagedPool memoryPool, bool isWaitClose, bool isCloseCient)
                : base(cache, serializeType, name, minRefreshSize, getKey, setKey)
            {
                if (client == null) log.Error.Throw(log.exceptionType.Null);
                this.client = client;
                this.isCloseCient = isCloseCient;
                this.isWaitClose = isWaitClose;
                this.memoryPool = memoryPool ?? unmanagedPool.TinyBuffers;
                new loader(this, bufferSize).Open();
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            protected override void dispose()
            {
                base.dispose();
                if (isCloseCient)
                {
                    try
                    {
                        if (isWaitClose)
                        {
                            client.flush(physicalIdentity);
                            client.flushFile(physicalIdentity, false);
                        }
                        client.close(physicalIdentity);
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, null, false);
                    }
                    fastCSharp.pub.Dispose(ref client);
                }
                typePool<insertWaiter>.Clear();
                typePool<updateWaiter>.Clear();
                typePool<deleteWaiter>.Clear();
                typePool<insertCallbacker>.Clear();
                typePool<updateCallbacker>.Clear();
                typePool<deleteCallbacker>.Clear();
            }
#endif
            /// <summary>
            /// 回调操作
            /// </summary>
            private unsafe abstract class callbacker : IDisposable
            {
                /// <summary>
                /// 内存数据库表格操作工具
                /// </summary>
                protected memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table;
                /// <summary>
                /// 内存池
                /// </summary>
                protected unmanagedPool memoryPool;
                /// <summary>
                /// 内存缓冲区
                /// </summary>
                protected pointer buffer;
                /// <summary>
                /// 内存流
                /// </summary>
                internal unmanagedStream Stream;
                /// <summary>
                /// 操作回调委托
                /// </summary>
                internal Action<fastCSharp.net.returnValue<int>> Callback;
                /// <summary>
                /// 关键字
                /// </summary>
                protected keyType key;
                /// <summary>
                /// 添加数据
                /// </summary>
                protected callbacker(unmanagedPool memoryPool)
                {
                    buffer = (this.memoryPool = memoryPool).Get();
                    Stream = new unmanagedStream(buffer.Byte, memoryPool.Size);
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public virtual void Dispose()
                {
                    Stream.Dispose();
                    if (memoryPool != null)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                }
            }
            /// <summary>
            /// 同步操作
            /// </summary>
            private unsafe abstract class waiter : callbacker
            {
                /// <summary>
                /// 数据
                /// </summary>
                protected valueType value;
                /// <summary>
                /// 同步等待事件
                /// </summary>
                protected autoWaitHandle waitHandle;
                /// <summary>
                /// 添加数据返回值
                /// </summary>
                protected fastCSharp.net.returnValue<int> returnValue;
                /// <summary>
                /// 添加数据
                /// </summary>
                protected waiter(unmanagedPool memoryPool)
                    : base(memoryPool)
                {
                    waitHandle = new autoWaitHandle(false);
                    Callback = onReturn;
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public override void Dispose()
                {
                    base.Dispose();
                    waitHandle.Set();
                }
                /// <summary>
                /// 添加数据回调委托
                /// </summary>
                /// <param name="returnValue"></param>
                private void onReturn(fastCSharp.net.returnValue<int> returnValue)
                {
                    if (memoryPool != null && Stream.data.data != buffer.Data)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                    this.returnValue = returnValue;
                    waitHandle.Set();
                }
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            private unsafe sealed class insertWaiter : waiter
            {
                /// <summary>
                /// 是否复制缓存数据
                /// </summary>
                private bool isCopy;
                /// <summary>
                /// 添加数据
                /// </summary>
                public valueType Value
                {
                    get
                    {
                        waitHandle.Wait();
                        if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table = this.table;
                            this.table = null;
                            if (returnValue.Value == 0)
                            {
                                this.value = null;
                                this.key = default(keyType);
                                typePool<insertWaiter>.PushNotNull(this);
                                table.Dispose();
                                return null;
                            }
                            valueType value = this.value;
                            keyType key = this.key;
                            int logSize = Stream.Length;
                            bool isCopy = this.isCopy;
                            this.value = null;
                            this.key = default(keyType);
                            typePool<insertWaiter>.PushNotNull(this);
                            Exception exception = null;
                            table.appendValue = returnValue.Value;
                            logSize -= sizeof(physicalServer.timeIdentity);
                            Monitor.Enter(table.cacheLock);
                            try
                            {
                                table.dataSize += logSize;
#if NOJIT
                                value = (valueType)table.cache.Insert(value, key, logSize, isCopy);
#else
                                value = table.cache.Insert(value, key, logSize, isCopy);
#endif
                            }
                            catch (Exception error)
                            {
                                value = null;
                                exception = error;
                            }
                            finally
                            {
                                Monitor.Exit(table.cacheLock);
                                if (exception != null)
                                {
                                    log.Default.Add(exception, null, false);
                                    table.Dispose();
                                }
                            }
                            return value;
                        }
                        this.table = null;
                        this.value = null;
                        this.key = default(keyType);
                        typePool<insertWaiter>.PushNotNull(this);
                        return null;
                    }
                }
                /// <summary>
                /// 添加数据
                /// </summary>
                private insertWaiter(unmanagedPool memoryPool)
                    : base(memoryPool)
                {
                }
                /// <summary>
                /// 取消当前调用
                /// </summary>
                public void Cancel()
                {
                    if (memoryPool != null && Stream.data.data != buffer.Data)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                    this.table = null;
                    this.value = null;
                    this.key = default(keyType);
                    typePool<insertWaiter>.PushNotNull(this);
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <param name="value"></param>
                /// <param name="key"></param>
                /// <param name="isCopy"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(valueType value, keyType key, bool isCopy)
                {
                    this.value = value;
                    this.key = key;
                    this.isCopy = isCopy;
                    Stream.Clear();
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <param name="table"></param>
                /// <returns></returns>
                public static insertWaiter Get(memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table)
                {
                    insertWaiter inserter = typePool<insertWaiter>.Pop();
                    if (inserter == null)
                    {
                        try
                        {
                            inserter = new insertWaiter(table.memoryPool);
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    inserter.table = table;
                    return inserter;
                }
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="isCopy">是否复制缓存数据</param>
            /// <returns></returns>
            public abstract valueType Insert(valueType value, bool isCopy = true);
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="key"></param>
            /// <param name="isCopy">是否复制缓存数据</param>
            /// <returns></returns>
            protected unsafe valueType insert(valueType value, keyType key, bool isCopy)
            {
                insertWaiter inserter = null;
                try
                {
                    if ((inserter = insertWaiter.Get(this)) != null)
                    {
                        inserter.Set(value, key, isCopy);
                        insert(value, inserter);
                        return inserter.Value;
                    }
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
                if (inserter != null) inserter.Cancel();
                return null;
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="callbacker"></param>
            private unsafe void insert(valueType value, callbacker callbacker)
            {
#if NotFastCSharpCode
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                unmanagedStream stream = callbacker.Stream;
                stream.UnsafeAddLength(sizeof(physicalServer.timeIdentity) + sizeof(int) * 2);
                Serializer.Insert(value, stream);
                byte* data = stream.data.Byte;
                *(physicalServer.timeIdentity*)data = physicalIdentity;
                *(int*)(data + sizeof(physicalServer.timeIdentity)) = stream.Length - sizeof(physicalServer.timeIdentity);
                *(int*)(data + (sizeof(physicalServer.timeIdentity) + sizeof(int))) = (int)(byte)memoryDatabaseTable.logType.Insert;
                client.append(stream, callbacker.Callback);
#endif
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            private unsafe sealed class insertCallbacker : callbacker
            {
                /// <summary>
                /// 添加数据
                /// </summary>
                private valueType value;
                /// <summary>
                /// 添加数据回调委托
                /// </summary>
                private Action<valueType> callback;
                /// <summary>
                /// 操作回调是否使用任务模式
                /// </summary>
                private bool isCallbackTask;
                /// <summary>
                /// 是否复制缓存数据
                /// </summary>
                private bool isCopy;
                /// <summary>
                /// 添加数据
                /// </summary>
                private insertCallbacker(unmanagedPool memoryPool)
                    : base(memoryPool)
                {
                    Callback = onReturn;
                }
                /// <summary>
                /// 添加数据回调委托
                /// </summary>
                /// <param name="returnValue"></param>
                private void onReturn(fastCSharp.net.returnValue<int> returnValue)
                {
                    if (memoryPool != null && Stream.data.data != buffer.Data)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                    Action<valueType> callback = this.callback;
                    bool isCallbackTask = this.isCallbackTask;
                    this.callback = null;
                    if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table = this.table;
                        this.table = null;
                        if (returnValue.Value == 0)
                        {
                            this.value = null;
                            this.key = default(keyType);
                            typePool<insertCallbacker>.PushNotNull(this);
                            table.Dispose();
                            if (callback != null)
                            {
                                if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, null);
                                else callback(null);
                            }
                        }
                        else
                        {
                            valueType value = this.value;
                            keyType key = this.key;
                            int logSize = Stream.Length;
                            bool isCopy = this.isCopy;
                            this.value = null;
                            this.key = default(keyType);
                            typePool<insertCallbacker>.PushNotNull(this);
                            Exception exception = null;
                            table.appendValue = returnValue.Value;
                            logSize -= sizeof(physicalServer.timeIdentity);
                            Monitor.Enter(table.cacheLock);
                            try
                            {
                                table.dataSize += logSize;
#if NOJIT
                                value = (valueType)table.cache.Insert(value, key, logSize, isCopy);
#else
                                value = table.cache.Insert(value, key, logSize, isCopy);
#endif
                            }
                            catch (Exception error)
                            {
                                value = null;
                                exception = error;
                            }
                            finally
                            {
                                Monitor.Exit(table.cacheLock);
                                if (exception != null)
                                {
                                    log.Default.Add(exception, null, false);
                                    table.Dispose();
                                }
                            }
                            if (callback != null)
                            {
                                if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, value);
                                else callback(value);
                            }
                        }
                    }
                    else
                    {
                        this.table = null;
                        this.value = null;
                        this.key = default(keyType);
                        typePool<insertCallbacker>.PushNotNull(this);
                        if (callback != null)
                        {
                            if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, null);
                            else callback(null);
                        }
                    }
                }
                /// <summary>
                /// 取消当前调用
                /// </summary>
                public void Cancel()
                {
                    if (memoryPool != null && Stream.data.data != buffer.Data)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                    Action<valueType> callback = this.callback;
                    bool isCallbackTask = this.isCallbackTask;
                    this.table = null;
                    this.value = null;
                    this.key = default(keyType);
                    this.callback = null;
                    typePool<insertCallbacker>.PushNotNull(this);
                    if (callback != null)
                    {
                        if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, null);
                        else callback(null);
                    }
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <param name="value"></param>
                /// <param name="key"></param>
                /// <param name="onInserted"></param>
                /// <param name="isCallbackTask"></param>
                /// <param name="isCopy"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(valueType value, Action<valueType> onInserted, keyType key, bool isCallbackTask, bool isCopy)
                {
                    this.value = value;
                    this.callback = onInserted;
                    this.key = key;
                    this.isCallbackTask = isCallbackTask;
                    this.isCopy = isCopy;
                    Stream.Clear();
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <param name="table"></param>
                /// <returns></returns>
                public static insertCallbacker Get(memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table)
                {
                    insertCallbacker inserter = typePool<insertCallbacker>.Pop();
                    if (inserter == null)
                    {
                        try
                        {
                            inserter = new insertCallbacker(table.memoryPool);
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    inserter.table = table;
                    return inserter;
                }
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="onInserted"></param>
            /// <param name="isCallbackTask">添加数据回调是否使用任务模式</param>
            /// <param name="isCopy">是否复制缓存数据</param>
            /// <returns></returns>
            public abstract void Insert(valueType value, Action<valueType> onInserted, bool isCallbackTask = true, bool isCopy = true);
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="key"></param>
            /// <param name="onInserted"></param>
            /// <param name="isCallbackTask">添加数据回调是否使用任务模式</param>
            /// <param name="isCopy">是否复制缓存数据</param>
            /// <returns></returns>
            protected unsafe bool insert(valueType value, keyType key, Action<valueType> onInserted, bool isCallbackTask, bool isCopy)
            {
                insertCallbacker inserter = null;
                try
                {
                    if ((inserter = insertCallbacker.Get(this)) != null)
                    {
                        inserter.Set(value, onInserted, key, isCallbackTask, isCopy);
                        insert(value, inserter);
                        return true;
                    }
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
                if (inserter != null)
                {
                    inserter.Cancel();
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 更新数据
            /// </summary>
            private unsafe sealed class updateWaiter : waiter
            {
                /// <summary>
                /// 更新成员位图
                /// </summary>
                private memberMap memberMap;
                /// <summary>
                /// 更新数据
                /// </summary>
                public valueType Value
                {
                    get
                    {
                        waitHandle.Wait();
                        if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table = this.table;
                            this.table = null;
                            if (returnValue.Value == 0)
                            {
                                this.value = null;
                                this.key = default(keyType);
                                this.memberMap = null;
                                typePool<updateWaiter>.PushNotNull(this);
                                table.Dispose();
                                return null;
                            }
                            valueType value = this.value;
                            keyType key = this.key;
                            memberMap memberMap = this.memberMap;
                            Exception exception = null;
                            this.value = null;
                            this.key = default(keyType);
                            this.memberMap = null;
                            typePool<updateWaiter>.PushNotNull(this);
                            table.appendValue = returnValue.Value;
                            Monitor.Enter(table.cacheLock);
                            try
                            {
#if NOJIT
                                value = (valueType)table.cache.Update(value, key, memberMap);
#else
                                value = table.cache.Update(value, key, memberMap);
#endif
                            }
                            catch (Exception error)
                            {
                                value = null;
                                exception = error;
                            }
                            finally
                            {
                                Monitor.Exit(table.cacheLock);
                                if (exception != null)
                                {
                                    log.Default.Add(exception, null, false);
                                    table.Dispose();
                                }
                            }
                            return value;
                        }
                        this.table = null;
                        this.value = null;
                        this.key = default(keyType);
                        this.memberMap = null;
                        typePool<updateWaiter>.PushNotNull(this);
                        return null;
                    }
                }
                /// <summary>
                /// 添加数据
                /// </summary>
                private updateWaiter(unmanagedPool memoryPool)
                    : base(memoryPool)
                {
                }
                /// <summary>
                /// 取消当前调用
                /// </summary>
                public void Cancel()
                {
                    if (memoryPool != null && Stream.data.data != buffer.Data)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                    this.table = null;
                    this.value = null;
                    this.key = default(keyType);
                    memberMap = null;
                    typePool<updateWaiter>.PushNotNull(this);
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <param name="value"></param>
                /// <param name="key"></param>
                /// <param name="memberMap"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(valueType value, memberMap memberMap, keyType key)
                {
                    this.value = value;
                    this.memberMap = memberMap;
                    this.key = key;
                    Stream.Clear();
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <param name="table"></param>
                /// <returns></returns>
                public static updateWaiter Get(memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table)
                {
                    updateWaiter updater = typePool<updateWaiter>.Pop();
                    if (updater == null)
                    {
                        try
                        {
                            updater = new updateWaiter(table.memoryPool);
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    updater.table = table;
                    return updater;
                }
            }
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="memberMap"></param>
            /// <param name="callbacker"></param>
            private unsafe void update(valueType value, memberMap<modelType> memberMap, callbacker callbacker)
            {
#if NotFastCSharpCode
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                unmanagedStream stream = callbacker.Stream;
                stream.UnsafeAddLength(sizeof(physicalServer.timeIdentity) + sizeof(int) * 2);
                using (memberMap<modelType> updateMemberMap = GetUpdateMemberMap(memberMap)) Serializer.Update(value, stream, updateMemberMap);
                byte* data = stream.data.Byte;
                *(physicalServer.timeIdentity*)data = physicalIdentity;
                *(int*)(data + sizeof(physicalServer.timeIdentity)) = stream.Length - sizeof(physicalServer.timeIdentity);
                *(int*)(data + (sizeof(physicalServer.timeIdentity) + sizeof(int))) = (int)(byte)memoryDatabaseTable.logType.Update;
                client.append(stream, callbacker.Callback);
#endif
            }
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="memberMap"></param>
            public unsafe valueType Update(valueType value, memberMap<modelType> memberMap)
            {
                if (value != null && memberMap != null && !memberMap.IsDefault)
                {
                    Exception exception = null;
                    keyType key = GetPrimaryKey(value);
                    bool isKey = true;
                    Monitor.Enter(cacheLock);
                    try
                    {
                        isKey = cache.ContainsKey(key);
                    }
                    catch (Exception error)
                    {
                        exception = error;
                    }
                    finally
                    {
                        Monitor.Exit(cacheLock);
                        if (exception != null) log.Error.Add(exception, null, false);
                    }
                    if (isKey)
                    {
                        updateWaiter updater = null;
                        try
                        {
                            if ((updater = updateWaiter.Get(this)) != null)
                            {
                                updater.Set(value, memberMap, key);
                                update(value, memberMap, updater);
                                return updater.Value;
                            }
                        }
                        catch (Exception error)
                        {
                            log.Default.Add(error, null, false);
                        }
                        if (updater != null) updater.Cancel();
                    }
                }
                return null;
            }
            /// <summary>
            /// 更新数据
            /// </summary>
            private unsafe sealed class updateCallbacker : callbacker
            {
                /// <summary>
                /// 更新数据
                /// </summary>
                private valueType value;
                /// <summary>
                /// 添加数据回调委托
                /// </summary>
                private Action<valueType> callback;
                /// <summary>
                /// 更新成员位图
                /// </summary>
                private memberMap memberMap;
                /// <summary>
                /// 操作回调是否使用任务模式
                /// </summary>
                private bool isCallbackTask;
                /// <summary>
                /// 添加数据
                /// </summary>
                private updateCallbacker(unmanagedPool memoryPool)
                    : base(memoryPool)
                {
                    Callback = onReturn;
                }
                /// <summary>
                /// 更新数据回调委托
                /// </summary>
                /// <param name="returnValue"></param>
                private void onReturn(fastCSharp.net.returnValue<int> returnValue)
                {
                    if (memoryPool != null && Stream.data.data != buffer.Data)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                    Action<valueType> callback = this.callback;
                    bool isCallbackTask = this.isCallbackTask;
                    this.callback = null;
                    if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table = this.table;
                        this.table = null;
                        if (returnValue.Value == 0)
                        {
                            this.value = null;
                            this.key = default(keyType);
                            memberMap = null;
                            typePool<updateCallbacker>.PushNotNull(this);
                            table.Dispose();
                            if (callback != null)
                            {
                                if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, null);
                                else callback(null);
                            }
                        }
                        else
                        {
                            valueType value = this.value;
                            keyType key = this.key;
                            memberMap memberMap = this.memberMap;
                            Exception exception = null;
                            this.value = null;
                            this.key = default(keyType);
                            this.memberMap = null;
                            typePool<updateCallbacker>.PushNotNull(this);
                            table.appendValue = returnValue.Value;
                            Monitor.Enter(table.cacheLock);
                            try
                            {
#if NOJIT
                                value = (valueType)table.cache.Update(value, key, memberMap);
#else
                                value = table.cache.Update(value, key, memberMap);
#endif
                            }
                            catch (Exception error)
                            {
                                value = null;
                                exception = error;
                            }
                            finally
                            {
                                Monitor.Exit(table.cacheLock);
                                if (exception != null)
                                {
                                    log.Default.Add(exception, null, false);
                                    table.Dispose();
                                }
                            }
                            if (callback != null)
                            {
                                if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, value);
                                else callback(value);
                            }
                        }
                    }
                    else
                    {
                        this.table = null;
                        this.value = null;
                        this.key = default(keyType);
                        memberMap = null;
                        typePool<updateCallbacker>.PushNotNull(this);
                        if (callback != null)
                        {
                            if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, null);
                            else callback(null);
                        }
                    }
                }
                /// <summary>
                /// 取消当前调用
                /// </summary>
                public void Cancel()
                {
                    if (memoryPool != null && Stream.data.data != buffer.Data)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                    Action<valueType> callback = this.callback;
                    bool isCallbackTask = this.isCallbackTask;
                    this.table = null;
                    this.value = null;
                    this.key = default(keyType);
                    memberMap = null;
                    this.callback = null;
                    typePool<updateCallbacker>.PushNotNull(this);
                    if (callback != null)
                    {
                        if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, null);
                        else callback(null);
                    }
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <param name="value"></param>
                /// <param name="key"></param>
                /// <param name="memberMap"></param>
                /// <param name="onUpdated"></param>
                /// <param name="isCallbackTask"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(valueType value, memberMap memberMap, Action<valueType> onUpdated, keyType key, bool isCallbackTask)
                {
                    this.value = value;
                    this.memberMap = memberMap;
                    callback = onUpdated;
                    this.key = key;
                    this.isCallbackTask = isCallbackTask;
                    Stream.Clear();
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <param name="table"></param>
                /// <returns></returns>
                public static updateCallbacker Get(memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table)
                {
                    updateCallbacker updater = typePool<updateCallbacker>.Pop();
                    if (updater == null)
                    {
                        try
                        {
                            updater = new updateCallbacker(table.memoryPool);
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                            return null;
                        }
                    }
                    updater.table = table;
                    return updater;
                }
            }
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="memberMap"></param>
            /// <param name="onUpdated"></param>
            /// <param name="isCallbackTask"></param>
            public unsafe void Update(valueType value, memberMap<modelType> memberMap, Action<valueType> onUpdated, bool isCallbackTask = true)
            {
                if (value != null && memberMap != null && !memberMap.IsDefault)
                {
                    Exception exception = null;
                    keyType key = GetPrimaryKey(value);
                    bool isKey = true;
                    Monitor.Enter(cacheLock);
                    try
                    {
                        isKey = cache.ContainsKey(key);
                    }
                    catch (Exception error)
                    {
                        exception = error;
                    }
                    finally
                    {
                        Monitor.Exit(cacheLock);
                        if (exception != null) log.Error.Add(exception, null, false);
                    }
                    if (isKey)
                    {
                        updateCallbacker updater = null;
                        try
                        {
                            if ((updater = updateCallbacker.Get(this)) != null)
                            {
                                updater.Set(value, memberMap, onUpdated, key, isCallbackTask);
                                update(value, memberMap, updater);
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            log.Default.Add(error, null, false);
                        }
                        if (updater != null)
                        {
                            updater.Cancel();
                            return;
                        }
                    }
                }
                if (onUpdated != null)
                {
                    if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(onUpdated, null);
                    else onUpdated(null);
                }
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            private unsafe sealed class deleteWaiter : waiter
            {
                /// <summary>
                /// 删除数据
                /// </summary>
                private deleteWaiter()
                    : base(fastCSharp.unmanagedPool.TinyBuffers)
                {
                }
                /// <summary>
                /// 删除数据
                /// </summary>
                public valueType Value
                {
                    get
                    {
                        waitHandle.Wait();
                        if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table = this.table;
                            this.table = null;
                            if (returnValue.Value == 0)
                            {
                                this.key = default(keyType);
                                typePool<deleteWaiter>.PushNotNull(this);
                                table.Dispose();
                                return null;
                            }
                            keyType key = this.key;
                            this.key = default(keyType);
                            typePool<deleteWaiter>.PushNotNull(this);
                            Exception exception = null;
                            valueType value = null;
                            int logSize;
                            table.appendValue = returnValue.Value;
                            Monitor.Enter(table.cacheLock);
                            try
                            {
#if NOJIT
                                value = (valueType)table.cache.Delete(key, out logSize);
#else
                                value = table.cache.Delete(key, out logSize);
#endif
                                table.dataSize -= logSize;
                            }
                            catch (Exception error)
                            {
                                value = null;
                                exception = error;
                            }
                            finally
                            {
                                Monitor.Exit(table.cacheLock);
                                if (exception != null)
                                {
                                    log.Default.Add(exception, null, false);
                                    table.Dispose();
                                }
                            }
                            return value;
                        }
                        this.table = null;
                        this.key = default(keyType);
                        typePool<deleteWaiter>.PushNotNull(this);
                        return null;
                    }
                }
                /// <summary>
                /// 取消当前调用
                /// </summary>
                public void Cancel()
                {
                    if (memoryPool != null && Stream.data.data != buffer.Data)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                    this.table = null;
                    this.key = default(keyType);
                    typePool<deleteWaiter>.PushNotNull(this);
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <param name="table"></param>
                /// <param name="key"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table, keyType key)
                {
                    this.table = table;
                    this.key = key;
                    Stream.Clear();
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <returns></returns>
                public static deleteWaiter Get()
                {
                    deleteWaiter deleter = typePool<deleteWaiter>.Pop();
                    if (deleter == null)
                    {
                        try
                        {
                            deleter = new deleteWaiter();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return deleter;
                }
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="value"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Delete(valueType value)
            {
                return value != null ? Delete(GetPrimaryKey(value)) : null;
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="key"></param>
            public unsafe valueType Delete(keyType key)
            {
                Exception exception = null;
                bool isKey = true;
                Monitor.Enter(cacheLock);
                try
                {
                    isKey = cache.ContainsKey(key);
                }
                catch (Exception error)
                {
                    exception = error;
                }
                finally
                {
                    Monitor.Exit(cacheLock);
                    if (exception != null) log.Error.Add(exception, null, false);
                }
                if (isKey)
                {
                    deleteWaiter deleter = null;
                    try
                    {
                        if ((deleter = deleteWaiter.Get()) != null)
                        {
                            deleter.Set(this, key);
                            delete(key, deleter);
                            return deleter.Value;
                        }
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, null, false);
                    }
                    if (deleter != null) deleter.Cancel();
                }
                return null;
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="callbacker"></param>
            private unsafe void delete(keyType key, callbacker callbacker)
            {
#if NotFastCSharpCode
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                unmanagedStream stream = callbacker.Stream;
                stream.UnsafeAddLength(sizeof(physicalServer.timeIdentity) + sizeof(int) * 2);
                Serializer.Delete(key, stream);
                byte* data = stream.data.Byte;
                *(physicalServer.timeIdentity*)data = physicalIdentity;
                *(int*)(data + sizeof(physicalServer.timeIdentity)) = stream.Length - sizeof(physicalServer.timeIdentity);
                *(int*)(data + (sizeof(physicalServer.timeIdentity) + sizeof(int))) = (int)(byte)memoryDatabaseTable.logType.Delete;
                client.append(stream, callbacker.Callback);
#endif
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            private unsafe sealed class deleteCallbacker : callbacker
            {
                /// <summary>
                /// 添加数据回调委托
                /// </summary>
                private Action<valueType> callback;
                /// <summary>
                /// 操作回调是否使用任务模式
                /// </summary>
                private bool isCallbackTask;
                /// <summary>
                /// 删除数据
                /// </summary>
                private deleteCallbacker()
                    : base(fastCSharp.unmanagedPool.TinyBuffers)
                {
                    Callback = onReturn;
                }
                /// <summary>
                /// 删除数据回调委托
                /// </summary>
                /// <param name="returnValue"></param>
                private void onReturn(fastCSharp.net.returnValue<int> returnValue)
                {
                    if (memoryPool != null && Stream.data.data != buffer.Data)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                    Action<valueType> callback = this.callback;
                    bool isCallbackTask = this.isCallbackTask;
                    this.callback = null;
                    if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table = this.table;
                        this.table = null;
                        if (returnValue.Value == 0)
                        {
                            this.key = default(keyType);
                            typePool<deleteCallbacker>.PushNotNull(this);
                            table.Dispose();
                            if (callback != null)
                            {
                                if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, null);
                                else callback(null);
                            }
                        }
                        else
                        {
                            keyType key = this.key;
                            this.key = default(keyType);
                            typePool<deleteCallbacker>.PushNotNull(this);
                            Exception exception = null;
                            valueType value = null;
                            int logSize;
                            table.appendValue = returnValue.Value;
                            Monitor.Enter(table.cacheLock);
                            try
                            {
#if NOJIT
                                value = (valueType)table.cache.Delete(key, out logSize);
#else
                                value = table.cache.Delete(key, out logSize);
#endif
                                table.dataSize -= logSize;
                            }
                            catch (Exception error)
                            {
                                value = null;
                                exception = error;
                            }
                            finally
                            {
                                Monitor.Exit(table.cacheLock);
                                if (exception != null)
                                {
                                    log.Default.Add(exception, null, false);
                                    table.Dispose();
                                }
                            }
                            if (callback != null)
                            {
                                if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, value);
                                else callback(value);
                            }
                        }
                    }
                    else
                    {
                        this.table = null;
                        this.key = default(keyType);
                        typePool<deleteCallbacker>.PushNotNull(this);
                        if (callback != null)
                        {
                            if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, null);
                            else callback(null);
                        }
                    }
                }
                /// <summary>
                /// 取消当前调用
                /// </summary>
                public void Cancel()
                {
                    if (memoryPool != null && Stream.data.data != buffer.Data)
                    {
                        memoryPool.Push(ref buffer);
                        memoryPool = null;
                    }
                    Action<valueType> callback = this.callback;
                    bool isCallbackTask = this.isCallbackTask;
                    this.table = null;
                    this.key = default(keyType);
                    this.callback = null;
                    typePool<deleteCallbacker>.PushNotNull(this);
                    if (callback != null)
                    {
                        if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(callback, null);
                        else callback(null);
                    }
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <param name="table"></param>
                /// <param name="key"></param>
                /// <param name="onDeleted"></param>
                /// <param name="isCallbackTask"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(memoryDatabaseTable.remoteTable<valueType, modelType, keyType> table, Action<valueType> onDeleted, keyType key, bool isCallbackTask)
                {
                    this.table = table;
                    callback = onDeleted;
                    this.key = key;
                    this.isCallbackTask = isCallbackTask;
                    Stream.Clear();
                }
                /// <summary>
                /// 获取添加数据
                /// </summary>
                /// <returns></returns>
                public static deleteCallbacker Get()
                {
                    deleteCallbacker deleter = typePool<deleteCallbacker>.Pop();
                    if (deleter == null)
                    {
                        try
                        {
                            deleter = new deleteCallbacker();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return deleter;
                }
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="onDeleted"></param>
            /// <param name="isCallbackTask"></param>
            public void Delete(valueType value, Action<valueType> onDeleted, bool isCallbackTask = true)
            {
                if (value != null)
                {
                    Delete(GetPrimaryKey(value), onDeleted, isCallbackTask);
                }
                else if (onDeleted != null)
                {
                    if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(onDeleted, null);
                    else onDeleted(null);
                }
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="onDeleted"></param>
            /// <param name="isCallbackTask"></param>
            public unsafe void Delete(keyType key, Action<valueType> onDeleted, bool isCallbackTask = true)
            {
                Exception exception = null;
                bool isKey = true;
                Monitor.Enter(cacheLock);
                try
                {
                    isKey = cache.ContainsKey(key);
                }
                catch (Exception error)
                {
                    exception = error;
                }
                finally 
                {
                    Monitor.Exit(cacheLock);
                    if (exception != null) log.Error.Add(exception, null, false);
                }
                if (isKey)
                {
                    deleteCallbacker deleter = null;
                    try
                    {
                        if ((deleter = deleteCallbacker.Get()) != null)
                        {
                            deleter.Set(this, onDeleted, key, isCallbackTask);
                            delete(key, deleter);
                            return;
                        }
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, null, false);
                    }
                    if (deleter != null)
                    {
                        deleter.Cancel();
                        return;
                    }
                }
                if (onDeleted != null)
                {
                    if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(onDeleted, null);
                    else onDeleted(null);
                }
            }
            /// <summary>
            /// 刷新写入文件缓存区
            /// </summary>
            /// <param name="isWriteFile">是否写入文件</param>
            /// <returns>是否操作成功</returns>
            public bool Flush(bool isWriteFile)
            {
#if NotFastCSharpCode
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                try
                {
                    if (client.flush(physicalIdentity) && client.flushFile(physicalIdentity, isWriteFile))
                    {
                        return true;
                    }
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
                Dispose();
#endif
                return false;
            }
        }
    }
    /// <summary>
    /// 内存数据库表格操作工具
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    public class memoryDatabaseTable<valueType, modelType> : memoryDatabaseTable.localTable<valueType, modelType, int>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 自增数据加载基本缓存接口
        /// </summary>
#if NOJIT
        private new fastCSharp.memoryDatabase.cache.ILoadIdentityCache cache;
#else
        private new fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, modelType> cache;
#endif
        /// <summary>
        /// 自增字段成员索引
        /// </summary>
        private int identityMemberIndex;
        /// <summary>
        /// 内存数据库表格操作工具
        /// </summary>
        /// <param name="cache">自增数据加载基本缓存接口</param>
        /// <param name="serializeType">数据序类型</param>
        /// <param name="name">表格名称</param>
        /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
        /// <param name="bufferSize">缓冲区字节大小</param>
#if NOJIT
        public memoryDatabaseTable(fastCSharp.memoryDatabase.cache.ILoadIdentityCache cache
#else
        public memoryDatabaseTable(fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, modelType> cache
#endif
            , memoryDatabaseTable.serializeType serializeType = memoryDatabaseTable.serializeType.Index
            , string name = null, int minRefreshSize = 0, int bufferSize = 0)
            : base(cache, serializeType, name, minRefreshSize, bufferSize, memoryDatabaseModel<modelType>.GetIdentity, memoryDatabaseModel<modelType>.SetIdentity)
        {
            this.cache = cache;
            identityMemberIndex = memoryDatabaseModel<modelType>.Identity.MemberMapIndex;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isCopy">是否复制缓存数据</param>
        /// <returns></returns>
        public unsafe override valueType Insert(valueType value, bool isCopy = true)
        {
            if (value != null && GetPrimaryKey(value) == 0)
            {
                Exception exception = null;
                int isError = 0;
                Monitor.Enter(physicalLock);
                unmanagedStream stream = Interlocked.Exchange(ref this.stream, null);
                try
                {
                    int identity = cache.NextIdentity();
                    SetPrimaryKey(value, identity);
                    byte[] buffer = Physical.LocalBuffer();
                    if (buffer == null)
                    {
                        value = null;
                        isError = 1;
                    }
                    else
                    {
                        fixed (byte* bufferFixed = buffer)
                        {
                            if (stream == null) stream = new unmanagedStream(bufferFixed + Physical.BufferIndex, buffer.Length - Physical.BufferIndex);
                            else stream.UnsafeReset(bufferFixed + Physical.BufferIndex, buffer.Length - Physical.BufferIndex);
                            stream.UnsafeAddLength(sizeof(int) * 2);
                            Serializer.Insert(value, stream);
                            byte* data = stream.data.Byte;
                            *(int*)(data + sizeof(int)) = (int)(byte)memoryDatabaseTable.logType.Insert;
                            *(int*)data = stream.Length;
                            isError = 1;
                            if (data == bufferFixed + Physical.BufferIndex)
                            {
                                Physical.BufferIndex += stream.Length;
#if NOJIT
                                if ((value = (valueType)cache.Insert(value, identity, stream.Length, isCopy)) != null)
#else
                                if ((value = cache.Insert(value, identity, stream.Length, isCopy)) != null)
#endif
                                {
                                    dataSize += stream.Length;
                                    isError = 0;
                                }
                            }
                            else if (Physical.Append(stream.data.Byte, stream.Length) == 0) value = null;
#if NOJIT
                            else if ((value = (valueType)cache.Insert(value, identity, stream.Length, isCopy)) != null)
#else
                            else if ((value = cache.Insert(value, identity, stream.Length, isCopy)) != null)
#endif
                            {
                                dataSize += stream.Length;
                                isError = 0;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    value = null;
                    exception = error;
                }
                finally
                {
                    if (isDisposed == 0)
                    {
                        if (stream != null) Interlocked.Exchange(ref this.stream, stream);
                    }
                    else fastCSharp.pub.Dispose(ref stream);
                    Monitor.Exit(physicalLock);
                    if (exception != null) log.Error.Add(exception, null, false);
                }
                if (value != null)
                {
                    Physical.WaitBuffer();
                    return value;
                }
                if (isError != 0) Dispose();
            }
            return null;
        }
        /// <summary>
        /// 获取更新成员位图
        /// </summary>
        /// <param name="memberMap"></param>
        /// <returns></returns>
        protected override memberMap<modelType> GetUpdateMemberMap(memberMap<modelType> memberMap)
        {
            memberMap.ClearMember(identityMemberIndex);
            memberMap<modelType> updateMemberMap = memberMap.Copy();
            updateMemberMap.SetMember(identityMemberIndex);
            return updateMemberMap;
        }
        /// <summary>
        /// 内存数据库表格操作工具(远程)
        /// </summary>
        public sealed class remote : memoryDatabaseTable.remoteTable<valueType, modelType, int>
        {
            /// <summary>
            /// 自增数据加载基本缓存接口
            /// </summary>
#if NOJIT
            private new fastCSharp.memoryDatabase.cache.ILoadIdentityCache cache;
#else
            private new fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, modelType> cache;
#endif
            /// <summary>
            /// 自增字段成员索引
            /// </summary>
            private int identityMemberIndex;
#if NotFastCSharpCode
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="cache">自增数据加载基本缓存接口</param>
            /// <param name="serializeType">数据序类型</param>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            /// <param name="bufferSize">缓冲区字节大小</param>
            /// <param name="memoryPool">数据缓冲区内存池</param>
            /// <param name="isWaitClose">是否等待数据库关闭</param>
            /// <param name="isCloseCient">是否自动关系内存数据库客户端</param>
            public remote(fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, modelType> cache
                , memoryDatabaseTable.serializeType serializeType = memoryDatabaseTable.serializeType.Index
                , string name = null, int minRefreshSize = 0, int bufferSize = 0, unmanagedPool memoryPool = null, bool isWaitClose = false, bool isCloseCient = true)
                : base(cache, serializeType, name, minRefreshSize, bufferSize, memoryDatabaseModel<modelType>.GetIdentity, memoryDatabaseModel<modelType>.SetIdentity, memoryPool, isWaitClose, isCloseCient)
            {
                fastCSharp.log.Error.Throw(log.exceptionType.NotFastCSharpCode);
            }
#else
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="client"></param>
            /// <param name="cache">自增数据加载基本缓存接口</param>
            /// <param name="serializeType">数据序类型</param>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            /// <param name="bufferSize">缓冲区字节大小</param>
            /// <param name="memoryPool">数据缓冲区内存池</param>
            /// <param name="isWaitClose">是否等待数据库关闭</param>
            /// <param name="isCloseCient">是否自动关系内存数据库客户端</param>
#if NOJIT
            public remote(fastCSharp.memoryDatabase.physicalServer.tcpClient client, fastCSharp.memoryDatabase.cache.ILoadIdentityCache cache
#else
            public remote(fastCSharp.memoryDatabase.physicalServer.tcpClient client, fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, modelType> cache
#endif
                , memoryDatabaseTable.serializeType serializeType = memoryDatabaseTable.serializeType.Index
                , string name = null, int minRefreshSize = 0, int bufferSize = 0, unmanagedPool memoryPool = null, bool isWaitClose = false, bool isCloseCient = true)
                : base(client, cache, serializeType, name, minRefreshSize, bufferSize, memoryDatabaseModel<modelType>.GetIdentity, memoryDatabaseModel<modelType>.SetIdentity, memoryPool, isWaitClose, isCloseCient)
            {
                this.cache = cache;
                identityMemberIndex = memoryDatabaseModel<modelType>.Identity.MemberMapIndex;
            }
#endif
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="onInserted"></param>
            /// <param name="isCallbackTask">添加数据回调是否使用任务模式</param>
            /// <param name="isCopy">是否复制缓存数据</param>
            /// <returns></returns>
            public unsafe override void Insert(valueType value, Action<valueType> onInserted, bool isCallbackTask = true, bool isCopy = true)
            {
                if (value != null && GetPrimaryKey(value) == 0)
                {
                    Monitor.Enter(cacheLock);
                    int identity = cache.NextIdentity();
                    Monitor.Exit(cacheLock);
                    SetPrimaryKey(value, identity);
                    if (insert(value, identity, onInserted, isCallbackTask, isCopy)) return;
                }
                if (onInserted != null)
                {
                    if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(onInserted, null);
                    else onInserted(null);
                }
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="isCopy">是否复制缓存数据</param>
            /// <returns></returns>
            public override valueType Insert(valueType value, bool isCopy = true)
            {
                if (value != null && GetPrimaryKey(value) == 0)
                {
                    Monitor.Enter(cacheLock);
                    int identity = cache.NextIdentity();
                    Monitor.Exit(cacheLock);
                    SetPrimaryKey(value, identity);
                    return insert(value, identity, isCopy);
                }
                return null;
            }
            /// <summary>
            /// 获取更新成员位图
            /// </summary>
            /// <param name="memberMap"></param>
            /// <returns></returns>
            protected override memberMap<modelType> GetUpdateMemberMap(memberMap<modelType> memberMap)
            {
                memberMap.ClearMember(identityMemberIndex);
                memberMap<modelType> updateMemberMap = memberMap.Copy();
                updateMemberMap.SetMember(identityMemberIndex);
                return updateMemberMap;
            }
        }
    }
    /// <summary>
    /// 内存数据库表格操作工具
    /// </summary>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    public sealed class memoryDatabaseModelTable<modelType> : memoryDatabaseTable<modelType, modelType>
        where modelType : class
    {
        /// <summary>
        /// 内存数据库表格操作工具
        /// </summary>
        /// <param name="cache">自增数据加载基本缓存接口</param>
        /// <param name="serializeType">数据序类型</param>
        /// <param name="name">表格名称</param>
        /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
        /// <param name="bufferSize">缓冲区字节大小</param>
#if NOJIT
        public memoryDatabaseModelTable(fastCSharp.memoryDatabase.cache.ILoadIdentityCache cache
#else
        public memoryDatabaseModelTable(fastCSharp.memoryDatabase.cache.ILoadIdentityCache<modelType, modelType> cache
#endif
            , memoryDatabaseTable.serializeType serializeType = memoryDatabaseTable.serializeType.Index
            , string name = null, int minRefreshSize = 0, int bufferSize = 0)
            : base(cache, serializeType, name, minRefreshSize, bufferSize) { }
    }
    /// <summary>
    /// 内存数据库表格操作工具
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public class memoryDatabaseTable<valueType, modelType, keyType> : memoryDatabaseTable.localTable<valueType, modelType, keyType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 内存数据库表格操作工具
        /// </summary>
        /// <param name="cache">自增数据加载基本缓存接口</param>
        /// <param name="serializeType">数据序类型</param>
        /// <param name="name">表格名称</param>
        /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
        /// <param name="bufferSize">缓冲区字节大小</param>
#if NOJIT
        public memoryDatabaseTable(fastCSharp.memoryDatabase.cache.ILoadCacheKey cache
#else
        public memoryDatabaseTable(fastCSharp.memoryDatabase.cache.ILoadCache<valueType, modelType, keyType> cache
#endif
            , memoryDatabaseTable.serializeType serializeType = memoryDatabaseTable.serializeType.Index
            , string name = null, int minRefreshSize = 0, int bufferSize = 0)
            : base(cache, serializeType, name, minRefreshSize, bufferSize
            , databaseModel<modelType>.GetPrimaryKeyGetter<keyType>("GetMemoryDatabasePrimaryKey", memoryDatabaseModel<modelType>.PrimaryKeyFields)
            , databaseModel<modelType>.GetPrimaryKeySetter<keyType>("SetMemoryDatabasePrimaryKey", memoryDatabaseModel<modelType>.PrimaryKeyFields))
        {
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isCopy">是否复制缓存数据</param>
        /// <returns></returns>
        public unsafe override valueType Insert(valueType value, bool isCopy = true)
        {
            if (value != null)
            {
                Exception exception = null;
                unmanagedStream stream = null;
                int isError = 0;
                keyType key = GetPrimaryKey(value);
                Monitor.Enter(physicalLock);
                try
                {
                    if (cache.ContainsKey(key)) value = null;
                    else
                    {
                        stream = Interlocked.Exchange(ref this.stream, null);
                        byte[] buffer = Physical.LocalBuffer();
                        if (buffer == null)
                        {
                            value = null;
                            isError = 1;
                        }
                        else
                        {
                            fixed (byte* bufferFixed = buffer)
                            {
                                if (stream == null) stream = new unmanagedStream(bufferFixed + Physical.BufferIndex, buffer.Length - Physical.BufferIndex);
                                else stream.UnsafeReset(bufferFixed + Physical.BufferIndex, buffer.Length - Physical.BufferIndex);
                                stream.UnsafeAddLength(sizeof(int) * 2);
                                Serializer.Insert(value, stream);
                                byte* data = stream.data.Byte;
                                *(int*)(data + sizeof(int)) = (int)(byte)memoryDatabaseTable.logType.Insert;
                                *(int*)data = stream.Length;
                                isError = 1;
                                if (data == bufferFixed + Physical.BufferIndex)
                                {
                                    Physical.BufferIndex += stream.Length;
#if NOJIT
                                    if ((value = (valueType)cache.Insert(value, key, stream.Length, isCopy)) != null)
#else
                                    if ((value = cache.Insert(value, key, stream.Length, isCopy)) != null)
#endif
                                    {
                                        dataSize += stream.Length;
                                        isError = 0;
                                    }
                                }
                                else if (Physical.Append(stream.data.Byte, stream.Length) == 0) value = null;
#if NOJIT
                                else if ((value = (valueType)cache.Insert(value, key, stream.Length, isCopy)) != null)
#else
                                else if ((value = cache.Insert(value, key, stream.Length, isCopy)) != null)
#endif
                                {
                                    dataSize += stream.Length;
                                    isError = 0;
                                }
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    value = null;
                    exception = error;
                }
                finally
                {
                    if (isDisposed == 0)
                    {
                        if (stream != null) Interlocked.Exchange(ref this.stream, stream);
                    }
                    else fastCSharp.pub.Dispose(ref stream);
                    Monitor.Exit(physicalLock);
                    if (exception != null) log.Error.Add(exception, null, false);
                }
                if (value != null)
                {
                    Physical.WaitBuffer();
                    return value;
                }
                if (isError != 0) Dispose();
            }
            return null;
        }
        /// <summary>
        /// 获取更新成员位图
        /// </summary>
        /// <param name="memberMap"></param>
        /// <returns></returns>
        protected override memberMap<modelType> GetUpdateMemberMap(memberMap<modelType> memberMap)
        {
            memberMap<modelType> updateMemberMap = memberMap.Copy();
            foreach (fastCSharp.code.cSharp.memoryDatabaseModel.fieldInfo primaryKey in memoryDatabaseModel<modelType>.PrimaryKeys)
            {
                memberMap.ClearMember(primaryKey.MemberMapIndex);
                updateMemberMap.SetMember(primaryKey.MemberMapIndex);
            }
            return updateMemberMap;
        }
        /// <summary>
        /// 内存数据库表格操作工具(远程)
        /// </summary>
        public sealed class remote : memoryDatabaseTable.remoteTable<valueType, modelType, keyType>
        {
#if NotFastCSharpCode
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="cache">自增数据加载基本缓存接口</param>
            /// <param name="serializeType">数据序类型</param>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            /// <param name="bufferSize">缓冲区字节大小</param>
            /// <param name="memoryPool">数据缓冲区内存池</param>
            /// <param name="isWaitClose">是否等待数据库关闭</param>
            /// <param name="isCloseCient">是否自动关系内存数据库客户端</param>
            public remote(fastCSharp.memoryDatabase.cache.ILoadCache<valueType, modelType, keyType> cache
                , memoryDatabaseTable.serializeType serializeType = memoryDatabaseTable.serializeType.Index
                , string name = null, int minRefreshSize = 0, int bufferSize = 0, unmanagedPool memoryPool = null, bool isWaitClose = false, bool isCloseCient = true)
                : base(cache, serializeType, name, minRefreshSize, bufferSize
                , databaseModel<modelType>.GetPrimaryKeyGetter<keyType>("GetMemoryDatabasePrimaryKey", memoryDatabaseModel<modelType>.PrimaryKeyFields)
                , databaseModel<modelType>.GetPrimaryKeySetter<keyType>("SetMemoryDatabasePrimaryKey", memoryDatabaseModel<modelType>.PrimaryKeyFields)
                , memoryPool, isWaitClose, isCloseCient)
            {
            }
#else
            /// <summary>
            /// 内存数据库表格操作工具
            /// </summary>
            /// <param name="client"></param>
            /// <param name="cache">自增数据加载基本缓存接口</param>
            /// <param name="serializeType">数据序类型</param>
            /// <param name="name">表格名称</param>
            /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
            /// <param name="bufferSize">缓冲区字节大小</param>
            /// <param name="memoryPool">数据缓冲区内存池</param>
            /// <param name="isWaitClose">是否等待数据库关闭</param>
            /// <param name="isCloseCient">是否自动关系内存数据库客户端</param>
#if NOJIT
            public remote(fastCSharp.memoryDatabase.physicalServer.tcpClient client, fastCSharp.memoryDatabase.cache.ILoadCacheKey cache
#else
            public remote(fastCSharp.memoryDatabase.physicalServer.tcpClient client, fastCSharp.memoryDatabase.cache.ILoadCache<valueType, modelType, keyType> cache
#endif
                , memoryDatabaseTable.serializeType serializeType = memoryDatabaseTable.serializeType.Index
                , string name = null, int minRefreshSize = 0, int bufferSize = 0, unmanagedPool memoryPool = null, bool isWaitClose = false, bool isCloseCient = true)
                : base(client, cache, serializeType, name, minRefreshSize, bufferSize
                , databaseModel<modelType>.GetPrimaryKeyGetter<keyType>("GetMemoryDatabasePrimaryKey", memoryDatabaseModel<modelType>.PrimaryKeyFields)
                , databaseModel<modelType>.GetPrimaryKeySetter<keyType>("SetMemoryDatabasePrimaryKey", memoryDatabaseModel<modelType>.PrimaryKeyFields)
                , memoryPool, isWaitClose, isCloseCient)
            {
            }
#endif
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="onInserted"></param>
            /// <param name="isCallbackTask">添加数据回调是否使用任务模式</param>
            /// <param name="isCopy">是否复制缓存数据</param>
            /// <returns></returns>
            public unsafe override void Insert(valueType value, Action<valueType> onInserted, bool isCallbackTask = true, bool isCopy = true)
            {
                if (value != null)
                {
                    Exception exception = null;
                    keyType key = GetPrimaryKey(value);
                    bool isKey = true;
                    Monitor.Enter(cacheLock);
                    try
                    {
                        isKey = cache.ContainsKey(key);
                    }
                    catch (Exception error)
                    {
                        exception = error;
                    }
                    finally
                    {
                        Monitor.Exit(cacheLock);
                        if (exception != null) log.Error.Add(exception, null, false);
                    }
                    if (!isKey && insert(value, key, onInserted, isCallbackTask, isCopy)) return;
                }
                if (onInserted != null)
                {
                    if (isCallbackTask) fastCSharp.threading.task.Tiny.Add(onInserted, null);
                    else onInserted(null);
                }
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="isCopy">是否复制缓存数据</param>
            /// <returns></returns>
            public override valueType Insert(valueType value, bool isCopy = true)
            {
                if (value != null)
                {
                    Exception exception = null;
                    keyType key = GetPrimaryKey(value);
                    bool isKey = true;
                    Monitor.Enter(cacheLock);
                    try
                    {
                        isKey = cache.ContainsKey(key);
                    }
                    catch (Exception error)
                    {
                        exception = error;
                    }
                    finally 
                    {
                        Monitor.Exit(cacheLock);
                        if (exception != null) log.Error.Add(exception, null, false);
                    }
                    if (!isKey) return insert(value, key, isCopy);
                }
                return null;
            }
            /// <summary>
            /// 获取更新成员位图
            /// </summary>
            /// <param name="memberMap"></param>
            /// <returns></returns>
            protected override memberMap<modelType> GetUpdateMemberMap(memberMap<modelType> memberMap)
            {
                memberMap<modelType> updateMemberMap = memberMap.Copy();
                foreach (fastCSharp.code.cSharp.memoryDatabaseModel.fieldInfo primaryKey in memoryDatabaseModel<modelType>.PrimaryKeys)
                {
                    memberMap.ClearMember(primaryKey.MemberMapIndex);
                    updateMemberMap.SetMember(primaryKey.MemberMapIndex);
                }
                return updateMemberMap;
            }
        }
    }
    /// <summary>
    /// 内存数据库表格操作工具
    /// </summary>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public sealed class memoryDatabaseModelTable<modelType, keyType> : memoryDatabaseTable<modelType, modelType, keyType>
        where modelType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 内存数据库表格操作工具
        /// </summary>
        /// <param name="cache">自增数据加载基本缓存接口</param>
        /// <param name="serializeType">数据序类型</param>
        /// <param name="name">表格名称</param>
        /// <param name="minRefreshSize">数据库日志文件最小刷新尺寸(单位:KB)</param>
        /// <param name="bufferSize">缓冲区字节大小</param>
#if NOJIT
        public memoryDatabaseModelTable(fastCSharp.memoryDatabase.cache.ILoadCacheKey cache
#else
        public memoryDatabaseModelTable(fastCSharp.memoryDatabase.cache.ILoadCache<modelType, modelType, keyType> cache
#endif
            , memoryDatabaseTable.serializeType serializeType = memoryDatabaseTable.serializeType.Index
            , string name = null, int minRefreshSize = 0, int bufferSize = 0)
            : base(cache, serializeType, name, minRefreshSize, bufferSize)
        {
        }
    }
}
