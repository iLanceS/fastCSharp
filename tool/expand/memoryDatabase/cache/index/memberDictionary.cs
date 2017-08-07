using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using fastCSharp.code;
using System.Runtime.CompilerServices;

namespace fastCSharp.memoryDatabase.cache.index
{
    /// <summary>
    /// 分组字典缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组字典关键字类型</typeparam>
    /// <typeparam name="valueKeyType">目标数据关键字类型</typeparam>
    /// <typeparam name="targetType">目标数据类型</typeparam>
    public class memberDictionary<valueType, modelType, keyType, valueKeyType, targetType> : member<valueType, modelType, keyType, targetType, Dictionary<valueKeyType, valueType>>
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
        /// 分组字典缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getValue">获取目标对象委托</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="getValueKey">获取数据关键字委托</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
#if NOJIT
        public memberDictionary(ILoadCache cache, Func<modelType, keyType> getKey, Func<keyType, targetType> getValue, Expression<Func<targetType, Dictionary<valueKeyType, valueType>>> member, Func<modelType, valueKeyType> getValueKey, bool isReset = true)
#else
        public memberDictionary(ILoadCache<valueType, modelType> cache, Func<modelType, keyType> getKey, Func<keyType, targetType> getValue, Expression<Func<targetType, Dictionary<valueKeyType, valueType>>> member, Func<modelType, valueKeyType> getValueKey, bool isReset = true)
#endif
            : base(cache, getKey, getValue, member)
        {
            if (getValueKey == null) log.Error.Throw(log.exceptionType.Null);
            this.getValueKey = getValueKey;

            if (isReset)
            {
                cache.OnInserted += onInserted;
                cache.OnUpdated += onUpdated;
                cache.OnDeleted += onDeleted;
                reset();
            }
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected virtual void reset()
        {
            cache.WaitLoad();
            foreach (valueType value in cache.Values) onInserted(value);
        }
#if NOJIT
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        private void onInserted(object value)
        {
            onInserted((valueType)value);
        }
#endif
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
                Dictionary<valueKeyType, valueType> dictionary = getMember(target);
                if (dictionary == null) setMember(target, dictionary = fastCSharp.dictionary<valueKeyType>.Create<valueType>());
                dictionary.Add(getValueKey(value), value);
            }
        }
#if NOJIT
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        /// <param name="memberMap">更新数据成员</param>
        private void onUpdated(object value, object oldValue, memberMap memberMap)
        {
            onUpdated((valueType)value, (valueType)oldValue, memberMap);
        }
#endif
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        /// <param name="memberMap">更新数据成员</param>
        protected void onUpdated(valueType value, valueType oldValue, memberMap memberMap)
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
                        Dictionary<valueKeyType, valueType> dictionary = getMember(target);
                        if (dictionary != null)
                        {
                            if (dictionary.Remove(oldValueKey))
                            {
                                dictionary.Add(newValueKey, value);
                                return;
                            }
                            dictionary.Add(newValueKey, value);
                        }
                        log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                    }
                }
            }
            else
            {
                onInserted(value, newKey);
                onDeleted(value, oldKey);
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
                Dictionary<valueKeyType, valueType> dictionary = getMember(target);
                if (dictionary == null || !dictionary.Remove(getValueKey(value)))
                {
                    log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                }
            }
        }
#if NOJIT
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        private void onDeleted(object value)
        {
            onDeleted((valueType)value);
        }
#endif
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
                Dictionary<valueKeyType, valueType> dictionary = getMember(target);
                if (dictionary != null) return dictionary.Values;
            }
            return nullValue<valueType>.Array;
        }
    }
    /// <summary>
    /// 分组字典缓存
    /// </summary>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组字典关键字类型</typeparam>
    /// <typeparam name="valueKeyType">目标数据关键字类型</typeparam>
    /// <typeparam name="targetType">目标数据类型</typeparam>
    public class memberDictionary<modelType, keyType, valueKeyType, targetType> : memberDictionary<modelType, modelType, keyType, valueKeyType, targetType>
        where modelType : class
        where keyType : IEquatable<keyType>
        where targetType : class
        where valueKeyType : IEquatable<valueKeyType>
    {
        /// <summary>
        /// 分组字典缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getValue">获取目标对象委托</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="getValueKey">获取数据关键字委托</param>
#if NOJIT
        public memberDictionary(ILoadCache cache, Func<modelType, keyType> getKey, Func<keyType, targetType> getValue, Expression<Func<targetType, Dictionary<valueKeyType, modelType>>> member, Func<modelType, valueKeyType> getValueKey)
#else
        public memberDictionary(ILoadCache<modelType, modelType> cache, Func<modelType, keyType> getKey, Func<keyType, targetType> getValue, Expression<Func<targetType, Dictionary<valueKeyType, modelType>>> member, Func<modelType, valueKeyType> getValueKey)
#endif
            : base(cache, getKey, getValue, member, getValueKey, true)
        {
        }
    }
}
