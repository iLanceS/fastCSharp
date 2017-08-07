using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using fastCSharp.threading;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace fastCSharp.code
{
    /// <summary>
    /// 成员位图
    /// </summary>
    public unsafe abstract class memberMap : IDisposable
    {
        /// <summary>
        /// 成员位图类型信息
        /// </summary>
        internal sealed class type
        {
            /// <summary>
            /// 类型
            /// </summary>
            internal Type Type;
            /// <summary>
            /// 成员位图内存池
            /// </summary>
            internal pool Pool;
            /// <summary>
            /// 名称索引查找数据
            /// </summary>
            private pointer.size nameIndexSearcher;
            /// <summary>
            /// 名称索引查找数据
            /// </summary>
            public pointer.size NameIndexSearcher { get { return nameIndexSearcher; } }
            /// <summary>
            /// 成员数量
            /// </summary>
            public int MemberCount { get; private set; }
            /// <summary>
            /// 字段成员数量
            /// </summary>
            public int FieldCount { get; private set; }
            /// <summary>
            /// 成员位图字节数量
            /// </summary>
            public int MemberMapSize { get; private set; }
            /// <summary>
            /// 字段成员位图序列化字节数量
            /// </summary>
            public int FieldSerializeSize { get; private set; }
            /// <summary>
            /// 成员位图类型信息
            /// </summary>
            /// <param name="type">类型</param>
            /// <param name="members">成员索引集合</param>
            /// <param name="fieldCount">字段成员数量</param>
            public type(Type type, fastCSharp.code.memberIndex[] members, int fieldCount)
            {
                Type = type;
                FieldCount = fieldCount;
                if ((MemberCount = members.Length) < 64) MemberMapSize = MemberCount < 32 ? 4 : 8;
                else MemberMapSize = ((MemberCount + 63) >> 6) << 3;
                nameIndexSearcher = fastCSharp.stateSearcher.charsSearcher.Create(members.getArray(value => value.Member.Name), true);
                if (MemberCount >= 64)
                {
                    Pool = pool.GetPool(MemberMapSize);
                    FieldSerializeSize = ((fieldCount + 31) >> 5) << 2;
                }
            }
            /// <summary>
            /// 获取成员索引
            /// </summary>
            /// <param name="name">成员名称</param>
            /// <returns>成员索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int GetMemberIndex(string name)
            {
                return name != null ? new stateSearcher.charsSearcher(ref nameIndexSearcher).Search(name) : -1;
            }
        }
        /// <summary>
        /// 成员索引
        /// </summary>
        public sealed class memberIndex
        {
            /// <summary>
            /// 成员索引
            /// </summary>
            private int index;
            /// <summary>
            /// 成员索引
            /// </summary>
            /// <param name="index">成员索引</param>
            internal memberIndex(int index)
            {
                this.index = index;
            }
            /// <summary>
            /// 判断成员索引是否有效
            /// </summary>
            /// <param name="memberMap"></param>
            /// <returns>成员索引是否有效</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool IsMember(memberMap memberMap)
            {
                return memberMap != null && memberMap.IsMember(index);
            }
            /// <summary>
            /// 清除成员索引,忽略默认成员
            /// </summary>
            /// <param name="memberMap"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void ClearMember(memberMap memberMap)
            {
                if (memberMap != null) memberMap.ClearMember(index);
            }
            /// <summary>
            /// 设置成员索引,忽略默认成员
            /// </summary>
            /// <param name="memberMap"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void SetMember(memberMap memberMap)
            {
                if (memberMap != null) memberMap.SetMember(index);
            }
        }
        /// <summary>
        /// 成员位图内存池
        /// </summary>
        internal sealed class pool
        {
            /// <summary>
            /// 空闲内存地址
            /// </summary>
            private byte* free;
            /// <summary>
            /// 成员位图字节数量
            /// </summary>
            private int size;
            /// <summary>
            /// 空闲内存地址访问锁
            /// </summary>
            private int freeLock;
            /// <summary>
            /// 成员位图内存池
            /// </summary>
            /// <param name="size">成员位图字节数量</param>
            private pool(int size)
            {
                this.size = size;
            }
            /// <summary>
            /// 获取成员位图
            /// </summary>
            /// <returns>成员位图</returns>
            public byte* Get()
            {
                byte* value;
                interlocked.CompareSetYield(ref freeLock);
                if (free != null)
                {
                    value = free;
                    //free = (byte*)*(ulong*)free;
                    free = *(byte**)free;
                    freeLock = 0;
                    return value;
                }
                freeLock = 0;
                interlocked.CompareSetYield(ref memoryLock);
                value = memoryStart;
                if ((memoryStart += size) <= memoryEnd)
                {
                    memoryLock = 0;
                    return value;
                }
                memoryStart -= size;
                memoryLock = 0;
                Monitor.Enter(createLock);
                interlocked.CompareSetYield(ref memoryLock);
                if ((memoryStart += size) <= memoryEnd)
                {
                    value = memoryStart - size;
                    memoryLock = 0;
                    Monitor.Exit(createLock);
                    return value;
                }
                memoryLock = 0;
                try
                {
                    value = unmanaged.GetStatic(memorySize, false).Byte;
                    interlocked.CompareSetYield(ref memoryLock);
                    memoryStart = value + size;
                    memoryEnd = value + memorySize;
                    memoryLock = 0;
                }
                finally { Monitor.Exit(createLock); }
                Interlocked.Increment(ref memoryCount);
                return value;
            }
            /// <summary>
            /// 获取成员位图
            /// </summary>
            /// <returns>成员位图</returns>
            public byte* GetClear()
            {
                byte* value = Get(), write = value + size;
                do
                {
                    *(ulong*)(write -= sizeof(ulong)) = 0;
                }
                while (write != value);
                return value;
            }
            /// <summary>
            /// 成员位图入池
            /// </summary>
            /// <param name="map">成员位图</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Push(byte* map)
            {
                //*(ulong*)map = (ulong)free;
                *(byte**)map = free;
                free = map;
            }
            /// <summary>
            /// 获取成员位图内存池
            /// </summary>
            /// <param name="size">成员位图字节数量</param>
            /// <returns></returns>
            public static pool GetPool(int size)
            {
                int index = size >> 3;
                if (index < pools.Length)
                {
                    pool pool = pools[index];
                    if (pool != null) return pool;
                    interlocked.CompareSetYield(ref poolLock);
                    if ((pool = pools[index]) == null)
                    {
                        try
                        {
                            pools[index] = pool = new pool(size);
                        }
                        finally { poolLock = 0; }
                        return pool;
                    }
                    poolLock = 0;
                    return pool;
                }
                return null;
            }
            /// <summary>
            /// 成员位图内存池集合
            /// </summary>
            private static readonly pool[] pools;
            /// <summary>
            /// 成员位图内存池集合访问锁
            /// </summary>
            private static int poolLock;
            /// <summary>
            /// 内存申请数量
            /// </summary>
            private static int memoryCount;
            /// <summary>
            /// 成员位图内存池字节大小
            /// </summary>
            private static readonly int memorySize = fastCSharp.config.appSetting.MemberMapPoolSize;
            /// <summary>
            /// 成员位图内存池起始位置
            /// </summary>
            private static byte* memoryStart;
            /// <summary>
            /// 成员位图内存池结束位置
            /// </summary>
            private static byte* memoryEnd;
            /// <summary>
            /// 成员位图内存池访问锁
            /// </summary>
            private static int memoryLock;
            /// <summary>
            /// 成员位图内存池访问锁
            /// </summary>
            private static readonly object createLock = new object();
            static pool()
            {
                int count = fastCSharp.config.appSetting.MaxMemberMapCount;
                if ((count >> 3) >= fastCSharp.config.appSetting.MemberMapPoolSize) log.Error.Add("成员位图支持数量过大 " + count.toString(), new System.Diagnostics.StackFrame(), false);
                pools = new pool[count >> 6];
            }
        }
        /// <summary>
        /// 成员位图类型信息
        /// </summary>
        internal type Type { get; private set; }
        /// <summary>
        /// 是否默认全部成员有效
        /// </summary>
        public abstract bool IsDefault { get; }
        /// <summary>
        /// 是否存在成员
        /// </summary>
        public abstract bool IsAnyMember { get; }
        /// <summary>
        /// 成员位图
        /// </summary>
        /// <param name="type">成员位图类型信息</param>
        internal memberMap(type type)
        {
            Type = type;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose() { }
        ///// <summary>
        ///// 比较是否相等
        ///// </summary>
        ///// <param name="other"></param>
        ///// <returns></returns>
        //public abstract bool Equals(memberMap other);
        /// <summary>
        /// 添加所有成员
        /// </summary>
        internal abstract void Full();
        /// <summary>
        /// 清空所有成员
        /// </summary>
        internal abstract void Empty();
        /// <summary>
        /// 字段成员序列化
        /// </summary>
        /// <param name="stream"></param>
        internal abstract void FieldSerialize(unmanagedStream stream);
        /// <summary>
        /// 字段成员反序列化
        /// </summary>
        /// <param name="deSerializer"></param>
        internal abstract void FieldDeSerialize(fastCSharp.emit.binaryDeSerializer deSerializer);
        /// <summary>
        /// 设置成员索引,忽略默认成员
        /// </summary>
        /// <param name="memberIndex">成员索引</param>
        internal abstract void SetMember(int memberIndex);
        /// <summary>
        /// 设置成员索引,忽略默认成员
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal bool SetMember(string memberName)
        {
            int index = Type.GetMemberIndex(memberName);
            if (index >= 0)
            {
                SetMember(index);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置成员索引,忽略默认成员
        /// </summary>
        /// <param name="member">成员</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal bool SetMember(LambdaExpression member)
        {
            return SetMember(GetMemberName(member));
        }
        /// <summary>
        /// 清除所有成员
        /// </summary>
        internal abstract void Clear();
        /// <summary>
        /// 清除成员索引,忽略默认成员
        /// </summary>
        /// <param name="memberIndex">成员索引</param>
        internal abstract void ClearMember(int memberIndex);
        /// <summary>
        /// 清除成员索引,忽略默认成员
        /// </summary>
        /// <param name="memberName">成员名称</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void ClearMember(string memberName)
        {
            int index = Type.GetMemberIndex(memberName);
            if (index >= 0) ClearMember(index);
        }
        /// <summary>
        /// 清除成员索引,忽略默认成员
        /// </summary>
        /// <param name="member">成员</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void ClearMember(LambdaExpression member)
        {
            ClearMember(GetMemberName(member));
        }
        /// <summary>
        /// 判断成员索引是否有效
        /// </summary>
        /// <param name="memberIndex">成员索引</param>
        /// <returns>成员索引是否有效</returns>
        internal abstract bool IsMember(int memberIndex);
        /// <summary>
        /// 判断成员索引是否有效
        /// </summary>
        /// <param name="memberIndex">成员索引</param>
        /// <returns>成员索引是否有效</returns>
        public bool UnsafeIsMember(int memberIndex)
        {
            return IsMember(memberIndex);
        }
        ///// <summary>
        ///// 判断成员索引是否有效
        ///// </summary>
        ///// <param name="memberName">成员名称</param>
        ///// <returns>成员索引是否有效</returns>
        //public bool IsMember(string memberName)
        //{
        //    int index = Type.GetMemberIndex(memberName);
        //    return index >= 0 && IsMember(index);
        //}
        ///// <summary>
        ///// 成员位图
        ///// </summary>
        ///// <param name="memberMap">成员位图</param>
        //internal abstract void CopyFrom(memberMap memberMap);
        ///// <summary>
        ///// 成员交集运算
        ///// </summary>
        ///// <param name="memberMap">成员位图</param>
        //public abstract void And(memberMap memberMap);
        ///// <summary>
        ///// 成员异或运算,忽略默认成员
        ///// </summary>
        ///// <param name="memberMap">成员位图</param>
        //public abstract void Xor(memberMap memberMap);
        ///// <summary>
        ///// 成员并集运算,忽略默认成员
        ///// </summary>
        ///// <param name="memberMap">成员位图</param>
        //public abstract void Or(memberMap memberMap);
        /// <summary>
        /// 获取成员名称
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        internal static string GetMemberName(LambdaExpression memberExpression)
        {
            Expression expression = memberExpression.Body;
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                MemberInfo member = ((MemberExpression)expression).Member;
                FieldInfo field = member as FieldInfo;
                if (field != null) return field.Name;
                PropertyInfo property = member as PropertyInfo;
                if (property != null) return property.Name;
            }
            return null;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="map"></param>
        /// <param name="fieldCount"></param>
        internal static void FieldSerialize(unmanagedStream stream, ulong map, int fieldCount)
        {
            if (map == 0) stream.Write(0);
            else
            {
                stream.PrepLength(sizeof(int) + sizeof(ulong));
                byte* data = stream.CurrentData;
                *(int*)data = fieldCount;
                *(ulong*)(data + sizeof(int)) = map;
                stream.UnsafeAddLength(sizeof(int) + sizeof(ulong));
                stream.PrepLength();
            }
        }
        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="map"></param>
        /// <param name="otherMap"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal static bool Equals(byte* map, byte* otherMap, int size)
        {
            if (map == null) return otherMap == null;
            if (otherMap != null)
            {
                byte* write = map, end = map + size, read = otherMap;
                ulong bits = *(ulong*)write ^ *(ulong*)read;
                while ((write += sizeof(ulong)) != end) bits |= *(ulong*)write ^ *(ulong*)(read += sizeof(ulong));
                return bits == 0;
            }
            return false;
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="map"></param>
        /// <param name="type"></param>
        internal static void FieldSerialize(unmanagedStream stream, byte* map, type type)
        {
            if (map == null) stream.Write(0);
            else
            {
                stream.PrepLength(type.FieldSerializeSize + sizeof(int));
                byte* data = stream.CurrentData, read = map;
                *(int*)data = type.FieldCount;
                data += sizeof(int);
                for (byte* end = map + (type.FieldSerializeSize & (int.MaxValue - sizeof(ulong) + 1)); read != end; read += sizeof(ulong), data += sizeof(ulong)) *(ulong*)data = *(ulong*)read;
                if ((type.FieldSerializeSize & sizeof(int)) != 0) *(uint*)data = *(uint*)read;
                stream.UnsafeAddLength(type.FieldSerializeSize + sizeof(int));
                stream.PrepLength();
            }
        }
    }
    /// <summary>
    /// 成员位图
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    public unsafe abstract class memberMap<valueType> : memberMap, IEquatable<memberMap<valueType>>
    {
        /// <summary>
        /// 成员位图
        /// </summary>
        internal memberMap() : base(TypeInfo) { }
        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Equals(memberMap<valueType> other);
        /// <summary>
        /// 成员位图
        /// </summary>
        /// <returns>成员位图</returns>
        public abstract memberMap<valueType> Copy();
        /// <summary>
        /// 成员位图
        /// </summary>
        /// <param name="memberMap">成员位图</param>
        internal abstract void CopyFrom(memberMap<valueType> memberMap);
        /// <summary>
        /// 成员交集运算
        /// </summary>
        /// <param name="memberMap">成员位图</param>
        public abstract void And(memberMap<valueType> memberMap);
        /// <summary>
        /// 成员异或运算,忽略默认成员
        /// </summary>
        /// <param name="memberMap">成员位图</param>
        public abstract void Xor(memberMap<valueType> memberMap);
        /// <summary>
        /// 成员并集运算,忽略默认成员
        /// </summary>
        /// <param name="memberMap">成员位图</param>
        public abstract void Or(memberMap<valueType> memberMap);
        /// <summary>
        /// 成员位图
        /// </summary>
        internal class value : memberMap<valueType>
        {
            /// <summary>
            /// 成员位图
            /// </summary>
            private ulong map;
            /// <summary>
            /// 是否默认全部成员有效
            /// </summary>
            public override bool IsDefault
            {
                get
                {
                    return map == 0;
                }
            }
            /// <summary>
            /// 是否存在成员
            /// </summary>
            public override bool IsAnyMember
            {
                get { return map != 0x8000000000000000UL; }
            }
            ///// <summary>
            ///// 比较是否相等
            ///// </summary>
            ///// <param name="other"></param>
            ///// <returns></returns>
            //public override bool Equals(memberMap other)
            //{
            //    value value = other as value;
            //    return value != null && map == value.map;
            //}
            /// <summary>
            /// 比较是否相等
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public override bool Equals(memberMap<valueType> other)
            {
                return map == ((value)other).map;
            }
            /// <summary>
            /// 添加所有成员
            /// </summary>
            internal override void Full()
            {
                map = ulong.MaxValue;
            }
            /// <summary>
            /// 清空所有成员
            /// </summary>
            internal override void Empty()
            {
                map = 0x8000000000000000UL;
            }
            /// <summary>
            /// 序列化
            /// </summary>
            /// <param name="stream"></param>
            internal override void FieldSerialize(unmanagedStream stream)
            {
                memberMap.FieldSerialize(stream, map, TypeInfo.FieldCount);
            }
            /// <summary>
            /// 字段成员反序列化
            /// </summary>
            /// <param name="deSerializer"></param>
            internal override void FieldDeSerialize(fastCSharp.emit.binaryDeSerializer deSerializer)
            {
                map = deSerializer.DeSerializeMemberMap(TypeInfo.FieldCount);
            }
            /// <summary>
            /// 设置成员索引,忽略默认成员
            /// </summary>
            /// <param name="memberIndex">成员索引</param>
            internal override void SetMember(int memberIndex)
            {
                map |= (1UL << memberIndex) | 0x8000000000000000UL;
            }
            /// <summary>
            /// 清除所有成员
            /// </summary>
            internal override void Clear()
            {
                map = 0;
            }
            /// <summary>
            /// 清除成员索引,忽略默认成员
            /// </summary>
            /// <param name="memberIndex">成员索引</param>
            internal override void ClearMember(int memberIndex)
            {
                map &= (1UL << memberIndex) ^ ulong.MaxValue;
            }
            /// <summary>
            /// 判断成员索引是否有效
            /// </summary>
            /// <param name="memberIndex">成员索引</param>
            /// <returns>成员索引是否有效</returns>
            internal override bool IsMember(int memberIndex)
            {
                return map == 0 || (map & (1UL << memberIndex)) != 0;
            }
            /// <summary>
            /// 成员位图
            /// </summary>
            /// <returns>成员位图</returns>
            public override memberMap<valueType> Copy()
            {
                value value = new value();
                value.map = map;
                return value;
            }
            /// <summary>
            /// 成员位图
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            internal override void CopyFrom(memberMap<valueType> memberMap)
            {
                map = ((value)memberMap).map;
            }
            /// <summary>
            /// 成员交集运算
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            public override void And(memberMap<valueType> memberMap)
            {
                if (memberMap == null || memberMap.IsDefault) return;
                //if (TypeInfo != memberMap.Type) log.Error.Throw(log.exceptionType.ErrorOperation);
                if (this.map == 0) this.map = ((value)memberMap).map;
                else this.map &= ((value)memberMap).map;
            }
            /// <summary>
            /// 成员异或运算,忽略默认成员
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            public override void Xor(memberMap<valueType> memberMap)
            {
                if (memberMap == null || memberMap.IsDefault || map == 0) return;
                //if (TypeInfo != memberMap.Type) log.Error.Throw(log.exceptionType.ErrorOperation);
                map ^= ((value)memberMap).map;
                map |= 0x8000000000000000UL;
            }
            /// <summary>
            /// 成员并集运算,忽略默认成员
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            public override void Or(memberMap<valueType> memberMap)
            {
                if (this.map != 0)
                {
                    ulong map = ((value)memberMap).map;
                    if (map == 0) this.map = 0;
                    else this.map |= map;
                }
            }
        }
        /// <summary>
        /// 指针成员位图
        /// </summary>
        internal class point : memberMap<valueType>
        {
            /// <summary>
            /// 成员位图
            /// </summary>
            private byte* map;
            /// <summary>
            /// 是否默认全部成员有效
            /// </summary>
            public override bool IsDefault { get { return map == null; } }
            /// <summary>
            /// 是否存在成员
            /// </summary>
            public override bool IsAnyMember
            {
                get
                {
                    if (map == null) return true;
                    byte* end = map + TypeInfo.MemberMapSize;
                    do
                    {
                        end -= sizeof(ulong);
                        if (*(ulong*)end != 0) return true;
                    }
                    while (end != map);
                    return false;
                }
            }

            ///// <summary>
            ///// 比较是否相等
            ///// </summary>
            ///// <param name="other"></param>
            ///// <returns></returns>
            //public override bool Equals(memberMap other)
            //{
            //    point value = other as point;
            //    if (value != null) return memberMap.Equals(map, value.map, TypeInfo.MemberMapSize);
            //    return false;
            //}
            /// <summary>
            /// 比较是否相等
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public override bool Equals(memberMap<valueType> other)
            {
                return memberMap.Equals(map, ((point)other).map, TypeInfo.MemberMapSize);
            }
            /// <summary>
            /// 添加所有成员
            /// </summary>
            internal override void Full()
            {
                if (map == null) map = TypeInfo.Pool.Get();
                byte* write = map, end = map + TypeInfo.MemberMapSize;
                do
                {
                    *(ulong*)write = ulong.MaxValue;
                }
                while ((write += sizeof(ulong)) != end);
            }
            /// <summary>
            /// 清空所有成员
            /// </summary>
            internal override void Empty()
            {
                if (map == null) map = TypeInfo.Pool.GetClear();
                else
                {
                    byte* write = map, end = map + TypeInfo.MemberMapSize;
                    do
                    {
                        *(ulong*)write = 0;
                    }
                    while ((write += sizeof(ulong)) != end);
                }
            }
            /// <summary>
            /// 序列化
            /// </summary>
            /// <param name="stream"></param>
            internal override void FieldSerialize(unmanagedStream stream)
            {
                memberMap.FieldSerialize(stream, map, TypeInfo);
            }
            /// <summary>
            /// 字段成员反序列化
            /// </summary>
            /// <param name="deSerializer"></param>
            internal override void FieldDeSerialize(fastCSharp.emit.binaryDeSerializer deSerializer)
            {
                if (map == null) map = TypeInfo.Pool.Get();
                deSerializer.DeSerializeFieldMemberMap(map, TypeInfo.FieldCount, TypeInfo.FieldSerializeSize);
            }
            /// <summary>
            /// 设置成员索引,忽略默认成员
            /// </summary>
            /// <param name="memberIndex">成员索引</param>
            internal override void SetMember(int memberIndex)
            {
                if (map == null) map = TypeInfo.Pool.GetClear();
                map[memberIndex >> 3] |= (byte)(1 << (memberIndex & 7));
            }
            /// <summary>
            /// 清除所有成员
            /// </summary>
            internal override void Clear()
            {
                Dispose();
            }
            /// <summary>
            /// 清除成员索引,忽略默认成员
            /// </summary>
            /// <param name="memberIndex">成员索引</param>
            internal override void ClearMember(int memberIndex)
            {
                if (map != null) map[memberIndex >> 3] &= (byte)(byte.MaxValue ^ (1 << (memberIndex & 7)));
            }
            /// <summary>
            /// 判断成员索引是否有效
            /// </summary>
            /// <param name="memberIndex">成员索引</param>
            /// <returns>成员索引是否有效</returns>
            internal override bool IsMember(int memberIndex)
            {
                return map == null || (map[memberIndex >> 3] & (1 << (memberIndex & 7))) != 0;
            }
            /// <summary>
            /// 成员位图
            /// </summary>
            /// <returns>成员位图</returns>
            public override memberMap<valueType> Copy()
            {
                point value = new point();
                if (map != null) unsafer.memory.UnsafeSimpleCopy(map, value.map = TypeInfo.Pool.Get(), TypeInfo.MemberMapSize);
                return value;
            }
            /// <summary>
            /// 成员位图
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            internal override void CopyFrom(memberMap<valueType> memberMap)
            {
                byte* map = ((point)memberMap).map;
                if (map == null) Dispose();
                else
                {
                    if (this.map == null) this.map = TypeInfo.Pool.Get();
                    unsafer.memory.UnsafeSimpleCopy(map, this.map, TypeInfo.MemberMapSize);
                }
            }
            /// <summary>
            /// 成员交集运算
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            public override void And(memberMap<valueType> memberMap)
            {
                if (memberMap == null || memberMap.IsDefault) return;
                //if (TypeInfo != memberMap.Type) log.Error.Throw(log.exceptionType.ErrorOperation);
                if (this.map == null) unsafer.memory.UnsafeSimpleCopy(((point)memberMap).map, this.map = TypeInfo.Pool.Get(), TypeInfo.MemberMapSize);
                else
                {
                    byte* write = this.map, end = this.map + TypeInfo.MemberMapSize, read = ((point)memberMap).map;
                    *(ulong*)write &= *(ulong*)read;
                    while ((write += sizeof(ulong)) != end) *(ulong*)write &= *(ulong*)(read += sizeof(ulong));
                }
            }
            /// <summary>
            /// 成员异或运算,忽略默认成员
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            public override void Xor(memberMap<valueType> memberMap)
            {
                if (memberMap == null || memberMap.IsDefault || map == null) return;
                //if (TypeInfo != memberMap.Type) log.Error.Throw(log.exceptionType.ErrorOperation);
                byte* write = this.map, end = this.map + TypeInfo.MemberMapSize, read = ((point)memberMap).map;
                *(ulong*)write ^= *(ulong*)read;
                while ((write += sizeof(ulong)) != end) *(ulong*)write ^= *(ulong*)(read += sizeof(ulong));
            }
            /// <summary>
            /// 成员并集运算,忽略默认成员
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            public override void Or(memberMap<valueType> memberMap)
            {
                if (map == null) return;
                if (memberMap == null || memberMap.IsDefault)
                {
                    TypeInfo.Pool.Push(map);
                    map = null;
                    return;
                }
                //if (TypeInfo != memberMap.Type) log.Error.Throw(log.exceptionType.ErrorOperation);
                byte* write = this.map, end = this.map + TypeInfo.MemberMapSize, read = ((point)memberMap).map;
                *(ulong*)write |= *(ulong*)read;
                while ((write += sizeof(ulong)) != end) *(ulong*)write |= *(ulong*)(read += sizeof(ulong));
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public override unsafe void Dispose()
            {
                if (map != null)
                {
                    TypeInfo.Pool.Push(map);
                    map = null;
                }
            }
        }
        /// <summary>
        /// 创建成员位图
        /// </summary>
        public struct builder
        {
            /// <summary>
            /// 成员位图
            /// </summary>
            private memberMap<valueType> memberMap;
            /// <summary>
            /// 成员位图
            /// </summary>
            /// <param name="value">创建成员位图</param>
            /// <returns>成员位图</returns>
            public static implicit operator memberMap<valueType>(builder value)
            {
                return value.memberMap;
            }
            /// <summary>
            /// 创建成员位图
            /// </summary>
            /// <param name="isNew">是否创建成员</param>
            internal builder(bool isNew)
            {
                memberMap = isNew ? New() : null;
            }
            /// <summary>
            /// 创建成员位图
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            internal builder(memberMap<valueType> memberMap)
            {
                this.memberMap = memberMap;
            }
            /// <summary>
            /// 添加成员
            /// </summary>
            /// <typeparam name="returnType"></typeparam>
            /// <param name="member"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public builder Append<returnType>(Expression<Func<valueType, returnType>> member)
            {
                if (memberMap != null) memberMap.SetMember(member);
                return this;
            }
            /// <summary>
            /// 添加成员
            /// </summary>
            /// <typeparam name="returnType"></typeparam>
            /// <param name="member"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public builder Clear<returnType>(Expression<Func<valueType, returnType>> member)
            {
                if (memberMap != null) memberMap.ClearMember(member);
                return this;
            }
        }
        /// <summary>
        /// 成员位图类型信息
        /// </summary>
        internal static readonly memberMap.type TypeInfo = new memberMap.type(typeof(valueType), memberIndexGroup<valueType>.GetAllMembers(), memberIndexGroup<valueType>.FieldCount);
        /// <summary>
        /// 默认成员位图
        /// </summary>
        internal static readonly memberMap<valueType> Default = New();
        /// <summary>
        /// 默认成员位图
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static memberMap<valueType> New()
        {
            return TypeInfo.MemberCount < 64 ? (memberMap<valueType>)new memberMap<valueType>.value() : new memberMap<valueType>.point();
        }
        /// <summary>
        /// 所有成员位图
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static memberMap<valueType> NewFull()
        {
            memberMap<valueType> value = TypeInfo.MemberCount < 64 ? (memberMap<valueType>)new memberMap<valueType>.value() : new memberMap<valueType>.point();
            value.Full();
            return value;
        }
        /// <summary>
        /// 空成员位图
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static memberMap<valueType> NewEmpty()
        {
            memberMap<valueType> value = TypeInfo.MemberCount < 64 ? (memberMap<valueType>)new memberMap<valueType>.value() : new memberMap<valueType>.point();
            value.Empty();
            return value;
        }
        /// <summary>
        /// 创建成员索引
        /// </summary>
        /// <typeparam name="returnType"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        public static memberMap.memberIndex CreateMemberIndex<returnType>(Expression<Func<valueType, returnType>> member)
        {
            int index = TypeInfo.GetMemberIndex(memberMap.GetMemberName(member));
            return index >= 0 ? new memberMap.memberIndex(index) : null;
        }
        /// <summary>
        /// 创建成员位图
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static builder CreateBuilder()
        {
            return new builder(true);
        }
        ///// <summary>
        ///// 判断成员位图类型是否匹配
        ///// </summary>
        ///// <typeparam name="valueType"></typeparam>
        ///// <param name="memberMap"></param>
        ///// <returns></returns>
        //public static bool IsType(memberMap memberMap)
        //{
        //    return memberMap.Type == Type;
        //}
        ///// <summary>
        ///// 成员位图
        ///// </summary>
        ///// <param name="names"></param>
        ///// <returns></returns>
        //internal static memberMap New(params string[] names)
        //{
        //    if (Type.MemberCount < 64)
        //    {
        //        memberMap.value value = new memberMap.value(Type);
        //        foreach (string name in names) value.SetMember(name);
        //        return value;
        //    }
        //    memberMap.point point = new memberMap.point(Type);
        //    foreach (string name in names) point.SetMember(name);
        //    return point;
        //}
        ///// <summary>
        ///// 成员位图
        ///// </summary>
        ///// <param name="members"></param>
        ///// <returns></returns>
        //public static memberMap New(params LambdaExpression[] members)
        //{
        //    if (Type.MemberCount < 64)
        //    {
        //        memberMap.value value = new memberMap.value(Type);
        //        foreach (LambdaExpression member in members) value.SetMember(member);
        //        return value;
        //    }
        //    memberMap.point point = new memberMap.point(Type);
        //    foreach (LambdaExpression member in members) point.SetMember(member);
        //    return point;
        //}
    }
}
