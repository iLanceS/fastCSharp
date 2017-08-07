using System;
using fastCSharp.code.cSharp;

namespace fastCSharp.sql.cache.part.events
{
    /// <summary>
    /// 自增id标识缓存计数器(反射模式)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    public sealed class identityCounter<valueType, modelType> : counter<valueType, modelType, long>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 自增id标识缓存计数器
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="group">数据分组</param>
        public identityCounter
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, int group = 0)
            : base(sqlTool, group, fastCSharp.emit.sqlModel<modelType>.GetIdentity)
        {
        }
    }
    /// <summary>
    /// 自增id标识缓存计数器(反射模式)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    public sealed class identityCounter32<valueType, modelType> : counter<valueType, modelType, int>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 自增id标识缓存计数器
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="group">数据分组</param>
        public identityCounter32
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, int group = 0)
            : base(sqlTool, group, fastCSharp.emit.sqlModel<modelType>.GetIdentity32)
        {
        }
    }
}