using System;
using fastCSharp.code.cSharp;
using System.Collections.Generic;
using System.Threading;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 分组字典缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组字典关键字类型</typeparam>
    /// <typeparam name="valueKeyType">目标数据关键字类型</typeparam>
    /// <typeparam name="targetType">目标数据类型</typeparam>
    public class memberDictionary<valueType, modelType, keyType, valueKeyType, targetType> : member<valueType, modelType, keyType, targetType, Dictionary<randomKey<valueKeyType>, valueType>>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
        where targetType : class
        where valueKeyType : IEquatable<valueKeyType>
    {
        /// <summary>
        /// 获取数据关键字委托
        /// </summary>
        private Func<modelType, valueKeyType> getValueKey;
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueKey"></param>
        /// <returns></returns>
        public valueType this[keyType key, valueKeyType valueKey]
        {
            get
            {
                targetType target = getValue(key);
                if (target != null)
                {
                    Dictionary<randomKey<valueKeyType>, valueType> values = GetMember(target);
                    if (values != null)
                    {
                        valueType value;
                        if (values.TryGetValue(valueKey, out value)) return value;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// 分组字典缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getValue">获取目标对象委托</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="getValueKey">获取数据关键字委托</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
        public memberDictionary(events.cache<valueType, modelType> cache, Func<modelType, keyType> getKey
            , Func<keyType, targetType> getValue, Expression<Func<targetType, Dictionary<randomKey<valueKeyType>, valueType>>> member
            , Func<IEnumerable<targetType>> getTargets, Func<modelType, valueKeyType> getValueKey, bool isReset = true)
            : base(cache, getKey, getValue, member, getTargets)
        {
            if (getValueKey == null) log.Error.Throw(log.exceptionType.Null);
            this.getValueKey = getValueKey;

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
            resetAll();
            foreach (valueType value in cache.Values) onInserted(value);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onInserted(valueType value)
        {
            onInserted(value, getKey(value));
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        /// <param name="key"></param>
        protected void onInserted(valueType value, keyType key)
        {
            targetType target = getValue(key);
            if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
            else
            {
                Dictionary<randomKey<valueKeyType>, valueType> dictionary = GetMember(target);
                if (dictionary == null) setMember(target, dictionary = dictionary<randomKey<valueKeyType>>.Create<valueType>());
                dictionary.Add(getValueKey(value), value);
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        protected void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            keyType oldKey = getKey(oldValue), newKey = getKey(value);
            if (newKey.Equals(oldKey))
            {
                valueKeyType oldValueKey = getValueKey(oldValue), newValueKey = getValueKey(value);
                if (!oldValueKey.Equals(newValueKey))
                {
                    targetType target = getValue(newKey);
                    if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + newKey.ToString(), new System.Diagnostics.StackFrame(), true);
                    else
                    {
                        Dictionary<randomKey<valueKeyType>, valueType> dictionary = GetMember(target);
                        if (dictionary != null)
                        {
                            if (dictionary.Remove(oldValueKey))
                            {
                                dictionary.Add(newValueKey, cacheValue);
                                return;
                            }
                            dictionary.Add(newValueKey, cacheValue);
                        }
                        log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                    }
                }
            }
            else
            {
                onInserted(cacheValue, newKey);
                onDeleted(oldValue, oldKey);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        /// <param name="key">被删除的数据关键字</param>
        protected void onDeleted(valueType value, keyType key)
        {
            targetType target = getValue(key);
            if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
            else
            {
                Dictionary<randomKey<valueKeyType>, valueType> dictionary = GetMember(target);
                if (dictionary == null || !dictionary.Remove(getValueKey(value)))
                {
                    log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                }
            }
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
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ICollection<valueType> GetCache(keyType key)
        {
            targetType target = getValue(key);
            if (target != null)
            {
                Dictionary<randomKey<valueKeyType>, valueType> dictionary = GetMember(target);
                if (dictionary != null) return dictionary.Values;
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取匹配数据数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>匹配数据数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int Count(keyType key)
        {
            return GetCache(key).count();
        }
        /// <summary>
        /// 获取匹配数据数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配数据数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int Count(keyType key, Func<valueType, bool> isValue)
        {
            return GetCache(key).count(isValue);
        }
        /// <summary>
        /// 获取第一个匹配数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>第一个匹配数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType FirstOrDefault(keyType key, Func<valueType, bool> isValue)
        {
            return GetCache(key).firstOrDefault(isValue);
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType[] GetArray(keyType key)
        {
            return GetCache(key).getArray();
        }
        /// <summary>
        /// 获取匹配数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配数据集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType[] GetFindArray(keyType key, Func<valueType, bool> isValue)
        {
            return GetCache(key).getFindArray(isValue);
        }
    }
}