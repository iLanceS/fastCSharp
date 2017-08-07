using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 分组列表缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组字典关键字类型</typeparam>
    /// <typeparam name="memberKeyType"></typeparam>
    /// <typeparam name="targetType"></typeparam>
    public class memberDictionaryList<valueType, modelType, keyType, memberKeyType, targetType>
        : member<valueType, modelType, keyType, targetType, Dictionary<randomKey<memberKeyType>, list<valueType>>>
        where valueType : class, modelType
        where modelType : class
        where keyType : struct, IEquatable<keyType>
        where memberKeyType : struct, IEquatable<memberKeyType>
        where targetType : class
    {
        /// <summary>
        /// 关键字获取器
        /// </summary>
        protected Func<modelType, memberKeyType> getMemberKey;
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
        public memberDictionaryList(events.cache<valueType, modelType> cache
            , Func<modelType, keyType> getKey, Func<modelType, memberKeyType> getMemberKey
            , Func<keyType, targetType> getValue, Expression<Func<targetType, Dictionary<randomKey<memberKeyType>, list<valueType>>>> member
            , Func<IEnumerable<targetType>> getTargets, bool isRemoveEnd, bool isReset)
            : base(cache, getKey, getValue, member, getTargets)
        {
            if (getMemberKey == null) log.Error.Throw(log.exceptionType.Null);
            this.getMemberKey = getMemberKey;
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
        private void onInserted(valueType value, keyType key)
        {
            targetType target = getValue(key);
            if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
            else
            {
                memberKeyType memberKey = getMemberKey(value);
                Dictionary<randomKey<memberKeyType>, list<valueType>> dictionary = GetMember(target);
                list<valueType> list;
                if (dictionary == null)
                {
                    setMember(target, dictionary = dictionary<randomKey<memberKeyType>>.Create<list<valueType>>());
                    dictionary.Add(memberKey, list = new list<valueType>());
                }
                else if (!dictionary.TryGetValue(memberKey, out list)) dictionary.Add(memberKey, list = new list<valueType>());
                list.Add(value);
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        protected void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            keyType key = getKey(value), oldKey = getKey(oldValue);
            if (!key.Equals(oldKey))
            {
                onInserted(cacheValue, key);
                onDeleted(cacheValue, oldKey, getMemberKey(oldValue));
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        /// <param name="key">被删除的数据关键字</param>
        protected void onDeleted(valueType value, keyType key, memberKeyType memberKey)
        {
            targetType target = getValue(key);
            if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
            else
            {
                Dictionary<randomKey<memberKeyType>, list<valueType>> dictionary = GetMember(target);
                if (dictionary != null)
                {
                    list<valueType> list;
                    if (dictionary.TryGetValue(memberKey, out list))
                    {
                        int index = Array.LastIndexOf(list.UnsafeArray, value, list.Count - 1);
                        if (index != -1)
                        {
                            if (list.Count != 1)
                            {
                                if (isRemoveEnd) list.RemoveAtEnd(index);
                                else list.RemoveAt(index);
                            }
                            else dictionary.Remove(memberKey);
                            return;
                        }
                    }
                }
                log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onDeleted(valueType value)
        {
            onDeleted(value, getKey(value), getMemberKey(value));
        }
    }
}
