using System;
using System.Threading;
using fastCSharp.code.cSharp;

namespace fastCSharp.memoryDatabase.cache
{
    /// <summary>
    /// 自增树缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    public abstract unsafe class identityTreeBase<valueType, memberType> : identity<valueType, memberType>, ILoadIdentityCache<valueType, memberType, int>
        where valueType : class, fastCSharp.data.IPrimaryKey<int>
        where memberType : IMemberMap<memberType>
    {
        /// <summary>
        /// 取消自增值
        /// </summary>
        /// <param name="identity">自增值</param>
        public void CancelIdentity(int identity)
        {
            while (identity > insertIdentity)
            {
                Thread.Sleep(0);
                if (identity > insertIdentity) Thread.Sleep(1);
            }
            if (identity == insertIdentity) Interlocked.CompareExchange(ref insertIdentity, identity + 1, identity);
        }
        /// <summary>
        /// 删除节点计数
        /// </summary>
        private int* counts;
        /// <summary>
        /// 计数节点数量
        /// </summary>
        private int countSize;
        /// <summary>
        /// 当前对象索引
        /// </summary>
        private int currentIndex;
        /// <summary>
        /// 最大子增值
        /// </summary>
        private int maxIdentity;
        /// <summary>
        /// 下一个添加数据对象索引
        /// </summary>
        private int insertIdentity;
        /// <summary>
        /// 获取数据对象索引
        /// </summary>
        /// <param name="identity">自增值</param>
        /// <returns>数据对象索引,失败返回null</returns>
        private int indexOf(int identity)
        {
            if (currentIndex != 0)
            {
                int start = 0, length = currentIndex;
                while (start < length)
                {
                    int average = start + ((length - start) >> 1), cmp = identity - array[average].Value.PrimaryKey;
                    if (cmp < 0) length = average;
                    else if (cmp == 0) return array[average].Lock == deleteLock ? -1 : average;
                    else start = average + 1;
                }
            }
            return -1;
        }
        /// <summary>
        /// 加载日志添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        protected override void loadInsert(valueType value, int logSize)
        {
            int identity = value.PrimaryKey;
            if (identity > maxIdentity)
            {
                if (count == currentIndex)
                {
                    if (array == null) array = new valueLock[identityArrayLength];
                    else if (currentIndex == array.Length)
                    {
                        valueLock[] newArray = new valueLock[array.Length << 1];
                        array.CopyTo(newArray, 0);
                        array = newArray;
                    }
                }
                else if (currentIndex == array.Length)
                {
                    if ((count << 1) > array.Length)
                    {
                        valueLock[] newArray = new valueLock[array.Length << 1];
                        currentIndex = 0;
                        foreach (valueLock cacheValue in array)
                        {
                            if (cacheValue.NotDelete) newArray[currentIndex++] = cacheValue;
                        }
                        array = newArray;
                    }
                    else
                    {
                        for (currentIndex = 0; array[currentIndex].NotDelete; ++currentIndex) ;
                        for (int index = currentIndex; currentIndex != count; array[currentIndex++] = array[index])
                        {
                            while (array[++index].IsDelete) ;
                        }
                    }
                }
                array[currentIndex++].Set(value, logSize);
                ++count;
                maxIdentity = identity;
            }
            else log.Error.Add("自增值错误 " + identity.toString() + " <= " + maxIdentity.toString(), false, false);
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
                int index = indexOf(value.PrimaryKey);
                if (index != -1) copy(array[index].Value, value, memberMap);
            }
        }
        /// <summary>
        /// 加载日志删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>日志字节长度</returns>
        protected override int loadDelete(int key)
        {
            return remove(key).Value;
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>被删除对象+日志字节长度,失败返回null</returns>
        private keyValue<valueType, int> remove(int key)
        {
            if (array != null)
            {
                int index = indexOf(key);
                if (index != -1)
                {
                    if (count == currentIndex)
                    {
                        if (countSize == array.Length) fastCSharp.unsafer.memory.Fill(counts, 0UL, array.Length >> 3);
                        else
                        {
                            if (counts != null)
                            {
                                unmanaged.Free(counts);
                                countSize = 0;
                                counts = null;
                            }
                            counts = unmanaged.Get(array.Length * sizeof(int), true).Int;
                            countSize = array.Length;
                        }
                    }
                    keyValue<valueType, int> value = array[index].Delete();
                    --count;
                    do
                    {
                        --counts[index];
                        index = (index | (index - 1)) + 1;
                    }
                    while (index != countSize);
                    return value;
                }
            }
            return default(keyValue<valueType, int>);
        }
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        protected override void loaded()
        {
            base.loaded();
            insertIdentity = currentIdentity + 1;
        }
        /// <summary>
        /// 获取操作队列锁
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>操作队列锁,失败返回null</returns>
        protected override object getLock(int key)
        {
            if (this.array != null && (uint)key < (uint)maxIdentity)
            {
                Monitor.Enter(cacheLock);
                try
                {
                    if (this.array != null)
                    {
                        int index = indexOf(key);
                        if (index != -1) return array[index].Lock;
                    }
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
            while (identity != insertIdentity)
            {
                Thread.Sleep(0);
                if (identity != insertIdentity) Thread.Sleep(1);
            }
            Monitor.Enter(cacheLock);
            try
            {
                loadInsert(value, logSize);
            }
            finally
            {
                Monitor.Exit(cacheLock);
                Interlocked.CompareExchange(ref insertIdentity, identity + 1, identity);
            }
        }
        /// <summary>
        /// 根据关键字获取对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>对象数据</returns>
        private valueType get(int identity)
        {
            if (identity <= maxIdentity)
            {
                valueType cacheValue = null;
                Monitor.Enter(cacheLock);
                try
                {
                    if (array != null)
                    {
                        int index = indexOf(identity);
                        if (index != -1) cacheValue = array[index].Value;
                    }
                }
                finally { Monitor.Exit(cacheLock); }
                return cacheValue;
            }
            return null;
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
        /// 加载日志删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>被删除对象+日志字节长度,失败返回null</returns>
        protected override keyValue<valueType, int> delete(int key)
        {
            if (key <= maxIdentity)
            {
                Monitor.Enter(cacheLock);
                try
                {
                    return remove(key);
                }
                finally { Monitor.Exit(cacheLock); }
            }
            return default(keyValue<valueType, int>);
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
                        if (value.NotDelete)
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
        /// 根据关键字获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据对象</param>
        /// <returns>是否存在对象</returns>
        protected override bool get(int key, ref valueType value)
        {
            if (key <= maxIdentity)
            {
                int index = -1;
                Monitor.Enter(cacheLock);
                try
                {
                    index = indexOf(key);
                }
                finally { Monitor.Exit(cacheLock); }
                if (index != -1)
                {
                    value = array[index].Value;
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
            currentIndex = 0;
            int* counts = this.counts;
            array = null;
            this.counts = null;
            unmanaged.Free(counts);
        }
    }
    /// <summary>
    /// 自增树缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    public class identityTree<valueType, memberType> : identityTreeBase<valueType, memberType>
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
    /// 自增树缓存(反射模式)
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public class identityTree<valueType> : identityTreeBase<valueType, memberMap<valueType>>
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
