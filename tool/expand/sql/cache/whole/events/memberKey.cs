using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.whole.events
{
    /// <summary>
    /// 关键字整表缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public sealed class memberKey<valueType, modelType, keyType> : key<valueType, modelType, valueType, keyType>
        where valueType : class, modelType
        where modelType : class
        where keyType : struct, IEquatable<keyType>
    {
        /// <summary>
        /// 事件缓存
        /// </summary>
        private cache<valueType, modelType> cache;
        /// <summary>
        /// 获取数据
        /// </summary>
        private Func<keyType, valueType> getValue;
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        private Func<valueType, valueType> getMember;
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        private Action<valueType, valueType> setMember;
        /// <summary>
        /// 不支持
        /// </summary>
        public override IEnumerable<valueType> Values
        {
            get
            {
                foreach (valueType target in cache.Values)
                {
                    valueType value = getMember(target);
                    if (value != null) yield return value;
                }
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="memberKey"></param>
        /// <returns>数据</returns>
        public override valueType this[keyType key]
        {
            get
            {
                valueType target = getValue(key);
                return target != null ? getMember(target) : null;
            }
        }
        /// <summary>
        /// 缓存数据数量
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// 关键字整表缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="getKey">键值获取器</param>
        /// <param name="getValue">根据关键字获取数据</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="group">数据分组</param>
        public memberKey(cache<valueType, modelType> cache, Func<modelType, keyType> getKey
            , Func<keyType, valueType> getValue, Expression<Func<valueType, valueType>> member, int group = 1)
            : base(cache.SqlTool, null, getKey, group)
        {
            if (getValue == null || member == null) log.Error.Throw(log.exceptionType.Null);
            memberExpression<valueType, valueType> expression = new memberExpression<valueType, valueType>(member);
            if (expression.Field == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            this.cache = cache;
            this.getValue = getValue;
            getMember = expression.GetMember;
            setMember = expression.SetMember;

            cache.SqlTool.OnInsertedLock += onInserted;
            cache.SqlTool.OnUpdatedLock += onUpdated;
            cache.SqlTool.OnDeletedLock += onDeleted;

            resetLock();
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected override void reset()
        {
            foreach (valueType value in SqlTool.Where(null, memberMap)) insert(value);
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        private void insert(valueType value)
        {
            setMember(getValue(GetKey(value)), value);
            ++Count;
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        private void onInserted(valueType value)
        {
            valueType newValue = fastCSharp.emit.constructor<valueType>.New();
            fastCSharp.emit.memberCopyer<modelType>.Copy(newValue, value, memberMap);
            insert(newValue);
            callOnInserted(newValue);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        /// <param name="memberMap">更新成员位图</param>
        private void onUpdated(valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            valueType cacheValue = getMember(getValue(GetKey(value)));
            if (cacheValue != null)
            {
                update(cacheValue, value, oldValue, memberMap);
                callOnUpdated(cacheValue, value, oldValue, memberMap);
            }
            else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        private void onDeleted(valueType value)
        {
            valueType cacheValue = getMember(getValue(GetKey(value)));
            if (cacheValue != null) callOnDeleted(cacheValue);
            else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
    }
    /// <summary>
    /// 关键字整表缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="memberKeyType"></typeparam>
    /// <typeparam name="targetType"></typeparam>
    public sealed class memberKey<valueType, modelType, memberCacheType, keyType, memberKeyType, targetType> : cache<valueType, modelType, memberCacheType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
        where keyType : struct, IEquatable<keyType>
        where memberKeyType : struct, IEquatable<memberKeyType>
        where targetType : class
    {
        /// <summary>
        /// 获取关键字
        /// </summary>
        private Func<modelType, keyType> getKey;
        /// <summary>
        /// 获取关键字
        /// </summary>
        private Func<modelType, memberKeyType> getMemberKey;
        /// <summary>
        /// 获取数据
        /// </summary>
        private Func<keyType, targetType> getValue;
        /// <summary>
        /// 获取所有节点集合
        /// </summary>
        private Func<IEnumerable<targetType>> getTargets;
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        private Func<targetType, keyValue<Dictionary<randomKey<memberKeyType>, valueType>, int>> getMember;
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        private Action<targetType, keyValue<Dictionary<randomKey<memberKeyType>, valueType>, int>> setMember;
        /// <summary>
        /// 不支持
        /// </summary>
        public override IEnumerable<valueType> Values
        {
            get
            {
                foreach (targetType target in getTargets())
                {
                    keyValue<Dictionary<randomKey<memberKeyType>, valueType>, int> cache = getMember(target);
                    if (cache.Key != null)
                    {
                        foreach (valueType value in cache.Key.Values) yield return value;
                    }
                }
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="memberKey"></param>
        /// <returns>数据</returns>
        public valueType this[keyType key, memberKeyType memberKey]
        {
            get
            {
                targetType target = getValue(key);
                if (target == null) return null;
                keyValue<Dictionary<randomKey<memberKeyType>, valueType>, int> cache = getMember(target);
                if (cache.Key == null) return null;
                do
                {
                    valueType value;
                    if (cache.Key.TryGetValue(memberKey, out value) && getMemberKey(value).Equals(memberKey)) return value;
                    Thread.Sleep(0);
                    keyValue<Dictionary<randomKey<memberKeyType>, valueType>, int> newCache = getMember(target);
                    if (cache.Value == newCache.Value) return null;
                    cache = newCache;
                }
                while (true);
            }
        }
        /// <summary>
        /// 缓存数据数量
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// 关键字整表缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="memberCache">成员缓存</param>
        /// <param name="getKey">键值获取器</param>
        /// <param name="getMemberKey">成员缓存键值获取器</param>
        /// <param name="member">缓存成员</param>
        /// <param name="group">数据分组</param>
        public memberKey(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<valueType, memberCacheType>> memberCache
            , Func<modelType, keyType> getKey, Func<modelType, memberKeyType> getMemberKey, Func<keyType, targetType> getValue
            , Expression<Func<targetType, keyValue<Dictionary<randomKey<memberKeyType>, valueType>, int>>> member
            , Func<IEnumerable<targetType>> getTargets, int group = 0)
            : base(sqlTool, memberCache, group)
        {
            if (getKey == null || getMemberKey == null || getValue == null || getTargets == null || member == null) log.Error.Throw(log.exceptionType.Null);
            memberExpression<targetType, keyValue<Dictionary<randomKey<memberKeyType>, valueType>, int>> expression = new memberExpression<targetType, keyValue<Dictionary<randomKey<memberKeyType>, valueType>, int>>(member);
            if (expression.Field == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            this.getKey = getKey;
            this.getMemberKey = getMemberKey;
            this.getValue = getValue;
            this.getTargets = getTargets;
            getMember = expression.GetMember;
            setMember = expression.SetMember;

            sqlTool.OnInsertedLock += onInserted;
            sqlTool.OnUpdatedLock += onUpdated;
            sqlTool.OnDeletedLock += onDeleted;

            resetLock();
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected override void reset()
        {
            foreach (valueType value in SqlTool.Where(null, memberMap)) insert(value);
        }
        /// <summary>
        /// 缺少目标数据错误数量
        /// </summary>
        public int MissTargetCount { get; private set; }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        private void insert(valueType value)
        {
            targetType target = getValue(getKey(value));
            if (target == null)
            {
                ++MissTargetCount;
                log.Error.Add("没有找到目标数据 " + typeof(targetType).FullName + "." + getKey(value).ToString(), null, false);
            }
            else
            {
                keyValue<Dictionary<randomKey<memberKeyType>, valueType>, int> cache = getMember(target);
                if (cache.Key == null) cache.Key = dictionary.Create<randomKey<memberKeyType>, valueType>();
                else ++cache.Value;
                setMemberCacheAndValue(value);
                cache.Key.Add(getMemberKey(value), value);
                setMember(target, cache);
                ++Count;
            }
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        private void onInserted(valueType value)
        {
            valueType newValue = fastCSharp.emit.constructor<valueType>.New();
            fastCSharp.emit.memberCopyer<modelType>.Copy(newValue, value, memberMap);
            insert(newValue);
            callOnInserted(newValue);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        /// <param name="memberMap">更新成员位图</param>
        private void onUpdated(valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            Dictionary<randomKey<memberKeyType>, valueType> cache = getMember(getValue(getKey(value))).Key;
            valueType cacheValue;
            if (cache != null && cache.TryGetValue(getMemberKey(value), out cacheValue))
            {
                update(cacheValue, value, oldValue, memberMap);
                callOnUpdated(cacheValue, value, oldValue, memberMap);
            }
            else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        private void onDeleted(valueType value)
        {
            Dictionary<randomKey<memberKeyType>, valueType> cache = getMember(getValue(getKey(value))).Key;
            memberKeyType memberKey = getMemberKey(value);
            valueType cacheValue;
            if (cache != null && cache.TryGetValue(memberKey, out cacheValue))
            {
                cache.Remove(memberKey);
                callOnDeleted(cacheValue);
            }
            else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
    }
}
