using System;
using System.Threading;
using fastCSharp.code.cSharp;

namespace fastCSharp.memoryDatabase.cache
{
    /// <summary>
    /// 自增数组缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    public abstract class identityArrayBase<valueType, memberType> : identity<valueType, memberType>, ILoadIdentityCache<valueType, memberType, int>
        where valueType : class, fastCSharp.data.IPrimaryKey<int>
        where memberType : IMemberMap<memberType>
    {
        /// <summary>
        /// 取消自增值
        /// </summary>
        /// <param name="identity">自增值</param>
        public void CancelIdentity(int identity)
        {
        }
        /// <summary>
        /// 根据自增值计算数组长度
        /// </summary>
        /// <param name="identity">自增值</param>
        /// <returns>数组长度</returns>
        private int getArrayLength(int identity)
        {
            return (identity + identityArrayLength - 1) / identityArrayLength * identityArrayLength;
        }
        /// <summary>
        /// 新建对象数组
        /// </summary>
        /// <param name="identity">自增值</param>
        private void newArray(int identity)
        {
            int length = array.Length << 1;
            valueLock[] newArray = new valueLock[length > identity ? length : getArrayLength(identity)];
            array.CopyTo(newArray, 0);
            array = newArray;
        }
        /// <summary>
        /// 加载日志添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        protected override void loadInsert(valueType value, int logSize)
        {
            int identity = value.PrimaryKey;
            if (array == null) array = new valueLock[getArrayLength(identity)];
            else if (identity >= array.Length) newArray(identity);
            count += array[identity].SetIfNull(value, logSize);
            if (identity > currentIdentity) currentIdentity = identity;
        }
        /// <summary>
        /// 加载日志修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        protected override void loadUpdate(valueType value, memberType memberMap)
        {
            if (array != null)
            {
                int identity = value.PrimaryKey;
                if ((uint)identity < (uint)array.Length)
                {
                    valueType cacheValue = array[identity].Value;
                    if (cacheValue != null) copy(cacheValue, value, memberMap);
                }
            }
        }
        /// <summary>
        /// 加载日志删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>日志字节长度</returns>
        protected override int loadDelete(int key)
        {
            if (array != null && (uint)key < (uint)array.Length)
            {
                keyValue<int, int> logSize = array[key].Clear();
                count -= logSize.Key;
                return logSize.Value;
            }
            return 0;
        }
        /// <summary>
        /// 获取当前数据集合
        /// </summary>
        protected override valueType[] getArray()
        {
            int index = 0;
            Monitor.Enter(cacheLock);
            try
            {
                if (array != null)
                {
                    valueType[] values = new valueType[count];
                    foreach (valueLock value in array)
                    {
                        if (value.Value != null)
                        {
                            values[index++] = value.Value;
                            if (index == count) break;
                        }
                    }
                    return values;
                }
            }
            finally { Monitor.Exit(cacheLock); }
            return null;
        }
        /// <summary>
        /// 获取操作队列锁
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>操作队列锁,失败返回null</returns>
        protected override object getLock(int key)
        {
            valueLock[] array = this.array;
            if (array != null && (uint)key < (uint)array.Length)
            {
                object value = array[key].Lock;
                if (value != null) return value;
                Monitor.Enter(cacheLock);
                try
                {
                    if (this.array != null) return this.array[key].GetLock();
                }
                finally { Monitor.Exit(cacheLock); }
            }
            return null;
        }
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        protected override void insert(valueType value, int logSize)
        {
            int identity = value.PrimaryKey;
            Monitor.Enter(cacheLock);
            try
            {
                if (array != null)
                {
                    if (identity >= array.Length) newArray(identity);
                    count += array[identity].SetIfNull(value, logSize);
                }
            }
            finally { Monitor.Exit(cacheLock); }
        }
        /// <summary>
        /// 根据关键字获取对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>对象数据</returns>
        private valueType get(int identity)
        {
            valueLock[] array = this.array;
            return array != null && (uint)identity < (uint)array.Length ? array[identity].Value : null;
        }
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <returns>修改后的对象,失败返回null</returns>
        protected override valueType update(valueType value, memberType memberMap)
        {
            valueType cacheValue = get(value.PrimaryKey);
            if (cacheValue != null)
            {
                copy(cacheValue, value, memberMap);
                return cacheValue;
            }
            return null;
        }
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <returns>修改后的对象,修改前的对象</returns>
        protected override keyValue<valueType, valueType> update2(valueType value, memberType memberMap)
        {
            valueType cacheValue = get(value.PrimaryKey), oldValue = null;
            if (cacheValue != null)
            {
                oldValue = fastCSharp.emit.memberCopyer<valueType>.MemberwiseClone(cacheValue);
                copy(cacheValue, value, memberMap);
            }
            return new keyValue<valueType, valueType>(cacheValue, oldValue);
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>被删除对象+日志字节长度,失败返回null</returns>
        protected override keyValue<valueType, int> delete(int key)
        {
            keyValue<valueType, int> value = default(keyValue<valueType, int>);
            if ((uint)key < (uint)array.Length)
            {
                Monitor.Enter(cacheLock);
                try
                {
                    if (array != null && (value = array[key].Remove()).Key != null) --count;
                }
                finally { Monitor.Exit(cacheLock); }
            }
            return value;
        }
        /// <summary>
        /// 根据关键字获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据对象</param>
        /// <returns>是否存在对象</returns>
        protected override bool get(int key, ref valueType value)
        {
            valueLock[] array = this.array;
            if (array != null && (uint)key < (uint)array.Length)
            {
                valueType cacheValue = array[key].Value;
                if (cacheValue != null)
                {
                    value = cacheValue;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        protected override void dispose()
        {
            count = 0;
            array = null;
        }
    }
    /// <summary>
    /// 自增数组缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    public class identityArray<valueType, memberType> : identityArrayBase<valueType, memberType>
        where valueType : class, fastCSharp.data.IPrimaryKey<int>, ICopy<valueType, memberType>
        where memberType : IMemberMap<memberType>
    {
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="value">目标数据</param>
        /// <param name="copyValue">被复制数据</param>
        /// <param name="memberMap">复制成员位图</param>
        protected override void copy(valueType value, valueType copyValue, memberType memberMap)
        {
            value.CopyFrom(copyValue, memberMap);
        }
    }
    /// <summary>
    /// 自增数组缓存(反射模式)
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public class identityArray<valueType> : identityArrayBase<valueType, memberMap<valueType>>
        where valueType : class, fastCSharp.data.IPrimaryKey<int>
    {
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="value">目标数据</param>
        /// <param name="copyValue">被复制数据</param>
        /// <param name="memberMap">复制成员位图</param>
        protected override void copy(valueType value, valueType copyValue, memberMap<valueType> memberMap)
        {
            copy<valueType>.Copy(value, copyValue, code.memberFilters.Instance, memberMap);
        }
    }
}
