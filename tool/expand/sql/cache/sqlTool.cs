using System;
using fastCSharp.code.cSharp;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache
{
    /// <summary>
    /// SQL操作缓存
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    public abstract class sqlTool<valueType, modelType> : IDisposable
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// SQL操作工具
        /// </summary>
        public fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> SqlTool { get; private set; }
        ///// <summary>
        ///// SQL访问锁
        ///// </summary>
        //public object Lock
        //{
        //    get { return SqlTool.Lock; }
        //}
        /// <summary>
        /// 
        /// </summary>
        protected fastCSharp.code.memberMap<modelType> memberMap;
        /// <summary>
        /// 数据成员位图
        /// </summary>
        internal fastCSharp.code.memberMap<modelType> MemberMap
        {
            get { return memberMap; }
        }
        /// <summary>
        /// 成员分组
        /// </summary>
        protected readonly int memberGroup;
        /// <summary>
        /// SQL操作缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="group">数据分组</param>
        protected sqlTool(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, int group)
        {
            if (sqlTool == null) log.Error.Throw(log.exceptionType.Null);
            memberGroup = group;
            SqlTool = sqlTool;
            memberMap = fastCSharp.emit.sqlModel<modelType>.GetCacheMemberMap(group);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            pub.Dispose(ref memberMap);
        }
        ///// <summary>
        ///// 等待缓存加载
        ///// </summary>
        //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        //public void LoadWait()
        //{
        //    SqlTool.LoadWait.Wait();
        //}
        /// <summary>
        /// 对象成员复制
        /// </summary>
        /// <param name="value">目标对象</param>
        /// <param name="readValue">被复制对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void CopyMember(valueType value, valueType readValue)
        {
            fastCSharp.emit.memberCopyer<modelType>.Copy(value, readValue, memberMap);
        }
    }
}
