using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using fastCSharp.code.cSharp;
using System.Reflection;

namespace fastCSharp.sql.cache.whole.events
{
    /// <summary>
    /// 事件缓存
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    public abstract class cache<valueType, modelType> : copy<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        public abstract IEnumerable<valueType> Values { get; }
        /// <summary>
        /// 添加记录事件
        /// </summary>
        public event Action<valueType> OnInserted;
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="value">新添加的对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void callOnInserted(valueType value)
        {
            if (OnInserted != null) OnInserted(value);
        }
        /// <summary>
        /// 更新记录事件
        /// </summary>
        public event Action<valueType, valueType, valueType, fastCSharp.code.memberMap<modelType>> OnUpdated;
        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="cacheValue"></param>
        /// <param name="value">更新后的对象</param>
        /// <param name="oldValue">更新前的对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void callOnUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            if (OnUpdated != null) OnUpdated(cacheValue, value, oldValue, memberMap);
        }
        /// <summary>
        /// 删除记录事件
        /// </summary>
        public event Action<valueType> OnDeleted;
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="value">被删除的对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void callOnDeleted(valueType value)
        {
            if (OnDeleted != null) OnDeleted(value);
        }
        /// <summary>
        /// SQL操作缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="group">数据分组</param>
        protected cache(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, int group) : base(sqlTool, group) { }
        /// <summary>
        /// 获取所有缓存数据
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<valueType> getAllValue()
        {
            return Values;
        }
        /// <summary>
        /// 缓存数据加载完成
        /// </summary>
        /// <param name="onInserted">添加记录事件</param>
        /// <param name="onUpdated">更新记录事件</param>
        /// <param name="onDeleted">删除记录事件</param>
        /// <param name="isSqlStreamTypeCount">是否日志流计数完成类型注册</param>
        public void Loaded(Action<valueType> onInserted = null, Action<valueType, valueType, valueType, fastCSharp.code.memberMap<modelType>> onUpdated = null, Action<valueType> onDeleted = null, bool isSqlStreamTypeCount = true)
        {
            if (onInserted != null) OnInserted += onInserted;
            if (onUpdated != null) OnUpdated += onUpdated;
            if (onDeleted != null) OnDeleted += onDeleted;
            SqlTool.LoadWait.Set();
            if (isSqlStreamTypeCount) fastCSharp.emit.sqlTable.sqlStreamCountLoaderType.Add(typeof(modelType), SqlTool.TableNumber);
        }
        /// <summary>
        /// 获取日志流
        /// </summary>
        /// <param name="memberCount">成员加载计数器</param>
        /// <param name="isMemberMap">是否支持成员位图</param>
        /// <returns></returns>
        public fastCSharp.sql.logStream<valueType, modelType> GetLogStream(fastCSharp.sql.logStream.memberCount memberCount = null, bool isMemberMap = true)
        {
            return new fastCSharp.sql.logStream<valueType, modelType>(this, memberCount, isMemberMap);
        }
        /// <summary>
        /// 获取日志流
        /// </summary>
        /// <param name="memberCount">成员加载计数器</param>
        /// <param name="isMemberMap">是否支持成员位图</param>
        /// <returns></returns>
        public fastCSharp.sql.logStream<valueType, modelType>.where GetLogStream(Func<modelType, bool> isValue, fastCSharp.sql.logStream.memberCount memberCount = null, bool isMemberMap = true)
        {
            return new fastCSharp.sql.logStream<valueType, modelType>.where(this, memberCount, isMemberMap, isValue);
        }
        /// <summary>
        /// 成员绑定缓存集合
        /// </summary>
        protected subArray<object> memberCaches;
        ///// <summary>
        ///// 创建分组列表缓存
        ///// </summary>
        ///// <typeparam name="keyType">分组字典关键字类型</typeparam>
        ///// <typeparam name="targetType">目标数据类型</typeparam>
        ///// <param name="getKey">分组字典关键字获取器</param>
        ///// <param name="getValue">获取目标对象委托</param>
        ///// <param name="member">缓存字段表达式</param>
        ///// <param name="getTargets">获取缓存目标对象集合</param>
        ///// <param name="isRemoveEnd">移除数据并使用最后一个数据移动到当前位置</param>
        ///// <param name="isReset">是否绑定事件并重置数据</param>
        ///// <returns></returns>
        //public memberList<valueType, modelType, keyType, targetType> CreateMemberList<keyType, targetType>(Func<modelType, keyType> getKey
        //    , Func<keyType, targetType> getValue, Expression<Func<targetType, list<valueType>>> member
        //    , Func<IEnumerable<targetType>> getTargets, bool isRemoveEnd = false, bool isReset = true)
        //    where keyType : IEquatable<keyType>
        //    where targetType : class
        //{
        //    new memberList<valueType, modelType, keyType, targetType>(this, getKey, getValue, member, getTargets, isRemoveEnd, isReset);
        //}
        /// <summary>
        /// 创建分组列表缓存
        /// </summary>
        /// <typeparam name="keyType">分组字典关键字类型</typeparam>
        /// <typeparam name="targetType">目标表格类型</typeparam>
        /// <typeparam name="targetModelType">目标模型类型</typeparam>
        /// <typeparam name="targetMemberCacheType">目标缓存绑定类型</typeparam>
        /// <param name="targetCache">目标缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="isRemoveEnd">移除数据并使用最后一个数据移动到当前位置</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public memberList<valueType, modelType, keyType, targetMemberCacheType> CreateMemberList<keyType, targetType, targetModelType, targetMemberCacheType>
            (key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<modelType, keyType> getKey
            , Expression<Func<targetMemberCacheType, list<valueType>>> member
            , bool isRemoveEnd = false, bool isReset = true, bool isSave = true)
            where keyType : IEquatable<keyType>
            where targetType : class, targetModelType
            where targetModelType : class
            where targetMemberCacheType : class
        {
            memberList<valueType, modelType, keyType, targetMemberCacheType> cache = new memberList<valueType, modelType, keyType, targetMemberCacheType>(this, getKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, isRemoveEnd, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建分组列表缓存
        /// </summary>
        /// <typeparam name="keyType">分组字典关键字类型</typeparam>
        /// <typeparam name="targetType">目标表格类型</typeparam>
        /// <typeparam name="targetModelType">目标模型类型</typeparam>
        /// <typeparam name="targetMemberCacheType">目标缓存绑定类型</typeparam>
        /// <param name="targetCache">目标缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="isRemoveEnd">移除数据并使用最后一个数据移动到当前位置</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public memberListWhere<valueType, modelType, keyType, targetMemberCacheType> CreateMemberListWhere<keyType, targetType, targetModelType, targetMemberCacheType>
            (key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<modelType, keyType> getKey
            , Expression<Func<targetMemberCacheType, list<valueType>>> member, Func<valueType, bool> isValue
            , bool isRemoveEnd = false, bool isSave = true)
            where keyType : IEquatable<keyType>
            where targetType : class, targetModelType
            where targetModelType : class
            where targetMemberCacheType : class
        {
            memberListWhere<valueType, modelType, keyType, targetMemberCacheType> cache = new memberListWhere<valueType, modelType, keyType, targetMemberCacheType>(this, getKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, isValue, isRemoveEnd);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 分组列表 延时排序缓存
        /// </summary>
        /// <typeparam name="keyType">分组字典关键字类型</typeparam>
        /// <typeparam name="targetType">目标表格类型</typeparam>
        /// <typeparam name="targetModelType">目标模型类型</typeparam>
        /// <typeparam name="targetMemberCacheType">目标缓存绑定类型</typeparam>
        /// <param name="targetCache">目标缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="sorter">数据排序</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public memberLadyOrderArray<valueType, modelType, keyType, targetMemberCacheType> CreateMemberLadyOrderArray<keyType, targetType, targetModelType, targetMemberCacheType>
            (key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<modelType, keyType> getKey
            , Expression<Func<targetMemberCacheType, ladyOrderArray<valueType>>> member, Func<subArray<valueType>, subArray<valueType>> sorter
            , bool isSave = false)
            where keyType : IEquatable<keyType>
            where targetType : class, targetModelType
            where targetModelType : class
            where targetMemberCacheType : class
        {
            memberLadyOrderArray<valueType, modelType, keyType, targetMemberCacheType> cache = new memberLadyOrderArray<valueType, modelType, keyType, targetMemberCacheType>(this, getKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, sorter, true);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 分组列表 延时排序缓存
        /// </summary>
        /// <typeparam name="keyType">分组字典关键字类型</typeparam>
        /// <typeparam name="targetType">目标表格类型</typeparam>
        /// <typeparam name="targetModelType">目标模型类型</typeparam>
        /// <typeparam name="targetMemberCacheType">目标缓存绑定类型</typeparam>
        /// <param name="targetCache">目标缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="sorter">数据排序</param>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public memberLadyOrderArrayWhere<valueType, modelType, keyType, targetMemberCacheType> CreateMemberLadyOrderArrayWhere<keyType, targetType, targetModelType, targetMemberCacheType>
            (key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<modelType, keyType> getKey
            , Expression<Func<targetMemberCacheType, ladyOrderArray<valueType>>> member, Func<subArray<valueType>, subArray<valueType>> sorter, Func<valueType, bool> isValue
            , bool isSave = false)
            where keyType : IEquatable<keyType>
            where targetType : class, targetModelType
            where targetModelType : class
            where targetMemberCacheType : class
        {
            memberLadyOrderArrayWhere<valueType, modelType, keyType, targetMemberCacheType> cache = new memberLadyOrderArrayWhere<valueType, modelType, keyType, targetMemberCacheType>(this, getKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, sorter, isValue);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建分组列表 延时排序缓存
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="targetType"></typeparam>
        /// <typeparam name="targetModelType"></typeparam>
        /// <typeparam name="targetMemberCacheType"></typeparam>
        /// <param name="targetCache">目标缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="sorter">排序器</param>
        /// <param name="isReset">是否初始化</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.whole.memberOrderList<valueType, modelType, keyType, targetMemberCacheType> CreateMemberOrderList<keyType, targetType, targetModelType, targetMemberCacheType>
            (key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<modelType, keyType> getKey, Expression<Func<targetMemberCacheType, list<valueType>>> member
            , Func<subArray<valueType>, subArray<valueType>> sorter, bool isReset, bool isSave = false)
            where keyType : IEquatable<keyType>
            where targetType : class, targetModelType
            where targetModelType : class
            where targetMemberCacheType : class
        {
            fastCSharp.sql.cache.whole.memberOrderList<valueType, modelType, keyType, targetMemberCacheType> cache = new fastCSharp.sql.cache.whole.memberOrderList<valueType, modelType, keyType, targetMemberCacheType>(this, getKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, sorter, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建分组字典缓存
        /// </summary>
        /// <typeparam name="keyType">分组字典关键字类型</typeparam>
        /// <typeparam name="targetType">目标表格类型</typeparam>
        /// <typeparam name="targetModelType">目标模型类型</typeparam>
        /// <typeparam name="targetMemberCacheType">目标缓存绑定类型</typeparam>
        /// <typeparam name="valueKeyType">目标数据关键字类型</typeparam>
        /// <param name="targetCache">目标缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="getValueKey">获取数据关键字委托</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public memberDictionary<valueType, modelType, keyType, valueKeyType, targetMemberCacheType> CreateMemberDictionary<keyType, targetType, targetModelType, targetMemberCacheType, valueKeyType>
            (key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<modelType, keyType> getKey
            , Expression<Func<targetMemberCacheType, Dictionary<randomKey<valueKeyType>, valueType>>> member
            , Func<modelType, valueKeyType> getValueKey, bool isReset = true, bool isSave = true)
            where keyType : IEquatable<keyType>
            where targetType : class, targetModelType
            where targetModelType : class
            where targetMemberCacheType : class
            where valueKeyType : IEquatable<valueKeyType>
        {
            memberDictionary<valueType, modelType, keyType, valueKeyType, targetMemberCacheType> cache = new memberDictionary<valueType, modelType, keyType, valueKeyType, targetMemberCacheType>(this, getKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, getValueKey, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建分组列表缓存
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="targetType"></typeparam>
        /// <typeparam name="targetModelType"></typeparam>
        /// <typeparam name="targetMemberCacheType"></typeparam>
        /// <typeparam name="memberKeyType"></typeparam>
        /// <param name="targetCache">目标缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getMemberKey">分组列表关键字获取器</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="isRemoveEnd">移除数据并使用最后一个数据移动到当前位置</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public memberDictionaryListWhere<valueType, modelType, keyType, memberKeyType, targetMemberCacheType> CreateMemberDictionaryListWhere<keyType, targetType, targetModelType, targetMemberCacheType, memberKeyType>
            (key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<modelType, keyType> getKey, Func<modelType, memberKeyType> getMemberKey
            , Expression<Func<targetMemberCacheType, Dictionary<randomKey<memberKeyType>, list<valueType>>>> member, Func<valueType, bool> isValue, bool isRemoveEnd, bool isSave = true)
            where keyType : struct, IEquatable<keyType>
            where targetType : class, targetModelType
            where targetModelType : class
            where targetMemberCacheType : class
            where memberKeyType : struct, IEquatable<memberKeyType>
        {
            memberDictionaryListWhere<valueType, modelType, keyType, memberKeyType, targetMemberCacheType> cache = new memberDictionaryListWhere<valueType, modelType, keyType, memberKeyType, targetMemberCacheType>(this, getKey, getMemberKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, isValue, isRemoveEnd);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建分组列表 延时排序缓存
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="targetType"></typeparam>
        /// <typeparam name="targetModelType"></typeparam>
        /// <typeparam name="memberKeyType"></typeparam>
        /// <param name="targetCache">目标缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getIndex">获取数组索引</param>
        /// <param name="arraySize">数组容器大小</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="sorter">排序器</param>
        /// <param name="isReset">是否初始化</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.whole.memberArrayLadyOrderArray<valueType, modelType, keyType, targetMemberCacheType> CreateMemberArrayLadyOrderArray<keyType, targetType, targetModelType, targetMemberCacheType>
            (key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<modelType, keyType> getKey
            , Func<valueType, int> getIndex, int arraySize, Expression<Func<targetMemberCacheType, ladyOrderArray<valueType>[]>> member
            , Func<subArray<valueType>, subArray<valueType>> sorter, bool isReset = true, bool isSave = false)
            where keyType : IEquatable<keyType>
            where targetType : class, targetModelType
            where targetModelType : class
            where targetMemberCacheType : class
        {
            fastCSharp.sql.cache.whole.memberArrayLadyOrderArray<valueType, modelType, keyType, targetMemberCacheType> cache = new fastCSharp.sql.cache.whole.memberArrayLadyOrderArray<valueType, modelType, keyType, targetMemberCacheType>(this, getKey, targetCache.GetMemberCacheByKey, getIndex, arraySize, member, targetCache.GetAllMemberCache, sorter, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建分组列表缓存
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="targetType"></typeparam>
        /// <typeparam name="targetModelType"></typeparam>
        /// <typeparam name="targetMemberCacheType"></typeparam>
        /// <param name="targetCache">目标缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getIndex">获取数组索引</param>
        /// <param name="arraySize">数组容器大小</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="isRemoveEnd">移除数据并使用最后一个数据移动到当前位置</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.whole.memberArrayList<valueType, modelType, keyType, targetMemberCacheType> CreateMemberArrayList<keyType, targetType, targetModelType, targetMemberCacheType>
            (key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<modelType, keyType> getKey, Func<valueType, int> getIndex, int arraySize
            , Expression<Func<targetMemberCacheType, list<valueType>[]>> member, bool isRemoveEnd = false, bool isReset = true, bool isSave = true)
            where keyType : struct, IEquatable<keyType>
            where targetType : class, targetModelType
            where targetModelType : class
            where targetMemberCacheType : class
        {
            fastCSharp.sql.cache.whole.memberArrayList<valueType, modelType, keyType, targetMemberCacheType> cache = new fastCSharp.sql.cache.whole.memberArrayList<valueType, modelType, keyType, targetMemberCacheType>(this, getKey, getIndex, arraySize, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, isRemoveEnd, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }

        /// <summary>
        /// 创建关键字整表缓存
        /// </summary>
        /// <typeparam name="primaryKey"></typeparam>
        /// <param name="getValue">根据关键字获取数据</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="group">数据分组</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public memberKey<valueType, modelType, primaryKey> CreateMemberPrimaryKey<primaryKey>(Func<primaryKey, valueType> getValue, Expression<Func<valueType, valueType>> member, int group = 1, bool isSave = true)
            where primaryKey : struct, IEquatable<primaryKey>
        {
            memberKey<valueType, modelType, primaryKey> cache = new memberKey<valueType, modelType, primaryKey>(this, ((fastCSharp.emit.sqlTable<valueType, modelType, primaryKey>)SqlTool).GetPrimaryKey, getValue, member, group);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建分组字典缓存
        /// </summary>
        /// <typeparam name="groupKeyType"></typeparam>
        /// <typeparam name="keyType"></typeparam>
        /// <param name="getGroupKey">分组关键字获取器</param>
        /// <param name="getKey">字典关键字获取器</param>
        /// <param name="isReset">是否初始化数据</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public dictionaryDictionary<valueType, modelType, groupKeyType, keyType> CreateDictionaryDictionary<groupKeyType, keyType>(Func<valueType, groupKeyType> getGroupKey, Func<valueType, keyType> getKey, bool isReset = true, bool isSave = false)
            where groupKeyType : IEquatable<groupKeyType>
            where keyType : IEquatable<keyType>
        {
            dictionaryDictionary<valueType, modelType, groupKeyType, keyType> cache = new dictionaryDictionary<valueType, modelType, groupKeyType, keyType>(this, getGroupKey, getKey, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建数组列表缓存
        /// </summary>
        /// <param name="getIndex">数组索引获取器</param>
        /// <param name="arraySize">数组容器大小</param>
        /// <param name="isRemoveEnd">移除数据并使用最后一个数据移动到当前位置</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.whole.arrayList<valueType, modelType> CreateArrayList(Func<valueType, int> getIndex, int arraySize, bool isRemoveEnd = false, bool isReset = true, bool isSave = false)
        {
            fastCSharp.sql.cache.whole.arrayList<valueType, modelType> cache = new fastCSharp.sql.cache.whole.arrayList<valueType, modelType>(this, getIndex, arraySize, isRemoveEnd, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建字典缓存
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="isReset">是否初始化</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.whole.dictionary<valueType, modelType, keyType> CreateDictionary<keyType>(Func<valueType, keyType> getKey, bool isReset = true, bool isSave = false)
            where keyType : IEquatable<keyType>
        {
            fastCSharp.sql.cache.whole.dictionary<valueType, modelType, keyType> cache = new fastCSharp.sql.cache.whole.dictionary<valueType, modelType, keyType>(this, getKey, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建字典缓存
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.whole.dictionaryWhere<valueType, modelType, keyType> CreateDictionaryWhere<keyType>(Func<valueType, keyType> getKey, Func<valueType, bool> isValue, bool isSave = false)
            where keyType : IEquatable<keyType>
        {
            fastCSharp.sql.cache.whole.dictionaryWhere<valueType, modelType, keyType> cache = new fastCSharp.sql.cache.whole.dictionaryWhere<valueType, modelType, keyType>(this, getKey, isValue);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建缓存时间事件
        /// </summary>
        /// <param name="getTime">时间获取器</param>
        /// <param name="run">事件委托</param>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.whole.timerWhere<valueType, modelType> CreateTimerWhere(Func<valueType, DateTime> getTime, Action run, Func<valueType, bool> isValue, bool isSave = false)
        {
            fastCSharp.sql.cache.whole.timerWhere<valueType, modelType> cache = new fastCSharp.sql.cache.whole.timerWhere<valueType, modelType>(this, getTime, run, isValue);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建字典+搜索树缓存
        /// </summary>
        /// <typeparam name="sortType"></typeparam>
        /// <param name="getIndex">数组索引获取器</param>
        /// <param name="arraySize">数组容器大小</param>
        /// <param name="getSort">排序关键字获取器</param>
        /// <param name="isReset">是否初始化</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.whole.arraySearchTree<valueType, modelType, sortType> CreateArraySearchTree<sortType>(Func<valueType, int> getIndex, int arraySize, Func<valueType, sortType> getSort, bool isReset = true, bool isSave = false)
            where sortType : IComparable<sortType>
        {
            fastCSharp.sql.cache.whole.arraySearchTree<valueType, modelType, sortType> cache = new fastCSharp.sql.cache.whole.arraySearchTree<valueType, modelType, sortType>(this, getIndex, arraySize, getSort, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建字典+搜索树缓存
        /// </summary>
        /// <typeparam name="sortType"></typeparam>
        /// <param name="getIndex">数组索引获取器</param>
        /// <param name="arraySize">数组容器大小</param>
        /// <param name="getSort">排序关键字获取器</param>
        /// <param name="isValue">缓存值判定</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.whole.arraySearchTreeWhere<valueType, modelType, sortType> CreateArraySearchTreeWhere<sortType>(Func<valueType, int> getIndex, int arraySize, Func<valueType, sortType> getSort, Func<valueType, bool> isValue, bool isSave = false)
            where sortType : IComparable<sortType>
        {
            fastCSharp.sql.cache.whole.arraySearchTreeWhere<valueType, modelType, sortType> cache = new fastCSharp.sql.cache.whole.arraySearchTreeWhere<valueType, modelType, sortType>(this, getIndex, arraySize, getSort, isValue);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
        /// <summary>
        /// 创建分组列表 延时排序缓存
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="sorter">排序器</param>
        /// <param name="isReset">是否初始化</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.whole.dictionaryListOrderLady<valueType, modelType, keyType> CreateDictionaryListOrderLady<keyType>(Func<modelType, keyType> getKey, Func<list<valueType>, subArray<valueType>> sorter, bool isReset = true, bool isSave = false)
            where keyType : IEquatable<keyType>
        {
            fastCSharp.sql.cache.whole.dictionaryListOrderLady<valueType, modelType, keyType> cache = new fastCSharp.sql.cache.whole.dictionaryListOrderLady<valueType, modelType, keyType>(this, getKey, sorter, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }

        /// <summary>
        /// 创建搜索树缓存
        /// </summary>
        /// <typeparam name="sortType">排序关键字类型</typeparam>
        /// <param name="getSort">排序关键字获取器</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns></returns>
        public searchTreeWhere<valueType, modelType, sortType> CreateSearchTreeWhere<sortType>(Func<valueType, sortType> getSort, Func<valueType, bool> isValue)
            where sortType : IComparable<sortType>
        {
            return new searchTreeWhere<valueType, modelType, sortType>(this, getSort, isValue);
        }
        /// <summary>
        /// 创建搜索树缓存
        /// </summary>
        /// <typeparam name="sortType1">排序关键字类型</typeparam>
        /// <typeparam name="sortType2">排序关键字类型</typeparam>
        /// <param name="getSort1">排序关键字获取器</param>
        /// <param name="getSort2">排序关键字获取器</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns></returns>
        public searchTreeWhere<valueType, modelType, fastCSharp.data.primaryKey<sortType1, sortType2>> CreateSearchTreeWhere<sortType1, sortType2>(Func<valueType, sortType1> getSort1, Func<valueType, sortType2> getSort2, Func<valueType, bool> isValue)
            where sortType1 : IEquatable<sortType1>, IComparable<sortType1>
            where sortType2 : IEquatable<sortType2>, IComparable<sortType2>
        {
            return new searchTreeWhere<valueType, modelType, fastCSharp.data.primaryKey<sortType1, sortType2>>(this, value => new fastCSharp.data.primaryKey<sortType1, sortType2> { Key1 = getSort1(value), Key2 = getSort2(value) }, isValue);
        }
    }
    /// <summary>
    /// 事件缓存
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    public abstract class cache<valueType, modelType, memberCacheType> : cache<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
    {
        /// <summary>
        /// 获取所有缓存数据
        /// </summary>
        public readonly Func<IEnumerable<valueType>> GetAllValue;
        /// <summary>
        /// 获取成员缓存
        /// </summary>
        public readonly Func<valueType, memberCacheType> GetMemberCache;
        /// <summary>
        /// 设置成员缓存
        /// </summary>
        private Action<valueType, memberCacheType> setMemberCache;
        /// <summary>
        /// 设置成员缓存数据
        /// </summary>
        private Action<memberCacheType, valueType> setMemberCacheValue;
        /// <summary>
        /// 获取所有成员缓存
        /// </summary>
        public readonly Func<IEnumerable<memberCacheType>> GetAllMemberCache;
        /// <summary>
        /// SQL操作缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="group">数据分组</param>
        protected cache(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<valueType, memberCacheType>> memberCache, int group)
            : base(sqlTool, group)
        {
            if (group == 0)
            {
                if (memberCache == null)
                {
                    if (typeof(valueType) != typeof(memberCacheType)) log.Default.Throw(log.exceptionType.Null);
                    GetAllValue = getAllValue;
                }
                else
                {
                    if (fastCSharp.emit.constructor<memberCacheType>.New == null) log.Default.Throw("找不到无参构造函数 " + typeof(memberCacheType).FullName, null, false);
                    memberExpression<valueType, memberCacheType> expression = new memberExpression<valueType, memberCacheType>(memberCache);
                    if (expression.Field == null) log.Error.Throw(log.exceptionType.ErrorOperation);
                    GetMemberCache = expression.GetMember;
                    setMemberCache = expression.SetMember;
                    GetAllMemberCache = getAllMemberCache;
                    setMemberCacheValue = fastCSharp.emit.pub.UnsafeSetField<memberCacheType, valueType>("Value");
                }
            }
        }
        /// <summary>
        /// 设置成员缓存与数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void setMemberCacheAndValue(valueType value)
        {
            if (setMemberCache != null)
            {
                if (setMemberCacheValue == null) setMemberCache(value, fastCSharp.emit.constructor<memberCacheType>.New());
                else
                {
                    memberCacheType memberCache = fastCSharp.emit.constructor<memberCacheType>.New();
                    setMemberCache(value, memberCache);
                    setMemberCacheValue(memberCache, value);
                }
            }
        }
        /// <summary>
        /// 所有成员缓存
        /// </summary>
        private IEnumerable<memberCacheType> getAllMemberCache()
        {
            foreach (valueType value in Values)
            {
                yield return GetMemberCache(value);
            }
        }
    }
}
