using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code.cSharp;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.part.events
{
    /// <summary>
    /// 缓存计数器
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public abstract class counter<valueType, modelType, keyType> : copy<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 缓存关键字获取器
        /// </summary>
        internal Func<modelType, keyType> GetKey { get; private set; }
        /// <summary>
        /// 缓存数据
        /// </summary>
        private Dictionary<randomKey<keyType>, keyValue<valueType, int>> values;
        /// <summary>
        /// 缓存数据数量
        /// </summary>
        public int Count
        {
            get { return values.Count; }
        }
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存值</returns>
        public valueType this[keyType key]
        {
            get
            {
                return Get(key);
            }
        }
        /// <summary>
        /// 缓存计数
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="getKey">缓存关键字获取器</param>
        /// <param name="group">数据分组</param>
        protected counter(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, int group, Expression<Func<modelType, keyType>> getKey)
            : this(sqlTool, group, getKey == null ? null : getKey.Compile())
        {
            sqlTool.SetSelectMember(getKey);
        }
        /// <summary>
        /// 缓存计数
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="getKey">缓存关键字获取器</param>
        /// <param name="group">数据分组</param>
        protected counter(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, int group, Func<modelType, keyType> getKey)
            : base(sqlTool, group)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            GetKey = getKey;
            values = dictionary<randomKey<keyType>>.Create<keyValue<valueType, int>>();

            sqlTool.OnUpdatedLock += onUpdated;
            sqlTool.OnDeletedLock += onDeleted;
        }
        /// <summary>
        /// 设置更新查询SQL数据成员
        /// </summary>
        /// <param name="member">字段表达式</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetSelectMember<returnType>(Expression<Func<modelType, returnType>> member)
        {
            SqlTool.SetSelectMember(member, memberMap);
        }
        /// <summary>
        /// 重置缓存
        /// </summary>
        protected override void reset()
        {
            values.Clear();
        }
        /// <summary>
        /// 更新记录事件
        /// </summary>
        public event Action<valueType, valueType, valueType, fastCSharp.code.memberMap<modelType>> OnUpdated;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        /// <param name="memberMap">更新成员位图</param>
        private void onUpdated(valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            keyValue<valueType, int> cacheValue;
            keyType key = GetKey(value);
            if (values.TryGetValue(key, out cacheValue)) update(cacheValue.Key, value, oldValue, memberMap);
            if (OnUpdated != null) OnUpdated(cacheValue.Key, value, oldValue, memberMap);
        }
        /// <summary>
        /// 删除记录事件
        /// </summary>
        public event Action<valueType> OnDeleted;
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        private void onDeleted(valueType value)
        {
            keyValue<valueType, int> cacheValue;
            keyType key = GetKey(value);
            if (values.TryGetValue(key, out cacheValue))
            {
                values.Remove(GetKey(value));
                if (OnDeleted != null) OnDeleted(cacheValue.Key);
            }
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal valueType Get(keyType key)
        {
            keyValue<valueType, int> valueCount;
            return values.TryGetValue(key, out valueCount) ? valueCount.Key : null;
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="value">查询数据</param>
        /// <returns>缓存数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal valueType Get(valueType value)
        {
            keyValue<valueType, int> valueCount;
            return values.TryGetValue(GetKey(value), out valueCount) ? valueCount.Key : null;
        }
        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="value">缓存数据</param>
        /// <returns>缓存数据</returns>
        internal valueType Add(valueType value)
        {
            keyValue<valueType, int> valueCount;
            keyType key = GetKey(value);
            if (values.TryGetValue(key, out valueCount))
            {
                ++valueCount.Value;
                values[key] = valueCount;
                return valueCount.Key;
            }
            valueType copyValue = fastCSharp.emit.constructor<valueType>.New();
            fastCSharp.emit.memberCopyer<modelType>.Copy(copyValue, value, memberMap);
            values.Add(key, new keyValue<valueType, int>(copyValue, 0));
            return copyValue;
        }
        /// <summary>
        /// 删除缓存数据
        /// </summary>
        /// <param name="value">缓存数据</param>
        internal void Remove(valueType value)
        {
            keyValue<valueType, int> valueCount;
            keyType key = GetKey(value);
            if (values.TryGetValue(key, out valueCount))
            {
                if (valueCount.Value == 0) values.Remove(key);
                else
                {
                    --valueCount.Value;
                    values[key] = valueCount;
                }
            }
            else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }

        /// <summary>
        /// 创建先进先出优先队列缓存
        /// </summary>
        /// <param name="getValue">数据获取器,禁止锁操作</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.part.queue<valueType, modelType, keyType> CreateQueue(Func<keyType, fastCSharp.code.memberMap<modelType>, valueType> getValue, int maxCount = 0)
        {
            return new fastCSharp.sql.cache.part.queue<valueType, modelType, keyType>(this, getValue, maxCount);
        }
    }
}
