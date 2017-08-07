using System;
using fastCSharp.code.cSharp;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 分组列表缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组字典关键字类型</typeparam>
    public class dictionaryList<valueType, modelType, memberCacheType, keyType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 整表缓存
        /// </summary>
        protected events.cache<valueType, modelType, memberCacheType> cache;
        /// <summary>
        /// 分组字典关键字获取器
        /// </summary>
        protected Func<valueType, keyType> getKey;
        /// <summary>
        /// 分组数据
        /// </summary>
        protected Dictionary<randomKey<keyType>, keyValue<keyType, list<valueType>>> groups;
        /// <summary>
        /// 关键字版本号
        /// </summary>
        protected int keyVersion;
        /// <summary>
        /// 移除数据并使用最后一个数据移动到当前位置
        /// </summary>
        protected bool isRemoveEnd;
        /// <summary>
        /// 分组列表缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="isRemoveEnd">移除数据并使用最后一个数据移动到当前位置</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
        public dictionaryList(events.cache<valueType, modelType, memberCacheType> cache, Func<valueType, keyType> getKey, bool isRemoveEnd, bool isReset)
        {
            if (cache == null || getKey == null) log.Error.Throw(log.exceptionType.Null);
            this.cache = cache;
            this.getKey = getKey;
            this.isRemoveEnd = isRemoveEnd;

            if (isReset)
            {
                cache.OnReset += reset;
                cache.OnInserted += onInserted;
                cache.OnUpdated += onUpdated;
                cache.OnDeleted += onDeleted;
                resetLock();
            }
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected void resetLock()
        {
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                reset();
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected virtual void reset()
        {
            insert insert = new insert { Groups = dictionary<randomKey<keyType>>.Create<keyValue<keyType, list<valueType>>>(), GetKey = getKey };
            foreach (valueType value in cache.Values) insert.OnInserted(value);
            this.groups = insert.Groups;
            ++keyVersion;
        }
        /// <summary>
        /// 数据添加器
        /// </summary>
        protected struct insert
        {
            /// <summary>
            /// 分组数据
            /// </summary>
            public Dictionary<randomKey<keyType>, keyValue<keyType, list<valueType>>> Groups;
            /// <summary>
            /// 分组字典关键字获取器
            /// </summary>
            public Func<valueType, keyType> GetKey;
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value">数据对象</param>
            public void OnInserted(valueType value)
            {
                keyValue<keyType, list<valueType>> list;
                keyType key = GetKey(value);
                if (!Groups.TryGetValue(key, out list)) Groups.Add(key, list = new keyValue<keyType, list<valueType>>(key, new list<valueType>()));
                list.Value.Add(value);
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value">数据对象</param>
            /// <param name="cache">缓存对象</param>
            public void OnInserted(valueType value, dictionaryList<valueType, modelType, memberCacheType, keyType> cache)
            {
                keyValue<keyType, list<valueType>> list;
                keyType key = GetKey(value);
                if (!Groups.TryGetValue(key, out list))
                {
                    Groups.Add(key, list = new keyValue<keyType, list<valueType>>(key, new list<valueType>()));
                    ++cache.keyVersion;
                }
                list.Value.Add(value);
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onInserted(valueType value)
        {
            new insert { Groups = groups, GetKey = getKey }.OnInserted(value, this);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        protected void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            keyType oldKey = getKey(oldValue);
            if (!getKey(value).Equals(oldKey))
            {
                onInserted(cacheValue);
                onDeleted(cacheValue, oldKey);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        /// <param name="key">被删除的数据关键字</param>
        protected void onDeleted(valueType value, keyType key)
        {
            keyValue<keyType, list<valueType>> keyList;
            if (groups.TryGetValue(key, out keyList))
            {
                list<valueType> list = keyList.Value;
                int index = Array.LastIndexOf(list.UnsafeArray, value, list.Count - 1);
                if (index != -1)
                {
                    if (list.Count != 1)
                    {
                        if (isRemoveEnd) list.RemoveAtEnd(index);
                        else list.RemoveAt(index);
                    }
                    else groups.Remove(key);
                    return;
                }
            }
            log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onDeleted(valueType value)
        {
            onDeleted(value, getKey(value));
        }
        /// <summary>
        /// 获取匹配数据数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>匹配数据数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int Count(keyType key)
        {
            keyValue<keyType, list<valueType>> list;
            return groups.TryGetValue(key, out list) ? list.Value.Count : 0;
        }
        /// <summary>
        /// 获取分组缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public list<valueType> GetNoLock(keyType key)
        {
            keyValue<keyType, list<valueType>> list;
            return groups.TryGetValue(key, out list) ? list.Value : null;
        }
        /// <summary>
        /// 获取匹配数据数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器,禁止锁操作</param>
        /// <returns>匹配数据数量</returns>
        public int CountLock(keyType key, Func<valueType, bool> isValue)
        {
            keyValue<keyType, list<valueType>> list;
            int version = keyVersion;
            if (groups.TryGetValue(key, out list) && list.Key.Equals(key))
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    return list.Value.GetCount(isValue);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            else if (version != keyVersion)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    if (groups.TryGetValue(key, out list)) return list.Value.GetCount(isValue);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return 0;
        }
        /// <summary>
        /// 获取第一个数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="nullValue">默认失败空值</param>
        /// <returns>数据集合</returns>
        public valueType First(keyType key, valueType nullValue = null)
        {
            keyValue<keyType, list<valueType>> list;
            int version = keyVersion;
            if (groups.TryGetValue(key, out list) && list.Key.Equals(key))
            {
                return list.Value.Count == 0 ? nullValue : list.Value.UnsafeArray[0];
            }
            else if (version != keyVersion)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    if (groups.TryGetValue(key, out list))
                    {
                        return list.Value.Count == 0 ? nullValue : list.Value.UnsafeArray[0];
                    }
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return nullValue;
        }
        /// <summary>
        /// 获取第一个匹配数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器,禁止锁操作</param>
        /// <returns>第一个匹配数据</returns>
        public valueType FirstOrDefaultLock(keyType key, Func<valueType, bool> isValue)
        {
            keyValue<keyType, list<valueType>> list;
            int version = keyVersion;
            if (groups.TryGetValue(key, out list) && list.Key.Equals(key))
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    return list.Value.FirstOrDefault(isValue);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            else if (version != keyVersion)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    if (groups.TryGetValue(key, out list)) return list.Value.FirstOrDefault(isValue);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return null;
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据集合</returns>
        public valueType[] GetArray(keyType key)
        {
            keyValue<keyType, list<valueType>> list;
            int version = keyVersion;
            if (!groups.TryGetValue(key, out list) || !list.Key.Equals(key) && version != keyVersion)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    groups.TryGetValue(key, out list);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return list.Value.getArray();
        }
        /// <summary>
        /// 获取匹配数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器,禁止锁操作</param>
        /// <returns>匹配数据集合</returns>
        public valueType[] GetFindArrayLock(keyType key, Func<valueType, bool> isValue)
        {
            keyValue<keyType, list<valueType>> list;
            int version = keyVersion;
            if (groups.TryGetValue(key, out list) && list.Key.Equals(key))
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    return list.Value.GetFindArray(isValue);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            else if (version != keyVersion)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    if (groups.TryGetValue(key, out list)) return list.Value.GetFindArray(isValue);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取匹配数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器,禁止锁操作</param>
        /// <returns>匹配数据集合</returns>
        public valueType[] GetFindArray(keyType key, Func<valueType, bool> isValue)
        {
            keyValue<keyType, list<valueType>> list;
            int version = keyVersion;
            if (!groups.TryGetValue(key, out list) || !list.Key.Equals(key) && version != keyVersion)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    groups.TryGetValue(key, out list);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return list.Value.toSubArray().GetFindArray(isValue);
        }
        /// <summary>
        /// 获取逆序分页数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public valueType[] GetPageDesc(keyType key, int pageSize, int currentPage, out int count)
        {
            keyValue<keyType, list<valueType>> list;
            int version = keyVersion;
            if (!groups.TryGetValue(key, out list) || !list.Key.Equals(key) && version != keyVersion)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    groups.TryGetValue(key, out list);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            count = list.Value.count();
            return list.Value.toSubArray().GetPageDesc(pageSize, currentPage);
        }
    }
}
