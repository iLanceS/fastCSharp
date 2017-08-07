using System;
using fastCSharp.code.cSharp;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole.events
{
    /// <summary>
    /// 自增ID整表缓存
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    public abstract class identityMemberMap<valueType, modelType, memberCacheType> : key<valueType, modelType, memberCacheType, int>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
    {
        /// <summary>
        /// 基础ID
        /// </summary>
        protected int baseIdentity;
        /// <summary>
        /// 缓存数据数量
        /// </summary>
        public int Count { get; protected set; }
        /// <summary>
        /// SQL操作缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="baseIdentity">基础ID</param>
        /// <param name="group">数据分组</param>
        protected identityMemberMap(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<valueType, memberCacheType>> memberCache, int group, int baseIdentity)
            : base(sqlTool, memberCache, fastCSharp.emit.sqlModel<modelType>.IdentityGetter(baseIdentity), group)
        {
            this.baseIdentity = baseIdentity;
        }

        /// <summary>
        /// 创建自增id标识缓存计数器
        /// </summary>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="group">数据分组</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.part.events.memberIdentityCounter32<valueType, modelType, memberCacheType> CreateCounter
            (Expression<Func<memberCacheType, keyValue<valueType, int>>> member, int group = 1)
        {
            return new fastCSharp.sql.cache.part.events.memberIdentityCounter32<valueType, modelType, memberCacheType>(this, member, group);
        }
        /// <summary>
        /// 创建先进先出优先队列缓存
        /// </summary>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="valueMember">节点成员</param>
        /// <param name="previousMember">前一个节点成员</param>
        /// <param name="nextMember">后一个节点成员</param>
        /// <param name="group">数据分组</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.part.memberQueue<valueType, modelType, memberCacheType, int> CreateCounterMemberQueue
            (Expression<Func<memberCacheType, keyValue<valueType, int>>> member, Expression<Func<memberCacheType, valueType>> valueMember
            , Expression<Func<memberCacheType, memberCacheType>> previousMember, Expression<Func<memberCacheType, memberCacheType>> nextMember, int group = 1, int maxCount = 0)
        {
            return CreateCounter(member, group).CreateMemberQueue(valueMember, previousMember, nextMember, maxCount);
        }

        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="value">待修改数据</param>
        /// <param name="memberMap">需要修改的字段成员位图</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务（不是数据库事务）</param>
        /// <returns>是否修改成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Update(valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction = false)
        {
            valueType cacheLock = this[fastCSharp.emit.sqlModel<modelType>.GetIdentity32(value)];
            return cacheLock != null && SqlTool.Client.UpdateByIdentity(SqlTool, value, memberMap, cacheLock, isIgnoreTransaction);
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="identity">自增id</param>
        /// <param name="updateExpression">SQL表达式</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>修改后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Update(int identity, fastCSharp.emit.sqlTable.updateExpression updateExpression, bool isIgnoreTransaction = false)
        {
            return Update(identity, ref updateExpression, isIgnoreTransaction);
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="identity">自增id</param>
        /// <param name="updateExpression">SQL表达式</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>修改后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Update(int identity, ref fastCSharp.emit.sqlTable.updateExpression updateExpression, bool isIgnoreTransaction = false)
        {
            valueType cacheLock = this[identity];
            if (cacheLock != null && updateExpression.Count != 0)
            {
                valueType value = fastCSharp.emit.constructor<valueType>.New();
                fastCSharp.emit.sqlModel<modelType>.SetIdentity(value, identity);
                if (SqlTool.Client.UpdateByIdentity(SqlTool, value, ref updateExpression, cacheLock, isIgnoreTransaction)) return value;
            }
            return null;
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="identity">自增id</param>
        /// <param name="expression">SQL表达式</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>修改后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Update<returnType>(int identity, Expression<Func<modelType, returnType>> expression, bool isIgnoreTransaction = false)
        {
            return expression != null ? Update(identity, SqlTool.UpdateExpression(expression), isIgnoreTransaction) : null;
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="identity">自增id</param>
        /// <param name="expression">字段表达式</param>
        /// <param name="returnValue">更新字段值</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>修改后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Update<returnType>(int identity, Expression<Func<modelType, returnType>> expression, returnType returnValue, bool isIgnoreTransaction = false)
        {
            return expression != null ? Update(identity, SqlTool.UpdateExpression(expression, returnValue), isIgnoreTransaction) : null;
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="identity">自增id</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="expressions">SQL表达式</param>
        /// <returns>修改后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Update<returnType>(int identity, bool isIgnoreTransaction, params Expression<Func<modelType, returnType>>[] expressions)
        {
            return expressions.Length != 0 ? Update(identity, SqlTool.UpdateExpression(expressions), isIgnoreTransaction) : null;
        }
        /// <summary>
        /// 删除数据库记录
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Delete(int identity, bool isIgnoreTransaction = false)
        {
            valueType value = this[identity];
            return value != null && SqlTool.Client.DeleteByIdentity(SqlTool, value, true, isIgnoreTransaction);
        }
        /// <summary>
        /// 删除数据库记录
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="isDelete">是否需要删除</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Delete(int identity, Func<valueType, bool> isDelete, bool isIgnoreTransaction = false)
        {
            valueType value = this[identity];
            return value != null && isDelete(value) && SqlTool.Client.DeleteByIdentity(SqlTool, value, true, isIgnoreTransaction);
        }
    }
}
